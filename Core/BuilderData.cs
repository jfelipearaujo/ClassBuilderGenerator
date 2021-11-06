using System.Collections.Generic;
using System.Text;

namespace ClassBuilderGenerator.Core
{
    public class BuilderData
    {
        public string ClassNamespace { get; set; }
        public string BuilderName { get; set; }
        public string ReturnType { get; set; }
        public StringBuilder BuilderBody { get; set; }
        public List<string> SubNamespaces { get; set; }

        public BuilderData()
        {
            BuilderBody = new StringBuilder();
            SubNamespaces = new List<string>();
        }
    }
}
