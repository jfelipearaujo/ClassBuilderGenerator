using EnvDTE;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using System.IO;
using System.Text;

namespace ClassBuilderGenerator.Core
{
    public static class BuilderCore
    {
        public static void Generate(ProjectItem projectItem, IVsHierarchy hierarchy, string itemFullPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            CodeElements elts = projectItem.FileCodeModel.CodeElements;

            var builderData = new BuilderData();

            for(int i = 1; i <= projectItem.FileCodeModel.CodeElements.Count; i++)
            {
                var elt = elts.Item(i);

                builderData = BuildData(elt, new BuilderData());

                if(builderData.BuilderName != null)
                {
                    break;
                }
            }

            var builderContent = new StringBuilder();

            builderContent.AppendLine("using System;");
            builderContent.AppendLine("using System.Collections.Generic;");
            builderContent.AppendLine("using System.Linq;");
            builderContent.AppendLine("using System.Text;");
            builderContent.AppendLine("using System.Threading.Tasks;");

            foreach(var subNamespace in builderData.SubNamespaces)
            {
                builderContent.Append("using ").Append(subNamespace).AppendLine(";");
            }

            builderContent.AppendLine();
            builderContent.Append(builderData.BuilderBody);

            string itemFolder = Path.GetDirectoryName(itemFullPath);
            string itemFilename = Path.GetFileNameWithoutExtension(itemFullPath);

            string itemName = itemFilename + "Builder.cs";

            AddBuilderFile(builderContent.ToString(), itemName, itemFolder);
            AddBuilderToSolution(projectItem.ContainingProject, itemName, itemFolder, hierarchy);
        }

        private static void AddBuilderFile(string content, string itemName, string projectPath)
        {
            string itemPath = Path.Combine(projectPath, itemName);

            if(!File.Exists(itemPath))
            {
                using(var writer = new StreamWriter(itemPath))
                {
                    writer.Write(content);
                }
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

        public static BuilderData BuildData(CodeElement elt, BuilderData builder)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var epStart = elt.StartPoint.CreateEditPoint();

            epStart.EndOfLine();

            if(elt.IsCodeType && (elt.Kind != vsCMElement.vsCMElementDelegate))
            {
                builder.BuilderName = elt.Name + "Builder";
                builder.ReturnType = elt.Name;

                builder.BuilderBody
                    .Append("\t")
                    .Append("public class ")
                    .AppendLine(builder.BuilderName)
                    .Append("\t")
                    .AppendLine("{");

                CodeType ct = elt as CodeType;

                CodeElements mems = ct.Members;

                for(int i = 1; i <= ct.Members.Count; i++)
                {
                    BuildHelper.BuildPrivateProperties(mems.Item(i), builder);
                }

                builder.BuilderBody
                    .AppendLine()
                    .Append("\t")
                    .Append("\t")
                    .Append("public ")
                    .Append(builder.BuilderName)
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

                builder.BuilderBody
                    .AppendLine()
                    .Append("\t")
                    .Append("\t")
                    .Append("public ")
                    .Append(builder.BuilderName)
                    .AppendLine(" Reset()")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("{");

                for(int i = 1; i <= ct.Members.Count; i++)
                {
                    BuildHelper.BuildResetProperties(mems.Item(i), builder);
                }

                builder.BuilderBody
                    .AppendLine()
                    .Append("\t")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("return this;")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("}")
                    .AppendLine();

                for(int i = 1; i <= ct.Members.Count; i++)
                {
                    BuildHelper.BuildWithMethods(mems.Item(i), builder);
                }

                builder.BuilderBody
                    .Append("\t")
                    .Append("\t")
                    .Append("public ")
                    .Append(builder.ReturnType)
                    .AppendLine(" Build()")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("{")
                    .Append("\t")
                    .Append("\t")
                    .Append("\t")
                    .Append("return new ")
                    .AppendLine(builder.ReturnType)
                    .Append("\t")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("{");

                for(int i = 1; i <= ct.Members.Count; i++)
                {
                    BuildHelper.BuilConstructorProperties(mems.Item(i), builder);
                }

                builder.BuilderBody
                    .Append("\t")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("};")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("}")
                    .Append("\t")
                    .AppendLine("}");
            }
            else if(elt.Kind == vsCMElement.vsCMElementNamespace)
            {
                builder.ClassNamespace = elt.Name;

                builder.BuilderBody.Append("namespace ")
                    .AppendLine(elt.Name)
                    .AppendLine("{");

                CodeNamespace cns = elt as CodeNamespace;

                CodeElements mems_vb = cns.Members;

                for(int i = 1; i <= cns.Members.Count; i++)
                {
                    BuildData(mems_vb.Item(i), builder);
                }

                builder.BuilderBody.Append("}");
            }

            return builder;
        }
    }
}
