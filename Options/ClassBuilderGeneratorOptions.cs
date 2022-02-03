using System.ComponentModel;

using ClassBuilderGenerator.Enums;

using Microsoft.VisualStudio.Shell;

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

        [Category("Generator")]
        [DisplayName("Behavior for missing properties")]
        [Description("This option configure the behavior when missing properties are detected")]
        [DefaultValue(MissingProperties.AlwaysAskWhatToDo)]
        [TypeConverter(typeof(EnumConverter))]
        public MissingProperties MissingProperties { get; set; } = MissingProperties.AlwaysAskWhatToDo;

        [Category("Generator")]
        [DisplayName("Generate Summary Information")]
        [Description("This option controls if the summary will be added to the methods")]
        [DefaultValue(true)]
        public bool GenerateSummaryInformation { get; set; } = true;

        [Category("Generator")]
        [DisplayName("Generate With method for collections")]
        [Description("This option controls if the With method will be added for properties like: List, IEnumerable, Collection, ICollection and Dictionary")]
        [DefaultValue(true)]
        public bool GenerateWithMethodForCollections { get; set; } = true;
    }
}
