using ClassBuilderGenerator.Options;

using EnvDTE;

using Microsoft.VisualStudio.Shell;

namespace ClassBuilderGenerator.Core
{
    public static class BuildHelper
    {
        public static void BuildPrivateProperties(CodeElement elt, BuilderData builder)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if(elt.Kind == vsCMElement.vsCMElementProperty)
            {
                var prop = elt as CodeProperty;
                var propType = prop.Type.AsString;

                var containNamespace = ContainNamespace(propType);
                var isPropGenericList = IsPropGenericList(propType);

                if(isPropGenericList)
                {
                    var listObjectType = GetListObjectType(propType);

                    if(ContainNamespace(listObjectType))
                    {
                        var propNamespace = GetNamespace(listObjectType);

                        if(!builder.SubNamespaces.Contains(propNamespace))
                        {
                            builder.SubNamespaces.Add(propNamespace);
                        }
                    }
                }
                else if(containNamespace)
                {
                    var propNamespace = GetNamespace(propType);

                    if(!builder.SubNamespaces.Contains(propNamespace))
                    {
                        builder.SubNamespaces.Add(propNamespace);
                    }
                }

                builder.BuilderBody
                    .Append("\t")
                    .Append("\t")
                    .Append("private ");

                if(isPropGenericList)
                {
                    var genericCollectionType = GetGenericCollectionType(propType);
                    var listObjectType = GetListObjectType(propType);

                    builder.BuilderBody
                        .Append(genericCollectionType)
                        .Append("<").Append(RemoveNamespace(listObjectType)).Append('>');
                }
                else if(containNamespace)
                {
                    builder.BuilderBody.Append(RemoveNamespace(propType));
                }
                else
                {
                    builder.BuilderBody.Append(propType);
                }

                builder.BuilderBody
                    .Append(" ")
                    .Append(elt.Name.ToCamelCase())
                    .AppendLine(";");
            }
        }

        public static void BuildResetProperties(CodeElement elt, BuilderData builder)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if(elt.Kind == vsCMElement.vsCMElementProperty)
            {
                var prop = elt as CodeProperty;

                builder.BuilderBody
                    .Append("\t")
                    .Append("\t")
                    .Append("\t")
                    .Append(elt.Name.ToCamelCase())
                    .Append(" = ")
                    .Append(GetDefaultValueFromProperty(prop.Type))
                    .AppendLine(";");
            }
        }

        public static void BuildWithMethods(CodeElement elt, BuilderData builder)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if(elt.Kind == vsCMElement.vsCMElementProperty)
            {
                var prop = elt as CodeProperty;
                var propType = prop.Type.AsString;

                var containNamespace = ContainNamespace(propType);
                var isPropGenericList = IsPropGenericList(propType);

                builder.BuilderBody
                    .Append("\t")
                    .Append("\t")
                    .Append("public ")
                    .Append(builder.BuilderName)
                    .Append(" With")
                    .Append(elt.Name)
                    .Append("(");

                if(isPropGenericList)
                {
                    var listObjectType = GetListObjectType(propType);

                    builder.BuilderBody.Append("List<").Append(RemoveNamespace(listObjectType)).Append('>');
                }
                else if(containNamespace)
                {
                    builder.BuilderBody.Append(RemoveNamespace(propType));
                }
                else
                {
                    builder.BuilderBody.Append(propType);
                }

                builder.BuilderBody
                    .Append(" ")
                    .Append(elt.Name.ToCamelCase())
                    .AppendLine(")")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("{")
                    .Append("\t")
                    .Append("\t")
                    .Append("\t")
                    .Append("this.")
                    .Append(elt.Name.ToCamelCase())
                    .Append(" = ")
                    .Append(elt.Name.ToCamelCase())
                    .AppendLine(";")
                    .Append("\t")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("return this;")
                    .Append("\t")
                    .Append("\t")
                    .AppendLine("}")
                    .AppendLine();

                if(isPropGenericList && GeneralOptions.Instance.GenerateListWithItemMethod)
                {
                    var genericCollectionType = GetGenericCollectionType(propType);

                    builder.BuilderBody
                        .Append("\t")
                        .Append("\t")
                        .Append("public ")
                        .Append(builder.BuilderName)
                        .Append(" With")
                        .Append(elt.Name)
                        .Append("Item(")
                        .Append(RemoveNamespace(GetListObjectType(propType)))
                        .AppendLine(" item)")
                        .Append("\t")
                        .Append("\t")
                        .AppendLine("{")
                        .Append("\t")
                        .Append("\t")
                        .Append("\t");

                    if(genericCollectionType == "IEnumerable"
                        || genericCollectionType == "Enumerable")
                    {
                        builder.BuilderBody
                            .Append("Enumerable.Append(")
                            .Append(elt.Name.ToCamelCase())
                            .AppendLine(", item);");
                    }
                    else
                    {
                        builder.BuilderBody
                            .Append(elt.Name.ToCamelCase())
                            .AppendLine(".Add(item);");
                    }

                    builder.BuilderBody
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
        }

        public static void BuilConstructorProperties(CodeElement elt, BuilderData builder)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if(elt.Kind == vsCMElement.vsCMElementProperty)
            {
                builder.BuilderBody
                    .Append("\t")
                    .Append("\t")
                    .Append("\t")
                    .Append("\t")
                    .Append(elt.Name)
                    .Append(" = ")
                    .Append(elt.Name.ToCamelCase())
                    .AppendLine(",");
            }
        }

        private static string GetDefaultValueFromProperty(CodeTypeRef codeTypeRef)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var type = codeTypeRef.AsString;

            switch(codeTypeRef.TypeKind)
            {
                case vsCMTypeRef.vsCMTypeRefCodeType:
                {
                    if(IsPropGenericList(type))
                    {
                        switch(GetGenericCollectionType(type))
                        {
                            case "List":
                                return $"new List<{RemoveNamespace(GetListObjectType(type))}>()";
                            default:
                                return "default";
                        }
                    }

                    return "default";
                }
                default:
                    return "default";
            }
        }

        private static bool IsPropGenericList(string propType)
        {
            return propType.Contains("System.Collections.Generic");
        }

        private static string GetListObjectType(string listFullType)
        {
            var start = listFullType.IndexOf("<") + 1;
            var end = listFullType.LastIndexOf(">");

            return listFullType.Substring(start, end - start);
        }

        private static bool ContainNamespace(string propType)
        {
            return propType.Contains(".");
        }

        private static string GetNamespace(string propType)
        {
            return propType.Substring(0, propType.LastIndexOf("."));
        }

        private static string RemoveNamespace(string propType)
        {
            return propType.Substring(propType.LastIndexOf(".") + 1);
        }

        private static string GetGenericCollectionType(string collection)
        {
            var temp = collection.Replace("System.Collections.Generic.", string.Empty);

            return temp.Substring(0, temp.IndexOf("<"));
        }
    }
}
