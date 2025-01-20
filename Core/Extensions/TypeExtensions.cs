using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Xml.Serialization;

namespace Donatas.Core.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNullable(this Type type) => Nullable.GetUnderlyingType(type) != null;

        public static DisplayAttribute? GetDisplayAttribute(this Type type, string propertyName)
        {
            if (type == null)
                return null;

            var currentType = type.GenericTypeArguments.FirstOrDefault() ?? type;
            var property = currentType.GetProperty(propertyName);

            return property == null
                ? throw new ArgumentException($"{propertyName} is missing in {type.Name}")
                : property.GetCustomAttribute(typeof(DisplayAttribute), true) as DisplayAttribute;
        }

        public static string? GetXmlElementName(this Type obj)
        {
            var o = obj;
            string attr;
            do
            {
                attr = o.GetCustomAttributes(true).OfType<XmlRootAttribute>().FirstOrDefault()?.ElementName;
                o = o?.BaseType;
            } while (string.IsNullOrEmpty(attr) && o != null);
            return attr;
        }
    }
}
