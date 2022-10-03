using ClassBuilderGenerator.Enums;
using ClassBuilderGenerator.Extensions;
using ClassBuilderGenerator.Helpers;
using ClassBuilderGenerator.Models;

using System.Linq;
using System.Text;

namespace ClassBuilderGenerator.Core
{
    public static class Generator
    {
        public static void GenerateBuilder(ClassInformation classInformation, StringBuilder builderContent, GeneratorOptions options)
        {
            #region Usings
            foreach (var item in classInformation.Usings)
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
                .AddClassSummary(options.GenerateSummaryInformation, classInformation)
                .AppendTab()
                .Append("public class ")
                .AppendLine(classInformation.BuilderName)
                .AppendTab()
                .AppendLine("{");
            #endregion

            #region Private properties
            foreach (var item in classInformation.Properties)
            {
                if (classInformation.CustomConstructor == null)
                {
                    builderContent
                        .AppendTab(2)
                        .Append("private ")
                        .Append(item.Type)
                        .Append(" ")
                        .AppendWhenTrue(options.AddUnderscorePrefixToTheFields, "_")
                        .Append(item.OriginalNameInCamelCase)
                        .AppendLine(";");
                }
                else
                {
                    var addPropertyToBuilder = false;

                    switch (options.MethodWithGenerator)
                    {
                        case MethodWithGenerator.GenerateAllProps:
                            addPropertyToBuilder = true;
                            break;
                        case MethodWithGenerator.PreferConstructorProps:
                            {
                                // Check if each property is used in the selected constructor
                                if (classInformation.CustomConstructor.Properties
                                    .Any(x => x.OriginalName == item.OriginalName
                                        || x.OriginalNameInCamelCase == item.OriginalNameInCamelCase))
                                {
                                    addPropertyToBuilder = true;
                                }
                            }
                            break;
                    }

                    if (addPropertyToBuilder)
                    {
                        builderContent
                            .AppendTab(2)
                            .Append("private ")
                            .Append(item.Type)
                            .Append(" ")
                            .AppendWhenTrue(options.AddUnderscorePrefixToTheFields, "_")
                            .Append(item.OriginalNameInCamelCase)
                            .AppendLine(";");
                    }
                }
            }
            #endregion

            #region Constructor
            builderContent
                .AppendLine()
                .AddBuilderConstructorSummary(options.GenerateSummaryInformation, classInformation)
                .AppendTab(2)
                .Append("public ")
                .Append(classInformation.BuilderName)
                .AppendLine("()")
                .AppendTab(2)
                .AppendLine("{")
                .AppendTab(3)
                .AppendLine("Reset();")
                .AppendTab(2)
                .AppendLine("}");
            #endregion

            #region Reset method
            builderContent
                .AppendLine()
                .AddResetSummary(options.GenerateSummaryInformation, classInformation)
                .AppendTab(2)
                .Append("public ")
                .Append(classInformation.BuilderName)
                .AppendLine(" Reset()")
                .AppendTab(2)
                .AppendLine("{");

            foreach (var item in classInformation.Properties)
            {
                var addResetProperty = false;

                switch (options.MethodWithGenerator)
                {
                    case MethodWithGenerator.GenerateAllProps:
                        addResetProperty = true;
                        break;
                    case MethodWithGenerator.PreferConstructorProps:
                        {
                            // Check if each property is used in the selected constructor
                            if (classInformation.CustomConstructor.Properties
                                .Any(x => x.OriginalName == item.OriginalName
                                    || x.OriginalNameInCamelCase == item.OriginalNameInCamelCase))
                            {
                                addResetProperty = true;
                            }
                        }
                        break;
                }

                if (addResetProperty)
                {
                    builderContent
                        .AppendTab(3)
                        .AppendWhenTrue(options.AddUnderscorePrefixToTheFields, "_")
                        .Append(item.OriginalNameInCamelCase)
                        .Append(" = ");

                    if (item.CollectionType.IsListType())
                    {
                        builderContent
                            .Append("new List<")
                            .Append(item.Type.GetEnumerableKeyType())
                            .Append(">()");
                    }
                    else if (item.CollectionType.IsArrayType())
                    {
                        builderContent
                            .Append("Array.Empty<")
                            .Append(item.Type.GetArrayType())
                            .Append(">()");
                    }
                    else if (item.CollectionType.IsMatrix2DimType())
                    {
                        builderContent
                            .Append("Array.Empty<")
                            .Append(item.Type.GetArrayType())
                            .Append("[]>()");
                    }
                    else if (item.CollectionType.IsDictionaryType())
                    {
                        builderContent
                            .Append("new Dictionary<")
                            .Append(item.Type.GetDictionaryKeyType())
                            .Append(", ")
                            .Append(item.Type.GetDictionaryValueType())
                            .Append(">()");
                    }
                    else if (item.CollectionType.IsCollectionType())
                    {
                        builderContent
                            .Append("new Collection<")
                            .Append(item.Type.GetEnumerableKeyType())
                            .Append(">()");
                    }
                    else if (item.CollectionType.IsEnumerableType())
                    {
                        builderContent
                            .Append("Enumerable.Empty<")
                            .Append(item.Type.GetEnumerableKeyType())
                            .Append(">()");
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
                .AppendTab(3)
                .AppendLine("return this;")
                .AppendTab(2)
                .AppendLine("}")
                .AppendLine();
            #endregion

            #region With methods
            foreach (var item in classInformation.Properties)
            {
                var addMethodWith = false;

                switch (options.MethodWithGenerator)
                {
                    case MethodWithGenerator.GenerateAllProps:
                        addMethodWith = true;
                        break;
                    case MethodWithGenerator.PreferConstructorProps:
                        // Check if each property is used in the selected constructor
                        if (classInformation.CustomConstructor.Properties
                            .Any(x => x.OriginalName == item.OriginalName
                                || x.OriginalNameInCamelCase == item.OriginalNameInCamelCase))
                        {
                            addMethodWith = true;
                        }
                        break;
                }

                if (options.GenerateWithMethodForCollections == false)
                {
                    addMethodWith = false;
                }

                if (addMethodWith &&
                    (item.CollectionType.IsValidCollection()
                    || item.CollectionType.IsValidArrayOrMatrix()))
                {
                    builderContent
                        .AddWithSummary(options.GenerateSummaryInformation, classInformation, item)
                        .AppendTab(2)
                        .Append("public ")
                        .Append(classInformation.BuilderName)
                        .Append(" With")
                        .Append(item.OriginalName.ToTitleCase())
                        .Append("(")
                        .Append(item.Type)
                        .Append(" ")
                        .Append(item.OriginalNameInCamelCase)
                        .AppendLine(")")
                        .AppendTab(2)
                        .AppendLine("{")
                        .AppendTab(3)
                        .AppendWhen(options.AddUnderscorePrefixToTheFields, "_", "this.")
                        .Append(item.OriginalNameInCamelCase)
                        .Append(" = ")
                        .Append(item.OriginalNameInCamelCase)
                        .AppendLine(";")
                        .AppendTab(3)
                        .AppendLine("return this;")
                        .AppendTab(2)
                        .AppendLine("}")
                        .AppendLine();
                }


                if (options.GenerateListWithItemMethod
                    && item.CollectionType.IsValidArrayOrMatrix())
                {
                    builderContent
                        .AddWithArrayOrMatrixItemSummary(options.GenerateSummaryInformation, classInformation, item)
                        .AppendTab(2)
                        .Append("public ")
                        .Append(classInformation.BuilderName)
                        .Append(" With")
                        .Append(item.OriginalName.ToTitleCase())
                        .Append("Item(")
                        .Append(item.Type.GetArrayType())
                        .AppendWhenTrue(item.CollectionType.IsMatrix2DimType(), "[]")
                        .AppendLine(" item)")
                        .AppendTab(2)
                        .AppendLine("{")
                        .AppendTab(3);

                    builderContent
                        .AppendWhenTrue(options.AddUnderscorePrefixToTheFields, "_")
                        .Append(item.OriginalNameInCamelCase)
                        .Append(" = ")
                        .AppendWhenTrue(options.AddUnderscorePrefixToTheFields, "_")
                        .Append(item.OriginalNameInCamelCase)
                        .AppendLine(".Append(item).ToArray();");

                    builderContent
                        .AppendTab(3)
                        .AppendLine("return this;")
                        .AppendTab(2)
                        .AppendLine("}")
                        .AppendLine();
                }
                else if (options.GenerateListWithItemMethod
                    && item.CollectionType.IsCollectionButNotKeyValue())
                {
                    builderContent
                        .AddWithCollectionItemSummary(options.GenerateSummaryInformation, classInformation, item)
                        .AppendTab(2)
                        .Append("public ")
                        .Append(classInformation.BuilderName)
                        .Append(" With")
                        .Append(item.OriginalName.ToTitleCase())
                        .Append("Item(")
                        .Append(item.Type.GetEnumerableKeyType())
                        .AppendLine(" item)")
                        .AppendTab(2)
                        .AppendLine("{")
                        .AppendTab(3);

                    if (item.CollectionType.IsEnumerableType())
                    {
                        builderContent
                            .AppendWhenTrue(options.AddUnderscorePrefixToTheFields, "_")
                            .Append(item.OriginalNameInCamelCase)
                            .Append(" = ")
                            .Append("Enumerable.Append(")
                            .AppendWhenTrue(options.AddUnderscorePrefixToTheFields, "_")
                            .Append(item.OriginalNameInCamelCase)
                            .AppendLine(", item);");
                    }
                    else
                    {
                        builderContent
                            .AppendWhenTrue(options.AddUnderscorePrefixToTheFields, "_")
                            .Append(item.OriginalNameInCamelCase)
                            .AppendLine(".Add(item);");
                    }

                    builderContent
                        .AppendTab(3)
                        .AppendLine("return this;")
                        .AppendTab(2)
                        .AppendLine("}")
                        .AppendLine();
                }
                else if (options.GenerateListWithItemMethod
                    && item.CollectionType.IsDictionaryType())
                {
                    builderContent
                        .AddWithCollectionItemSummary(options.GenerateSummaryInformation, classInformation, item)
                        .AppendTab(2)
                        .Append("public ")
                        .Append(classInformation.BuilderName)
                        .Append(" With")
                        .Append(item.OriginalName.ToTitleCase())
                        .Append("Item(")
                        .Append(item.Type.GetDictionaryKeyType())
                        .Append(" key, ")
                        .Append(item.Type.GetDictionaryValueType())
                        .AppendLine(" value)")
                        .AppendTab(2)
                        .AppendLine("{")
                        .AppendTab(3);

                    builderContent
                        .AppendWhenTrue(options.AddUnderscorePrefixToTheFields, "_")
                        .Append(item.OriginalNameInCamelCase)
                        .AppendLine(".Add(key, value);");

                    builderContent
                        .AppendTab(3)
                        .AppendLine("return this;")
                        .AppendTab(2)
                        .AppendLine("}")
                        .AppendLine();
                }
            }
            #endregion

            #region Build method
            builderContent
                .AddBuildSummary(options.GenerateSummaryInformation, classInformation)
                .AppendTab(2)
                .AppendWhen(classInformation.IsPublicAccessible, "public ", "internal ")
                .Append(classInformation.Name)
                .AppendLine(" Build()")
                .AppendTab(2)
                .AppendLine("{")
                .AppendTab(3)
                .Append("return new ");

            if (classInformation.CustomConstructor == null)
            {
                builderContent
                    .AppendLine(classInformation.Name)
                    .AppendTab(3)
                    .AppendLine("{");

                foreach (var item in classInformation.Properties)
                {
                    builderContent
                        .AppendTab(4)
                        .Append(item.OriginalName)
                        .Append(" = ")
                        .AppendWhenTrue(options.AddUnderscorePrefixToTheFields, "_")
                        .Append(item.OriginalNameInCamelCase)
                        .AppendLine(",");
                }

                builderContent
                    .AppendTab(3)
                    .AppendLine("};");
            }
            else
            {
                builderContent
                    .Append(classInformation.Name)
                    .Append("(");

                foreach (var constructorProperty in classInformation.CustomConstructor.Properties)
                {
                    builderContent
                        .AppendWhenTrue(options.AddUnderscorePrefixToTheFields, "_")
                        .Append(constructorProperty.OriginalName)
                        .Append(", ");
                }

                builderContent.Remove(builderContent.Length - 2, 2);

                builderContent.AppendLine(");");
            }

            builderContent
                .AppendTab(2)
                .AppendLine("}")
                .AppendTab()
                .AppendLine("}")
                .Append("}");
            #endregion
        }
    }
}
