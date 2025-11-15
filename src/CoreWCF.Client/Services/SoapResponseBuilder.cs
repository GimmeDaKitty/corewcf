using System.Xml;
using CoreWCF.Client.Data;

namespace CoreWCF.Client.Services;

public static class SoapResponseBuilder
{
    public static async Task<TResponse> GetResponseAsync<TResponse>(string nodeName, HttpResponseMessage response)
    {
        var responseXml = await response.Content.ReadAsStringAsync();
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(responseXml);

        var ns = new XmlNamespaceManager(xmlDoc.NameTable);
        ns.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
        ns.AddNamespace("tem", "http://tempuri.org/");
        ns.AddNamespace("dc", "http://schemas.datacontract.org/2004/07/CoreWCF.Contracts");

        var node = xmlDoc.SelectSingleNode($"//soap:Body//tem:*[local-name()='{nodeName}']", ns);
        if (node == null)
        {
            throw new InvalidOperationException($"Node {nodeName} not found in response");
        }
        
        if (typeof(TResponse) == typeof(CatFactResponse))
        {
            return DeserializeCatFactResponse<TResponse>(node, ns);
        }

        throw new NotSupportedException($"Type {typeof(TResponse).Name} not supported");
    }
    
    private static TResponse DeserializeCatFactResponse<TResponse>(XmlNode node, XmlNamespaceManager ns)
    {
        var factNode = node.SelectSingleNode("tem:CatFact", ns);
        if (factNode == null)
        {
            throw new InvalidOperationException("CatFact node not found");
        }

        var response = new CatFactResponse(factNode.InnerText);
        return (TResponse)(object)response;    
    }
}