using ClassBuilderGenerator.Core;

using EnvDTE;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Task = System.Threading.Tasks.Task;

namespace ClassBuilderGenerator
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ClassBuilderGenerator
    {
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassBuilderGenerator"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private ClassBuilderGenerator(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(BuilderConstants.CommandSet, BuilderConstants.CommandId);

            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);

            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ClassBuilderGenerator Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in ClassBuilderGenerator's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new ClassBuilderGenerator(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if(!ProjectHelper.IsSingleProjectItemSelection(out var hierarchy, out var itemid))
                {
                    VsShellUtilities.ShowMessageBox(
                        this.package,
                        "Please, select ONLY one class to create a builder!",
                        "Error",
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                    return;
                }

                var vsProject = (IVsProject)hierarchy;

                var projectExtension = ProjectHelper.ProjectSupportsBuilders(vsProject);

                if(projectExtension == "err" || !string.IsNullOrEmpty(projectExtension))
                {
                    VsShellUtilities.ShowMessageBox(
                        this.package,
                        $"This project extension ({projectExtension}) is not supported, sorry!",
                        "Error",
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                    return;
                }

                if(!ProjectHelper.ItemSupportsBuilders(vsProject, itemid))
                {
                    var error = new StringBuilder();

                    error.AppendLine("There are some possible errors:")
                        .AppendLine()
                        .AppendLine("- You're trying to create a builder from a non CSharp (.cs) class")
                        .AppendLine("- You're trying to create a builder from another builder");

                    VsShellUtilities.ShowMessageBox(
                        this.package,
                        error.ToString(),
                        "Error",
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                    return;
                }

                if(ErrorHandler.Failed(vsProject.GetMkDocument(VSConstants.VSITEMID_ROOT, out var projectFullPath)))
                    return;

                if(!(vsProject is IVsBuildPropertyStorage))
                    return;

                if(ErrorHandler.Failed(vsProject.GetMkDocument(itemid, out var itemFullPath)))
                    return;

                var solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
                int hr = solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_SaveIfDirty, hierarchy, 0);

                if(hr < 0)
                {
                    throw new COMException(string.Format("Failed to add project item {0} {1}", itemFullPath, ProjectHelper.GetErrorInfo()), hr);
                }

                var selectedProjectItem = ProjectHelper.GetProjectItemFromHierarchy(hierarchy, itemid);

                var fullPath = Path.GetDirectoryName(itemFullPath);
                var itemName = Path.GetFileNameWithoutExtension(itemFullPath);
                var newItemFullPath = Path.Combine(fullPath, $"{itemName}Builder.cs");

                IVsUIShell uiShell = (IVsUIShell)ServiceProvider.GetService(typeof(SVsUIShell));

                if(File.Exists(newItemFullPath))
                {
                    var message = new StringBuilder();

                    message.Append("There is already a file in this directory called '").Append(itemName).AppendLine("Builder'.")
                        .AppendLine()
                        .AppendLine("Do you want to overwrite this file?");

                    var result = VsShellUtilities.PromptYesNo(message.ToString(),
                        "File already exists",
                        OLEMSGICON.OLEMSGICON_QUERY,
                        uiShell);

                    if(!result)
                        return;
                }

                // ---
                var classInformation = new ClassInformation();

                CollectClassData(classInformation, selectedProjectItem);
                // ---

                BuilderCore.Generate(selectedProjectItem, hierarchy, itemFullPath, this.package, uiShell);

                var buildPropertyStorage = vsProject as IVsBuildPropertyStorage;

                hierarchy.ParseCanonicalName(newItemFullPath, out var addedFileId);
                buildPropertyStorage.SetItemAttribute(addedFileId, "IsBuilderFile", "True");
            }
            catch(Exception ex)
            {
                VsShellUtilities.ShowMessageBox(
                    this.package,
                    ex.Message,
                    "Error",
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        private void CollectClassData(ClassInformation classInformation, ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            CodeElements codeElements = projectItem.FileCodeModel.CodeElements;

            for(int i = 1; i <= projectItem.FileCodeModel.CodeElements.Count; i++)
            {
                CodeElement codeElement = codeElements.Item(i);

                if(codeElement.Kind == vsCMElement.vsCMElementNamespace)
                {
                    classInformation.Namespace = codeElement.Name;

                    CodeNamespace codeNamespace = codeElement as CodeNamespace;

                    CodeElements subCodeElements = codeNamespace.Members;

                    for(int j = 1; j <= codeNamespace.Members.Count; j++)
                    {
                        codeElement = subCodeElements.Item(j);

                        if(codeElement.IsCodeType && codeElement.Kind != vsCMElement.vsCMElementDelegate)
                        {
                            classInformation.Name = codeElement.Name;

                            CodeClass codeClass = codeElement as CodeClass;

                            for(int k = 1; k <= codeClass.Members.Count; k++)
                            {
                                CodeElement subCodeElement = codeClass.Members.Item(k);

                                // Collect constructor data
                                if(subCodeElement.Name == classInformation.Name
                                    && subCodeElement.Kind == vsCMElement.vsCMElementFunction)
                                {
                                    CodeFunction codeFunction = subCodeElement as CodeFunction;

                                    foreach(CodeFunction item in codeFunction.Overloads)
                                    {
                                        var constructor = item.get_Prototype((int)vsCMPrototype.vsCMPrototypeParamNames)
                                            .Replace(" (", "(");

                                        var constructorProperties = item.get_Prototype((int)(vsCMPrototype.vsCMPrototypeParamTypes
                                            | vsCMPrototype.vsCMPrototypeParamNames))
                                            .Replace(" (", "(");

                                        if(!constructor.Contains("()") && !classInformation.Constructors.ContainsKey(constructor))
                                        {
                                            var properties = constructorProperties
                                                .Replace($"{classInformation.Name}(", string.Empty)
                                                .Replace(")", string.Empty)
                                                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(x => new PropertyInformation
                                                {
                                                    Type = x.TrimStart().Split(' ')[0],
                                                    Name = x.TrimStart().Split(' ')[1]
                                                });

                                            classInformation.Constructors.Add(constructor, properties);
                                        }
                                    }
                                }
                            }

                            CodeType codeType = codeElement as CodeType;

                            CodeElements codeTypeCodeElements = codeType.Members;

                            for(int l = 1; l <= codeType.Members.Count; l++)
                            {
                                CodeElement codeTypeCodeElement = codeTypeCodeElements.Item(l);

                                if(codeTypeCodeElement.Kind != vsCMElement.vsCMElementProperty)
                                    continue;

                                var property = codeTypeCodeElement as CodeProperty;

                                if(property.Access != vsCMAccess.vsCMAccessPublic)
                                    continue;

                                var propertyInfo = new PropertyInformation
                                {
                                    Type = property.Type.AsString,
                                    Name = property.Name.ToCamelCase()
                                };

                                classInformation.Properties.Add(propertyInfo);

                                // check if its a List property
                                if(propertyInfo.Type.Contains("System.Collections.Generic"))
                                {
                                    var start = propertyInfo.Type.IndexOf("<") + 1;
                                    var end = propertyInfo.Type.LastIndexOf(">");
                                    var subPropType = propertyInfo.Type.Substring(start, end - start);

                                    // Check if have a namespace
                                    if(subPropType.Contains("."))
                                    {
                                        var propUsing = subPropType.Substring(0, subPropType.LastIndexOf("."));

                                        if(!classInformation.Usings.Contains(propUsing))
                                        {
                                            classInformation.Usings.Add(propUsing);
                                        }
                                    }
                                }
                                // Check if have a namespace
                                else if(propertyInfo.Type.Contains("."))
                                {
                                    var propUsing = propertyInfo.Type.Substring(0, propertyInfo.Type.LastIndexOf("."));

                                    if(!classInformation.Usings.Contains(propUsing))
                                    {
                                        classInformation.Usings.Add(propUsing);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
