using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassBuilderGenerator.Extensions
{
    public static class CodeAnalysisExtensions
    {
        public static IEnumerable<T> GetChildNodesOfType<T>(this SyntaxNode node)
        {
            return node.ChildNodes().OfType<T>();
        }

        public static string GetPropertyType(this SyntaxNode node)
        {
            foreach (var propertyTypeNode in node.GetChildNodesOfType<PredefinedTypeSyntax>())
            {
                return propertyTypeNode.ToString();
            }

            foreach (var propertyTypeNode in node.GetChildNodesOfType<IdentifierNameSyntax>())
            {
                return propertyTypeNode.Identifier.Text;
            }

            foreach (var listNode in node.GetChildNodesOfType<GenericNameSyntax>())
            {
                foreach (var typeArgumentListNode in listNode.GetChildNodesOfType<TypeArgumentListSyntax>())
                {
                    return $"{listNode.Identifier.Text}<{string.Join(", ", typeArgumentListNode.Arguments.Select(x => x.ToString()))}>";
                }
            }

            foreach (var arrayNode in node.GetChildNodesOfType<ArrayTypeSyntax>())
            {
                return $"{arrayNode.ElementType}{arrayNode.RankSpecifiers}";
            }

            return null;
        }

        public static bool IsPublicAccessible(this SyntaxTokenList syntaxTokenList)
        {
            return syntaxTokenList.Any(x => x.Text.Equals("public", StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsPrivateAccessible(this SyntaxTokenList syntaxTokenList)
        {
            return !syntaxTokenList.IsPublicAccessible();
        }
    }
}
