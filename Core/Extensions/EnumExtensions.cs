using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Donatas.Core.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns the value stored in the description attribute of the target Enum member
        /// </summary>
        /// <param name="value">The target enum member</param>
        /// <returns>The description of the enum item</returns>
        public static string GetDescription(this Enum value)
        {
            var fi = value?.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi?.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes?.Length > 0 ? attributes[0].Description : Regex.Replace(value?.ToString(), "([a-z])([A-Z])", "$1 $2");
        }

        public static T GetAttributeValue<T>(this Enum value)
        {
            var fi = value?.GetType().GetField(value.ToString());
            var attributes = fi?.GetCustomAttributes(typeof(T), false);
            return (attributes?.Length == 1)
                ? (T)attributes[0]
                : throw new NullReferenceException($"Attribute '{typeof(T).Name}' not found on enum '{value?.GetType().Name}'");
        }

        public static string ToEnum<T>(this sbyte ind)
        {
            ThrowIfNotEnum<T>();

            var typedValue = Convert.ChangeType(ind, Enum.GetUnderlyingType(typeof(T)));

            if (!Enum.IsDefined(typeof(T), typedValue))
                throw new ArgumentException($"Enum '{typeof(T).Name}' index '{ind}' is out of range");

            // Since enum names does not allow spaces, this is the work arround:
            var ret = Enum.Parse(typeof(T), $"{ind}").ToString().Replace("_", " ");
            return ret;
        }

        public static T GetEnumFromDescription<T>(string description)
        {
            var type = ThrowIfNotEnum<T>();

            foreach (var _ in type.GetFields())
            {
                if ((Attribute.GetCustomAttribute(_, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                    && (attribute.Description == description)
                    || (_.Name == description))
                    return (T)_.GetValue(null);
            }

            throw new ArgumentException($"Description '{description}' not found on enum '{type}'");
        }

        public static string EnumToString(this Enum value)
        {
            return Regex.Replace(value?.ToString() ?? String.Empty, "([a-z])_?([A-Z])", "$1 $2");
        }

        private static Type ThrowIfNotEnum<T>() =>
            ((typeof(T)).IsEnum)
            ? typeof(T)
            : throw new InvalidOperationException($"Type '{nameof(T)}' is not an enum");
    }
}
