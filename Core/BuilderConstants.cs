using System;
using System.Collections.Immutable;

namespace ClassBuilderGenerator.Core
{
    public static class BuilderConstants
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public static readonly int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("f0e84b49-6da3-4a14-8aea-77b443eed474");

        /// <summary>
        /// Supported project extensions
        /// </summary>
        public static readonly ImmutableList<string> SupportedProjectExtensions = ImmutableList
            .Create(".csproj", ".vbproj", ".fsproj");

        /// <summary>
        /// Reserved keywords
        /// </summary>
        public static readonly ImmutableList<string> ReservedKeywords = ImmutableList
            .Create("abstract", "add", "alias", "and", "as", "ascending", "async", "await",
                    "base", "bool", "break", "by", "byte",
                    "case", "catch", "char", "checked", "class", "const", "continue",
                    "decimal", "default", "delegate", "descending", "do", "double", "dynamic",
                    "else", "enum", "equals", "event", "explicit", "extern",
                    "false", "finally", "fixed", "float", "for", "foreach", "from",
                    "get", "global", "goto", "group",
                    "if", "implicit", "in", "init", "int", "interface", "internal", "into", "is",
                    "join",
                    "let", "lock", "long",
                    "managed",
                    "nameof", "namespace", "new", "nint", "not", "notnull", "nuint", "null",
                    "object", "on", "operator", "or", "orderby", "out", "override",
                    "params", "partial", "private", "protected", "public",
                    "readonly", "record", "ref", "remove", "return",
                    "sbyte", "sealed", "select", "set", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
                    "this", "throw", "true", "try", "typeof",
                    "uint", "ulong", "unchecked", "unmanaged", "unsafe", "ushort", "using",
                    "value", "var", "virtual", "void", "volatile",
                    "when", "where", "while", "with",
                    "yield");
    }
}
