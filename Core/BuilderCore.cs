using ClassBuilderGenerator.Enums;
using ClassBuilderGenerator.Forms;

using EnvDTE;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClassBuilderGenerator.Core
{
    public static class BuilderCore
    {
        public static void Generate(ProjectItem projectItem, IVsHierarchy hierarchy, string itemFullPath, AsyncPackage package, IVsUIShell uiShell)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            CodeElements elts = projectItem.FileCodeModel.CodeElements;

            var builderData = new BuilderData();

            var options = package as ClassBuilderGeneratorPackage;

            var generateListWithItemMethod = options.GenerateListWithItemMethod;
            var methodWithGenerator = options.MethodWithGenerator;

            for(int i = 1; i <= projectItem.FileCodeModel.CodeElements.Count; i++)
            {
                var elt = elts.Item(i);

                builderData = BuildData(elt, new BuilderData
                {
                    SubNamespaces = new List<string>
                    {
                        "System",
                        "System.Collections.Generic",
                        "System.Linq",
                        "System.Text",
                        "System.Threading.Tasks"
                    }
                }, package, uiShell, generateListWithItemMethod, methodWithGenerator);

                if(builderData.BuilderName != null)
                {
                    break;
                }
            }

            var builderContent = new StringBuilder();

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

        private static BuilderData BuildData(CodeElement elt,
            BuilderData builder, AsyncPackage package, IVsUIShell uiShell,
            bool generateListWithItemMethod, MethodWithGenerator methodWithGenerator)
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

                CodeClass codeClass = elt as CodeClass;

                for(int i = 1; i <= codeClass.Members.Count; i++)
                {
                    var subCodeElement = codeClass.Members.Item(i);

                    if(subCodeElement.Name == elt.Name
                        && subCodeElement.Kind == vsCMElement.vsCMElementFunction)
                    {
                        CodeFunction codeFunction = subCodeElement as CodeFunction;

                        foreach(CodeFunction item in codeFunction.Overloads)
                        {
                            var constructor = item.get_Prototype((int)vsCMPrototype.vsCMPrototypeParamNames)
                                .Replace(" (", "(");

                            if(!constructor.Contains("()") && !builder.Constructors.Contains(constructor))
                            {
                                builder.Constructors.Add(constructor);
                            }
                        }
                    }
                }

                if(builder.Constructors.Count > 1)
                {
                    var frmConstructorSelector = new FrmConstructorSelector(package, uiShell, builder.Constructors)
                    {
                        StartPosition = FormStartPosition.CenterScreen
                    };

                    frmConstructorSelector.ShowDialog();

                    builder.CustomConstructor = frmConstructorSelector.SelectedConstructor;
                }
                else if(builder.Constructors.Count == 1)
                {
                    builder.CustomConstructor = builder.Constructors.First();
                }

                CodeType ct = elt as CodeType;

                CodeElements mems = ct.Members;

                for(int i = 1; i <= ct.Members.Count; i++)
                {
                    BuildHelper.BuildPrivateProperties(mems.Item(i), builder, methodWithGenerator);
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
                    BuildHelper.BuildResetProperties(mems.Item(i), builder, methodWithGenerator);
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
                    BuildHelper.BuildWithMethods(mems.Item(i), builder, generateListWithItemMethod, methodWithGenerator);
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
                    .Append("return new ");

                if(!string.IsNullOrEmpty(builder.CustomConstructor))
                {
                    builder.BuilderBody
                        .Append(builder.CustomConstructor)
                        .AppendLine(";");
                }
                else
                {
                    builder.BuilderBody
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
                        .AppendLine("};");
                }

                builder.BuilderBody
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
                    BuildData(mems_vb.Item(i), builder, package, uiShell, generateListWithItemMethod, methodWithGenerator);
                }

                builder.BuilderBody.Append("}");
            }

            return builder;
        }
    }
}
