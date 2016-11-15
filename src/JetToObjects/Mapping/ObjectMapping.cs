using System.Collections.Generic;

namespace JetToObjects.Mapping
{
    public class ObjectMapping
    {
        private readonly List<string> _exclusions = new List<string>();
        private readonly Dictionary<string, string> _mappings = new Dictionary<string, string>();

        public void Map(string from, string to)
        {
            _mappings.Add(from, to);
        }

        public string Get(string from)
        {
            string value;
            return _mappings.TryGetValue(from, out value) ? value : null;
        }

        public void Exclude(string field)
        {
            _exclusions.Add(field);
        }

        public bool IsExcluded(string field)
        {
            return _exclusions.Contains(field);
        }
    }
}