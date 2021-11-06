using EnvDTE;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ClassBuilderGenerator.Core
{
    public static class ProjectHelper
    {
        public static bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            hierarchy = null;
            itemid = VSConstants.VSITEMID_NIL;

            if(!(Package.GetGlobalService(typeof(SVsShellMonitorSelection)) is IVsMonitorSelection monitorSelection)
                || !(Package.GetGlobalService(typeof(SVsSolution)) is IVsSolution solution))
            {
                return false;
            }

            IntPtr hierarchyPtr = IntPtr.Zero;
            IntPtr selectionContainerPtr = IntPtr.Zero;

            try
            {
                var hr = monitorSelection.GetCurrentSelection(out hierarchyPtr,
                    out itemid,
                    out var multiItemSelect,
                    out selectionContainerPtr);

                if(ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL)
                {
                    return false;
                }

                if(multiItemSelect != null)
                    return false;

                if(itemid == VSConstants.VSITEMID_ROOT)
                    return false;

                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;

                if(hierarchy == null)
                    return false;

                if(ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out Guid guidProjectID)))
                {
                    return false;
                }

                return true;
            }
            finally
            {
                if(selectionContainerPtr != IntPtr.Zero)
                {
                    Marshal.Release(selectionContainerPtr);
                }

                if(hierarchyPtr != IntPtr.Zero)
                {
                    Marshal.Release(hierarchyPtr);
                }
            }
        }

        public static bool ProjectSupportsBuilders(IVsProject project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if(ErrorHandler.Failed(project.GetMkDocument(VSConstants.VSITEMID_ROOT, out var projectFullPath)))
                return false;

            string projectExtension = Path.GetExtension(projectFullPath);

            foreach(string supportedExtension in BuilderConstants.SupportedProjectExtensions)
            {
                if(projectExtension.Equals(supportedExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ItemSupportsBuilders(IVsProject project, uint itemid)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if(ErrorHandler.Failed(project.GetMkDocument(itemid, out var itemFullPath)))
                return false;

            // make sure its not a transform file itsle
            bool isAlreadyBuilderFile = IsItemBuilderItem(project, itemid);

            var transformFileInfo = new FileInfo(itemFullPath);
            bool isCSharpFile = transformFileInfo.Name.EndsWith(".cs");

            return (isCSharpFile && isAlreadyBuilderFile);
        }

        public static bool IsItemBuilderItem(IVsProject vsProject, uint itemid)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if(!(vsProject is IVsBuildPropertyStorage buildPropertyStorage))
                return false;

            bool isItemBuilderFile = false;

            buildPropertyStorage.GetItemAttribute(itemid, "IsBuilderFile", out var value);

            if(string.Compare("true", value, true) == 0)
                isItemBuilderFile = true;

            if(!isItemBuilderFile)
            {
                buildPropertyStorage.GetItemAttribute(itemid, "FullPath", out var filepath);

                if(!string.IsNullOrEmpty(filepath))
                {
                    var fi = new FileInfo(filepath);

                    if(fi.Name.EndsWith(".cs"))
                    {
                        isItemBuilderFile = true;
                    }
                }
            }
            return isItemBuilderFile;
        }

        public static string GetErrorInfo()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string errText = null;

            var uiShell = (IVsUIShell)Package.GetGlobalService(typeof(IVsUIShell));

            uiShell?.GetErrorInfo(out errText);

            return errText ?? string.Empty;
        }

        public static ProjectItem GetProjectItemFromHierarchy(IVsHierarchy pHierarchy, uint itemID)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ErrorHandler.ThrowOnFailure(pHierarchy.GetProperty(itemID,
                (int)__VSHPROPID.VSHPROPID_ExtObject, out var propertyValue));

            if(!(propertyValue is ProjectItem projectItem))
                return null;

            return projectItem;
        }
    }
}
