using ClassBuilderGenerator.Core;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

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
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;

            commandService.AddCommand(menuItem);
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if(sender is OleMenuCommand menuCommand)
            {
                menuCommand.Visible = false;
                menuCommand.Enabled = false;

                if(!ProjectHelper.IsSingleProjectItemSelection(out var hierarchy, out var itemid))
                    return;

                var vsProject = (IVsProject)hierarchy;

                if(!ProjectHelper.ProjectSupportsBuilders(vsProject))
                    return;

                if(!ProjectHelper.ItemSupportsBuilders(vsProject, itemid))
                    return;

                menuCommand.Visible = true;
                menuCommand.Enabled = true;
            }
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
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
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

            if(!ProjectHelper.IsSingleProjectItemSelection(out var hierarchy, out var itemid))
                return;

            var vsProject = (IVsProject)hierarchy;

            if(!ProjectHelper.ProjectSupportsBuilders(vsProject))
                return;

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

            BuilderCore.Generate(selectedProjectItem, hierarchy, itemFullPath);
        }
    }
}
