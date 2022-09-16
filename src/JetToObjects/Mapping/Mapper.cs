using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ekm.oledb.data
{
    public static class Mapper
    {
        public static TDest Map<TDest, TMapping>(dynamic source) where TDest : class, new()
            where TMapping : ObjectMapping
        {
            var result = Activator.CreateInstance<TDest>();
            var mapping = Activator.CreateInstance<TMapping>();
            var destinationProperties = GetProperties(result);

            foreach (var sourceProperty in source as IDictionary<string, object>)
            {
                var property = FindProperty(destinationProperties, mapping, sourceProperty.Key);

                if (property != null)
                    property.SetValue(result, sourceProperty.Value, null);
            }

            return result;
        }

        public static IEnumerable<TDest> MapMany<TDest, TMapping>(dynamic source) where TDest : class, new()
            where TMapping : ObjectMapping
        {
            var items = new List<TDest>();

            foreach (var item in source)
                items.Add(Map<TDest, TMapping>(item));

            return items;
        }

        private static PropertyInfo FindProperty<TMapping>(PropertyInfo[] properties, TMapping mapping,
            string propertyName) where TMapping : ObjectMapping
        {
            if (mapping.IsExcluded(propertyName))
                return null;

            return properties.SingleOrDefault(dp => String.Equals(dp.Name, mapping.Get(propertyName), StringComparison.CurrentCultureIgnoreCase)) ??
                   properties.SingleOrDefault(dp => String.Equals(dp.Name, propertyName, StringComparison.CurrentCultureIgnoreCase));
        }

        private static PropertyInfo[] GetProperties<T>(T source)
        {
            return source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
    }
}