namespace ClassBuilderGenerator.Core
{
    public class PropertyInformation
    {
        public string Type { get; set; }
        public string OriginalName { get; set; }
        public string OriginalNameInCamelCase
        {
            get
            {
                return OriginalName.ToCamelCase();
            }
        }
    }
}
