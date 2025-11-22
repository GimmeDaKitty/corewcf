using System.Runtime.Serialization;
using System.Xml;

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

    if (typeof(TResponse) == typeof(byte[]))
    {
        if (string.IsNullOrWhiteSpace(node.InnerText))
        {
            return (TResponse)(object)Array.Empty<byte>();
        }
        return (TResponse)(object)Convert.FromBase64String(node.InnerText);
    }

    if (typeof(TResponse) == typeof(GetCatTypesResponse))
    {
        var catTypesNode = node.SelectSingleNode("tem:CatTypes", ns);
        if (catTypesNode == null)
        {
            throw new InvalidOperationException("CatTypes node not found");
        }

        // Try to detect which namespace is used for CatType elements
        var firstCatType = catTypesNode.SelectSingleNode("dc:CatType", ns) 
                          ?? catTypesNode.SelectSingleNode("tem:CatType", ns);

        List<Contracts.CatType> catTypes = new();
        
        if (firstCatType?.NamespaceURI == "http://schemas.datacontract.org/2004/07/CoreWCF.Contracts")
        {
            // Server.CoreWCF response - use DataContractSerializer
            using var stringReader = new StringReader(catTypesNode.OuterXml);
            using var xmlReader = XmlReader.Create(stringReader);
            xmlReader.ReadToDescendant("CatTypes", "http://tempuri.org/");
            
            var serializer = new DataContractSerializer(typeof(Contracts.CatType[]), "CatTypes", "http://tempuri.org/");
            catTypes.AddRange((Contracts.CatType[])serializer.ReadObject(xmlReader));
        }
        else
        {
            // Server.REST response - manually parse from tempuri namespace
            foreach (XmlNode catTypeNode in catTypesNode.ChildNodes)
            {
                if (catTypeNode.LocalName != "CatType") continue;
                
                var raceName = catTypeNode.SelectSingleNode("tem:RaceName", ns)?.InnerText;
                var likesChildren = bool.Parse(catTypeNode.SelectSingleNode("tem:LikesChildren", ns)?.InnerText ?? "false");
                
                catTypes.Add(new Contracts.CatType
                {
                    RaceName = raceName,
                    LikesChildren = likesChildren
                });
            }
        }

        return (TResponse)(object)new GetCatTypesResponse
        {
            CatTypes = catTypes.ToArray()
        };
    }

    throw new NotSupportedException($"Type {typeof(TResponse).Name} not supported");
    }
}