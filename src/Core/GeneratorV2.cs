using ClassBuilderGenerator.Enums;
using ClassBuilderGenerator.Extensions;
using ClassBuilderGenerator.Forms;
using ClassBuilderGenerator.Models;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ClassBuilderGenerator.Core
{
    public static class GeneratorV2
    {
        public static void CollectClassData(string sourceClassPath,
            ClassInformation classInformation,
            GeneratorOptions generatorOptions)
        {
            using (var stream = new FileStream(sourceClassPath, FileMode.Open))
            {
                var sourceText = SourceText.From(stream);
                var syntaxTree = CSharpSyntaxTree.ParseText(sourceText);
                var root = (CompilationUnitSyntax)syntaxTree.GetRoot();

                foreach (var usingNode in root.Usings.SelectMany(x => x.GetChildNodesOfType<QualifiedNameSyntax>()))
                {
                    classInformation.Usings.AddIfNotExists(usingNode.ToString());
                }

                foreach (var namespaceNode in root.GetChildNodesOfType<NamespaceDeclarationSyntax>())
                {
                    classInformation.Namespace = namespaceNode.Name.ToString();

                    foreach (var classNode in namespaceNode.GetChildNodesOfType<ClassDeclarationSyntax>())
                    {
                        classInformation.Name = classNode.Identifier.Text;
                        classInformation.IsPublicAccessible = classNode.Modifiers.IsPublicAccessible();

                        foreach (var constructorNode in classNode.GetChildNodesOfType<ConstructorDeclarationSyntax>())
                        {
                            var constructorProperties = new List<PropertyInformation>();

                            foreach (var parameter in constructorNode.ParameterList.Parameters)
                            {
                                constructorProperties.Add(new PropertyInformation
                                {
                                    Type = parameter.GetPropertyType(),
                                    OriginalName = parameter.Identifier.Text
                                });
                            }

                            var constructorDeclaration = $"{classInformation.Name}({string.Join(", ", constructorProperties.Select(x => x.OriginalName))})";

                            classInformation.Constructors.Add(constructorDeclaration, constructorProperties);
                        }

                        foreach (var fieldNode in classNode.GetChildNodesOfType<FieldDeclarationSyntax>())
                        {
                            if (fieldNode.Modifiers.IsPrivateAccessible())
                                continue;

                            classInformation.Properties.Add(new PropertyInformation
                            {
                                Type = fieldNode.Declaration.GetPropertyType(),
                                OriginalName = fieldNode.Declaration.Variables.FirstOrDefault().ToString()
                            });
                        }

                        foreach (var propertyNode in classNode.GetChildNodesOfType<PropertyDeclarationSyntax>())
                        {
                            if (propertyNode.Modifiers.IsPrivateAccessible())
                                continue;

                            classInformation.Properties.Add(new PropertyInformation
                            {
                                Type = propertyNode.GetPropertyType(),
                                OriginalName = propertyNode.Identifier.Text
                            });
                        }
                    }
                }

                if (classInformation.Constructors.Count > 1)
                {
                    var frmConstructorSelector = new FrmConstructorSelector(classInformation.Constructors.Keys.ToList())
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
                else if (classInformation.Constructors.Count == 1)
                {
                    classInformation.CustomConstructor = new CustomConstructor
                    {
                        Constructor = classInformation.Constructors.First().Key,
                        Properties = classInformation.Constructors.First().Value
                    };
                }

                // Check if all constructor properties are exposed to be created
                if (classInformation.CustomConstructor != null)
                {
                    var missingProperties = new List<PropertyInformation>();

                    foreach (var constructorProperty in classInformation.CustomConstructor.Properties)
                    {
                        if (!classInformation.Properties.Any(x => x.OriginalNameInCamelCase == constructorProperty.OriginalName
                             && x.Type == constructorProperty.Type))
                        {
                            missingProperties.Add(constructorProperty);
                        }
                    }

                    if (missingProperties.Any())
                    {
                        switch (generatorOptions.MissingPropertiesBehavior)
                        {
                            case MissingPropertiesBehavior.AlwaysAskWhatToDo:
                                {
                                    var frmMissingProperty = new FrmMissingProperty(classInformation.CustomConstructor, missingProperties)
                                    {
                                        StartPosition = FormStartPosition.CenterScreen
                                    };

                                    frmMissingProperty.ShowDialog();

                                    if (frmMissingProperty.ForceCreatingOfMissingProperties)
                                    {
                                        classInformation.Properties.AddRange(frmMissingProperty.MissingProperties);
                                    }
                                }
                                break;
                            case MissingPropertiesBehavior.AlwaysForceCreationOfMissingProperties:
                                {
                                    classInformation.Properties.AddRange(missingProperties);
                                }
                                break;
                            case MissingPropertiesBehavior.DoNothing:
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
