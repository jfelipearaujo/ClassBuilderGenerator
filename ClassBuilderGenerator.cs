using ClassBuilderGenerator.Core;
using ClassBuilderGenerator.Enums;
using ClassBuilderGenerator.Forms;

using EnvDTE;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

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

                var itemFolder = Path.GetDirectoryName(itemFullPath);
                var itemName = Path.GetFileNameWithoutExtension(itemFullPath);
                var itemBuilderName = $"{itemName}Builder.cs";
                var newItemFullPath = Path.Combine(itemFolder, itemBuilderName);

                IVsUIShell uiShell = (IVsUIShell)ServiceProvider.GetService(typeof(SVsUIShell));

                if(File.Exists(newItemFullPath))
                {
                    var message = new StringBuilder();

                    message.Append("There is already a file in this directory called '")
                        .Append(itemName).AppendLine("Builder'.")
                        .AppendLine()
                        .AppendLine("Do you want to overwrite this file?");

                    var result = VsShellUtilities.PromptYesNo(message.ToString(),
                        "File already exists",
                        OLEMSGICON.OLEMSGICON_QUERY,
                        uiShell);

                    if(!result)
                        return;
                }

                var options = package as ClassBuilderGeneratorPackage;

                var generateListWithItemMethod = options.GenerateListWithItemMethod;
                var methodWithGenerator = options.MethodWithGenerator;
                var missingPropertiesBehavior = options.MissingProperties;
                var generateSummaryInformation = options.GenerateSummaryInformation;

                var builderContent = new StringBuilder();
                var classInformation = new ClassInformation();

                CollectClassData(classInformation, selectedProjectItem, uiShell, missingPropertiesBehavior);

                GenerateBuilder(classInformation, builderContent, generateListWithItemMethod, methodWithGenerator, generateSummaryInformation);

                AddBuilderFile(builderContent.ToString(), itemBuilderName, itemFolder);
                AddBuilderToSolution(selectedProjectItem.ContainingProject, itemBuilderName, itemFolder, hierarchy);

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

        private void CollectClassData(ClassInformation classInformation,
            ProjectItem projectItem,
            IVsUIShell uiShell,
            MissingProperties missingPropertiesBehavior)
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
                                                    Type = x.TrimStart().Split(' ')[0].RemoveNamespace(classInformation),
                                                    OriginalName = x.TrimStart().Split(' ')[1]
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

                                classInformation.Properties.Add(new PropertyInformation
                                {
                                    Type = property.Type.AsString.RemoveNamespace(classInformation),
                                    OriginalName = property.Name
                                });
                            }
                        }
                    }
                }
            }

            if(classInformation.Constructors.Count > 1)
            {
                var frmConstructorSelector = new FrmConstructorSelector(package, uiShell, classInformation.Constructors.Keys.ToList())
                {
                    StartPosition = FormStartPosition.CenterScreen
                };

                frmConstructorSelector.ShowDialog();

                classInformation.CustomConstructor = new CustomConstructor
                {
                    Constructor = frmConstructorSelector.SelectedConstructor,
                    Properties = classInformation.Constructors.FirstOrDefault(x => x.Key == frmConstructorSelector.SelectedConstructor).Value
                };
            }
            else if(classInformation.Constructors.Count == 1)
            {
                classInformation.CustomConstructor = new CustomConstructor
                {
                    Constructor = classInformation.Constructors.First().Key,
                    Properties = classInformation.Constructors.First().Value
                };
            }

            // Check if all constructor properties are exposed to be created
            if(classInformation.CustomConstructor != null)
            {
                var missingProperties = new List<PropertyInformation>();

                foreach(var item in classInformation.CustomConstructor.Properties)
                {
                    if(!classInformation.Properties.Any(x => x.OriginalNameInCamelCase == item.OriginalNameInCamelCase
                        && x.Type == item.Type))
                    {
                        missingProperties.Add(item);
                    }
                }

                if(missingProperties.Any()
                    && missingPropertiesBehavior == MissingProperties.AlwaysAskWhatToDo)
                {
                    var frmMissingProperty = new FrmMissingProperty(classInformation.CustomConstructor,
                        missingProperties)
                    {
                        StartPosition = FormStartPosition.CenterScreen
                    };

                    frmMissingProperty.ShowDialog();

                    if(frmMissingProperty.ForceCreatingOfMissingProperties)
                    {
                        classInformation.Properties.AddRange(frmMissingProperty.MissingProperties);
                    }
                }
                else if(missingProperties.Any()
                    && missingPropertiesBehavior == MissingProperties.AlwaysForceCreationOfMissingProperties)
                {
                    classInformation.Properties.AddRange(missingProperties);
                }
            }
        }

        private void GenerateBuilder(ClassInformation classInformation,
            StringBuilder builderContent,
            bool generateListWithItemMethod,
            MethodWithGenerator methodWithGenerator,
            bool generateSummaryInformation)
        {
            #region Usings
            foreach(var item in classInformation.Usings)
            {
                builderContent.Append("using ")
                    .Append(item).AppendLine(";");
            }

            builderContent.AppendLine();
            #endregion

            #region Class header
            builderContent
                .Append("namespace ")
                .Append(classInformation.Namespace)
                .AppendLine()
                .AppendLine("{")
                .AddClassSummary(generateSummaryInformation, classInformation)
                .Append("\t")
                .Append("public class ")
                .AppendLine(classInformation.BuilderName)
                .Append("\t")
                .AppendLine("{");
            #endregion

            #region Private properties
            foreach(var item in classInformation.Properties)
            {
                if(classInformation.CustomConstructor == null)
                {
                    builderContent
                        .Append("\t")
                        .Append("\t")
                        .Append("private ")
                        .Append(item.Type)
                        .Append(" ")
                        .Append(item.OriginalNameInCamelCase)
                        .AppendLine(";");
                }
                else
                {
                    var addPropertyToBuilder = false;

                    switch(methodWithGenerator)
                    {
                        case MethodWithGenerator.GenerateAllProps:
                            addPropertyToBuilder = true;
                            break;
                        case MethodWithGenerator.PreferConstructorProps:
                        {
                            // Check if each property is used in the selected constructor
                            if(classInformation.CustomConstructor.Properties
                                .Any(x => x.OriginalName == item.OriginalName
                                    || x.OriginalNameInCamelCase == item.OriginalNameInCamelCase))
                            {
                                addPropertyToBuilder = true;
                            }
                        }
                        break;
                    }

                    if(addPropertyToBuilder)
                    {
                        builderContent
                            .Append("\t")
                            .Append("\t")
                            .Append("private ")
                            .Append(item.Type)
                            .Append(" ")
                            .Append(item.OriginalNameInCamelCase)
                            .AppendLine(";");
                    }
                }
            }
            #endregion

            #region Constructor
            builderContent
                    .AppendLine()
                    .AddBuilderConstructorSummary(generateSummaryInformation, classInformation)
                    .Append("\t")
                    .Append("\t")
                    .Append("public ")
                    .Append(classInformation.BuilderName)
                    .AppendLine("()")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("{")
                    .Append("\t")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("Reset();")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("}");
            #endregion

            #region Reset method
            builderContent
                .AppendLine()
                .AddResetSummary(generateSummaryInformation, classInformation)
                .Append("\t")
                .Append("\t")
                .Append("public ")
                .Append(classInformation.BuilderName)
                .AppendLine(" Reset()")
                .Append("\t")
                .Append("\t")
                .AppendLine("{");

            foreach(var item in classInformation.Properties)
            {
                var addResetProperty = false;

                switch(methodWithGenerator)
                {
                    case MethodWithGenerator.GenerateAllProps:
                        addResetProperty = true;
                        break;
                    case MethodWithGenerator.PreferConstructorProps:
                    {
                        // Check if each property is used in the selected constructor
                        if(classInformation.CustomConstructor.Properties
                            .Any(x => x.OriginalName == item.OriginalName
                                || x.OriginalNameInCamelCase == item.OriginalNameInCamelCase))
                        {
                            addResetProperty = true;
                        }
                    }
                    break;
                }

                if(addResetProperty)
                {
                    builderContent
                        .Append("\t")
                        .Append("\t")
                        .Append("\t")
                        .Append(item.OriginalNameInCamelCase)
                        .Append(" = ");

                    if(item.Type.Contains("List"))
                    {
                        builderContent
                            .Append("new ")
                            .Append(item.Type)
                            .Append("()");
                    }
                    else
                    {
                        builderContent.Append("default");
                    }

                    builderContent.AppendLine(";");
                }
            }

            builderContent
                .AppendLine()
                .Append("\t")
                .Append("\t")
                .Append("\t")
                .AppendLine("return this;")
                .Append("\t")
                .Append("\t")
                .AppendLine("}")
                .AppendLine();
            #endregion

            #region With methods
            foreach(var item in classInformation.Properties)
            {
                var addMethodWith = false;

                switch(methodWithGenerator)
                {
                    case MethodWithGenerator.GenerateAllProps:
                        addMethodWith = true;
                        break;
                    case MethodWithGenerator.PreferConstructorProps:
                        // Check if each property is used in the selected constructor
                        if(classInformation.CustomConstructor.Properties
                            .Any(x => x.OriginalName == item.OriginalName
                                || x.OriginalNameInCamelCase == item.OriginalNameInCamelCase))
                        {
                            addMethodWith = true;
                        }
                        break;
                }

                if(addMethodWith)
                {
                    builderContent
                        .AddWithSummary(generateSummaryInformation, classInformation, item)
                        .Append("\t")
                        .Append("\t")
                        .Append("public ")
                        .Append(classInformation.BuilderName)
                        .Append(" With")
                        .Append(item.OriginalName.ToTitleCase())
                        .Append("(")
                        .Append(item.Type)
                        .Append(" ")
                        .Append(item.OriginalNameInCamelCase)
                        .AppendLine(")")
                        .Append("\t")
                        .Append("\t")
                        .AppendLine("{")
                        .Append("\t")
                        .Append("\t")
                        .Append("\t")
                        .Append("this.")
                        .Append(item.OriginalNameInCamelCase)
                        .Append(" = ")
                        .Append(item.OriginalNameInCamelCase)
                        .AppendLine(";")
                        .Append("\t")
                        .Append("\t")
                        .Append("\t")
                        .AppendLine("return this;")
                        .Append("\t")
                        .Append("\t")
                        .AppendLine("}")
                        .AppendLine();

                    #region With Method for list item
                    if(generateListWithItemMethod && (item.Type.Contains("List")
                            || item.Type.Contains("IEnumerable")
                            || item.Type.Contains("Enumerable")
                            || item.Type.Contains("ICollection")))
                    {
                        var start = item.Type.IndexOf("<") + 1;
                        var end = item.Type.LastIndexOf(">");
                        var subListObject = item.Type.Substring(start, end - start);

                        if(subListObject.Contains("."))
                        {
                            subListObject = subListObject
                                .Substring(subListObject.LastIndexOf(".") + 1);
                        }

                        builderContent
                            .AddWithCollectionItemSummary(generateSummaryInformation, classInformation, item, subListObject)
                            .Append("\t")
                            .Append("\t")
                            .Append("public ")
                            .Append(classInformation.BuilderName)
                            .Append(" With")
                            .Append(item.OriginalName.ToTitleCase())
                            .Append("Item(")
                            .Append(subListObject)
                            .AppendLine(" item)")
                            .Append("\t")
                            .Append("\t")
                            .AppendLine("{")
                            .Append("\t")
                            .Append("\t")
                            .Append("\t");

                        if(item.Type.Contains("IEnumerable")
                            || item.Type.Contains("Enumerable"))
                        {
                            builderContent
                                .Append("Enumerable.Append(")
                                .Append(item.OriginalNameInCamelCase)
                                .AppendLine(", item);");
                        }
                        else
                        {
                            builderContent
                                .Append(item.OriginalNameInCamelCase)
                                .AppendLine(".Add(item);");
                        }

                        builderContent
                            .Append("\t")
                            .Append("\t")
                            .Append("\t")
                            .AppendLine("return this;")
                            .Append("\t")
                            .Append("\t")
                            .AppendLine("}")
                            .AppendLine();
                    }
                }
                #endregion
            }
            #endregion

            #region Build method
            builderContent
                .AddBuildSummary(generateSummaryInformation, classInformation)
                .Append("\t")
                .Append("\t")
                .Append("public ")
                .Append(classInformation.Name)
                .AppendLine(" Build()")
                .Append("\t")
                .Append("\t")
                .AppendLine("{")
                .Append("\t")
                .Append("\t")
                .Append("\t")
                .Append("return new ");

            if(classInformation.CustomConstructor == null)
            {
                builderContent
                    .AppendLine(classInformation.Name)
                    .Append("\t")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("{");

                foreach(var item in classInformation.Properties)
                {
                    builderContent
                        .Append("\t")
                        .Append("\t")
                        .Append("\t")
                        .Append("\t")
                        .Append(item.OriginalName)
                        .Append(" = ")
                        .Append(item.OriginalNameInCamelCase)
                        .AppendLine(",");
                }

                builderContent
                    .Append("\t")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("};");
            }
            else
            {
                builderContent
                    .Append(classInformation.CustomConstructor.Constructor)
                    .AppendLine(";");
            }

            builderContent
                .Append("\t")
                .Append("\t")
                .AppendLine("}")
                .Append("\t")
                .AppendLine("}")
                .Append("}");
            #endregion
        }

        private static void AddBuilderFile(string content, string itemName, string projectPath)
        {
            string itemPath = Path.Combine(projectPath, itemName);

            using(var writer = new StreamWriter(itemPath, false))
            {
                writer.Write(content);
            }
        }

        private static void AddBuilderToSolution(Project proj, string itemName, string projectPath, IVsHierarchy heirarchy)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string itemPath = Path.Combine(projectPath, itemName);

            if(!File.Exists(itemPath))
                return;

            heirarchy.ParseCanonicalName(itemPath, out var removeFileId);

            if(removeFileId < uint.MaxValue)
            {
                var itemToRemove = ProjectHelper.GetProjectItemFromHierarchy(heirarchy, removeFileId);

                itemToRemove?.Remove();
            }

            var addedItem = proj.ProjectItems.AddFromFile(itemPath);

            addedItem.Properties.Item("ItemType").Value = "Compile";
        }
    }
}
