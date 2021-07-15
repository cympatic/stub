using Cympatic.Extensions.SpecFlow.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cympatic.Extensions.SpecFlow
{
    public static class TypeExtensions
    {
        public static IEnumerable<string> GetIgnoredProperties(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var ignored = false;
                foreach (var attribute in property.GetCustomAttributes(typeof(IgnoreAttribute), true).OfType<IgnoreAttribute>())
                {
                    yield return attribute.PropertyName;

                    ignored = true;
                }

                if (!ignored &&
                    property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    foreach (var propertyName in property.PropertyType.GetGenericArguments().First().GetIgnoredProperties())
                    {
                        yield return propertyName;
                    }
                }
            }
        }

        public static string GetSpecFlowItemName(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var attribute = type
                .GetCustomAttributes(typeof(SpecFlowItemAttribute), true)
                .FirstOrDefault() as SpecFlowItemAttribute;

            return attribute?.Name ?? type.Name;
        }

        public static IEnumerable<Type> GetAllClassesOf(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                    type.IsAssignableFrom(t) &&
                    !t.IsInterface &&
                    !t.IsAbstract);
        }
    }
}

