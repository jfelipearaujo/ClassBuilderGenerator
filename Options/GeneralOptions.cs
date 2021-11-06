using System.ComponentModel;

namespace ClassBuilderGenerator.Options
{
    internal class GeneralOptions : BaseOptionModel<GeneralOptions>
    {
        [Category("Lists")]
        [DisplayName("Generate WithItem Method")]
        [Description("Specifies if the generator will create or not the With method to add an item into the list property")]
        [DefaultValue(true)]
        public bool GenerateListWithItemMethod { get; set; } = true;
    }
}
