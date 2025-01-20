using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Donatas.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Return the byte array equivalent to the string
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <returns>Byte array</returns>
        public static byte[] GetBytes(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return new byte[0];
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static byte[]? GetBytes(this string str, Encoding enc) =>
            enc?.GetBytes(str);

        public static byte[] GetBytesFromHex(this string hex)
        {
            const int twoModulo = 2;
            const int hexBase = 16;
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % twoModulo == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, twoModulo), hexBase))
                             .ToArray();
        }

        public static string GetSha256Hash(this string str) =>
            new SHA256Managed().GetHash(str);

        private static string GetHash(this HashAlgorithm ha, string str) =>
            string.Join(string.Empty, ha.GetHashBytes(str).Select(_ => _.ToString("x2")));

        private static byte[] GetHashBytes(this HashAlgorithm ha, string str) =>
            ha.ComputeHash(str.GetBytes(Encoding.UTF8));

        public static string ConvertUrlsToHtmlLinks(this string msg, string target = "_blank") =>
            new Regex(@"((www\.|(http|https|ftp|news|file)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?_,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])", RegexOptions.IgnoreCase)
                .Replace(msg, "<a href=\"$1\" title=\"Click to open in a new window or tab\" target=\"" + target + "\">$1</a>")
                .Replace("href=\"www", "href=\"http://www");

        public static string Quote(this string inStr) =>
            $"\"{inStr}\"";

        public static IEnumerable<string> SplitInParts(this string s, int partLength)
        {
            ArgumentNullException.ThrowIfNull(s);

            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            return s.SplitInPart(partLength);
        }

        private static IEnumerable<string> SplitInPart(this string s, int partLength)
        {
            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        public static Stream ToStream(this string str)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static T ToEnumFromDescription<T>(this string description, T def)
        {
            if (string.IsNullOrWhiteSpace(description))
                return def;
            var fis = typeof(T).GetFields();

            foreach (var fi in fis)
            {
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0 && description.Equals(attributes[0].Description, StringComparison.InvariantCultureIgnoreCase))
                    return fi.Name.ToEnumFromName<T>();
            }
            return def;
        }

        public static T ToEnumFromName<T>(this string value, bool ignoreCase = true) =>
            (T)Enum.Parse(typeof(T), value, ignoreCase);

        public static bool EqualsIcic(this string str1, string str2)
        {
            if (str1 == null && str2 == null)
                return true;
            else if (str1 == null)
                return false;
            else
                return str1.Equals(str2, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string EncodeXml(this string str) =>
            str
                .Replace("&", "&amp;")
                .Replace("'", "&apos;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");

        public static string DecodeXml(this string str) =>
            str
                .Replace("&gt;", ">")
                .Replace("&lt;", "<")
                .Replace("&apos;", "'")
                .Replace("&amp;", "&");

        public static Type GetTypeFromStr(this string strFullyQualifiedName)
        {
            var type = Type.GetType(strFullyQualifiedName);

            if (type != null)
                return type;

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(strFullyQualifiedName);

                if (type != null)
                    return type;
            }
            throw new ArgumentException($"Object could not be created from type {strFullyQualifiedName}");
        }

        public static string UrlEncode(this string messageToEncode) =>
            HttpUtility.UrlEncode(messageToEncode);

        public static string UrlDecode(this string messageToDecode) =>
            HttpUtility.UrlDecode(messageToDecode);
    }
}
