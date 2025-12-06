
namespace CoreWCF.Server.REST.Services;

public static class SoapRequestOperationExtractor
{
    public static string GetSoapOperation(string soapRequest)
    {
        var xmlDoc = new System.Xml.XmlDocument();
        xmlDoc.LoadXml(soapRequest);

        var ns = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
        ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
        ns.AddNamespace("tem", "http://tempuri.org/");

        if (xmlDoc.SelectSingleNode("//tem:GetPhoto", ns) != null)
        {
            return "GetPhoto";
        }

        if (xmlDoc.SelectSingleNode("//tem:GetCatTypesRequest", ns) != null)
        {
            return "GetCatTypes";
        }

        // Other operations here.....

        throw new InvalidOperationException("Unknown SOAP operation");
    }
}