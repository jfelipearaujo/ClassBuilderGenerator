﻿using System.Collections.Generic;

namespace ClassBuilderGenerator.Models
{
    public class ClassInformation
    {
        public ClassInformation()
        {
            Usings = new List<string>
            {
                "System",
                "System.Collections.Generic",
                "System.Linq",
                "System.Text",
                "System.Threading.Tasks",
                "System.Collections.ObjectModel"
            };

            Constructors = new Dictionary<string, IEnumerable<PropertyInformation>>();
            Properties = new List<PropertyInformation>();
        }

        public string Name { get; set; }
        public string BuilderName => $"{Name}Builder";
        public string Namespace { get; set; }
        public bool IsPublicAccessible { get; set; }
        public CustomConstructor CustomConstructor { get; set; }
        public List<string> Usings { get; set; }
        public Dictionary<string, IEnumerable<PropertyInformation>> Constructors { get; set; }
        public List<PropertyInformation> Properties { get; set; }
    }
}