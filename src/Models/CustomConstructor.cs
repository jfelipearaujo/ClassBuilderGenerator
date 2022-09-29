using System.Collections.Generic;

namespace ClassBuilderGenerator.Models
{
    public class CustomConstructor
    {
        public string Constructor { get; set; }
        public IEnumerable<PropertyInformation> Properties { get; set; }
    }
}