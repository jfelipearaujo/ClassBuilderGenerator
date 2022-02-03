using System.Collections.Generic;
using System.Linq;

namespace ClassBuilderGenerator.Core
{
    public class StringCheckBuilder
    {
        private Dictionary<string, bool> checks;

        public StringCheckBuilder()
        {
            Reset();
        }

        public StringCheckBuilder Reset()
        {
            checks = new Dictionary<string, bool>();
            return this;
        }

        public StringCheckBuilder IsList(string input)
        {
            checks.Add("List", input.RegexMatch("^List"));
            return this;
        }

        public StringCheckBuilder IsIEnumerable(string input)
        {
            checks.Add("IEnumerable", input.RegexMatch("^IEnumerable"));
            return this;
        }

        public StringCheckBuilder IsCollection(string input)
        {
            checks.Add("Collection", input.RegexMatch("^Collection"));
            return this;
        }

        public StringCheckBuilder IsICollection(string input)
        {
            checks.Add("ICollection", input.RegexMatch("^ICollection"));
            return this;
        }

        public StringCheckBuilder IsDictionary(string input)
        {
            checks.Add("Dictionary", input.RegexMatch("^Dictionary"));
            return this;
        }

        public bool CheckForOrCondition()
        {
            var result = checks.Any(condition => condition.Value);

            Reset();

            return result;
        }
    }
}
