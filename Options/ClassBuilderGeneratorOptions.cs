using ClassBuilderGenerator.Enums;

using Microsoft.VisualStudio.Shell;

using System.ComponentModel;

namespace ClassBuilderGenerator.Options
{
    public class ClassBuilderGeneratorOptions : DialogPage
    {
        [Category("Lists")]
        [DisplayName("Generate WithItem Method")]
        [Description("Specifies if the generator will create or not the With method to add an item into the list property")]
        [DefaultValue(true)]
        public bool GenerateListWithItemMethod { get; set; } = true;

        [Category("Generator")]
        [DisplayName("With Method Generator Handler")]
        [Description("This enum will define how the generator should handle which 'with' method will be generated")]
        [DefaultValue(MethodWithGenerator.GenerateAllProps)]
        [TypeConverter(typeof(EnumConverter))]
        public MethodWithGenerator WithMethodGeneratorHandler { get; set; } = MethodWithGenerator.GenerateAllProps;
    }
}
