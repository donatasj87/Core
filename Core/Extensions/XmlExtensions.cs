using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Donatas.Core.Extensions
{
    public static class XmlExtensions
    {
        public static string? GetXmlElementName(this ICustomAttributeProvider property)
        {
            if (property == null)
                return null;
            return (property.GetCustomAttributes(typeof(XmlElementAttribute), true).FirstOrDefault() as XmlElementAttribute)?.ElementName;
        }

        public static XElement Normalize(this XElement element) =>
            new(
                element?.Name,
                element?.Attributes().OrderBy(a => a.Name.ToString()),
                element?.Nodes().Select(n => (n as XElement)?.Normalize() ?? n));

        public static string XmlToJson(this XDocument xDocument)
        {
            xDocument.Descendants().Where(_ => _.HasAttributes).ToList().ForEach(_ => _.RemoveAttributes()); // remove tableName="1" attributes
            xDocument.Descendants().Where(_ => !_.HasElements).ToList().ForEach(_ => _.Value = _.Value); // remove CDATA from values
            SortXmlByName(xDocument.Root);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xDocument.ToString());
            var json = JsonConvert.SerializeXmlNode(xmlDocument, Newtonsoft.Json.Formatting.Indented);
            return json;
        }

        private static void SortXmlByName(XContainer container)
        {
            var nodes = (from childEl in container.Elements()
                         where !childEl.Elements().Any()
                         orderby childEl.Name.LocalName
                         select childEl)
                .Union(
                    from childEl in container.Elements()
                    where childEl.Elements().Any()
                    orderby childEl.Name.LocalName
                    select childEl);

            if (nodes.Any())
                container.ReplaceNodes(nodes);

            container.Elements().ToList().ForEach(_ => SortXmlByName(_));
        }
    }
}
