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
    }
}
