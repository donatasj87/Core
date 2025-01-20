using System.Reflection;

namespace Donatas.Core.Extensions
{
    public static class AttributeExtensions
    {
        public static T? GetCustomAttribute<T>(this ICustomAttributeProvider t) where T : Attribute
        {
            if (t == null)
                return null;

            var attr = t.GetCustomAttributes(typeof(T), false);

            return (attr.FirstOrDefault() != null)
                ? attr.FirstOrDefault() as T
                : throw new ArgumentException($"Attribute '{typeof(T).Name}' not found on type {t}");
        }
    }
}
