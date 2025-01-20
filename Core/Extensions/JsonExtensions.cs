using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml;

namespace Donatas.Core.Extensions
{
    public static class JsonExtensions
    {
        public static XmlDocument? JsonToXml(this JObject json) =>
            JsonConvert.DeserializeXmlNode(json.ToString());
    }
}
