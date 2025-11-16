using System.Xml;
using System.Xml.Serialization;

namespace CoreWCF.Client.REST;

public static class SoapResponseBuilder
{
    public static async Task<TResponse> GetResponseAsync<TResponse>(string nodeName, HttpResponseMessage response)
    {
        var responseXml = await response.Content.ReadAsStringAsync();
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(responseXml);

        var ns = new XmlNamespaceManager(xmlDoc.NameTable);
        ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
        ns.AddNamespace("tem", "http://tempuri.org/");

        var node = xmlDoc.SelectSingleNode($"//tem:{nodeName}", ns);

        if (node == null)
        {
            throw new InvalidOperationException($"Node {nodeName} not found in response");
        }

        // For byte[], convert from base64
        if (typeof(TResponse) == typeof(byte[]))
        {
            if (string.IsNullOrWhiteSpace(node.InnerText))
            {
                return (TResponse)(object)Array.Empty<byte>();
            }
            return (TResponse)(object)Convert.FromBase64String(node.InnerText);
        }

        // For other types, use XmlSerializer
        using var stringReader = new StringReader(node.OuterXml);
        using var xmlReader = XmlReader.Create(stringReader);
        return (TResponse?)new XmlSerializer(typeof(TResponse)).Deserialize(xmlReader)
               ?? throw new InvalidOperationException($"Resultaat van type {typeof(TResponse).Name} niet gevonden");
    }
}