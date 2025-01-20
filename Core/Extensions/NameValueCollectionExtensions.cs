using System.Collections.Specialized;
using System.Text;

namespace Donatas.Core.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static IDictionary<string, string?> ToDictionary(this NameValueCollection? nameValueCollection)
        {
            var dict = new Dictionary<string, string?>();

            foreach (var key in nameValueCollection?.AllKeys.Where(k => k != null))
                dict.Add(key, nameValueCollection[key]);

            return dict;
        }

        public static string ToDisplayString(this NameValueCollection? nameValueCollection)
        {
            var displayString = new StringBuilder();

            foreach (var key in nameValueCollection?.AllKeys.Where(k => k != null))
                displayString.Append($"{key}: {nameValueCollection[key]} ");

            return displayString.ToString();
        }
    }
}
