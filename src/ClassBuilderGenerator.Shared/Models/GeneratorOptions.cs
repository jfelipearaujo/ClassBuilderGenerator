using Shared.Enums;

namespace Shared.Models
{
    public class GeneratorOptions
    {
        public bool GenerateListWithItemMethod { get; set; }
        public bool GenerateSummaryInformation { get; set; }
        public bool GenerateWithMethodForCollections { get; set; }
        public MethodWithGenerator MethodWithGenerator { get; set; }
        public bool AddUnderscorePrefixToTheFields { get; set; }
    }
}