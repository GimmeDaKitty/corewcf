using System.Text;

namespace CoreWCF.Server.REST.Services;

public static class SoapRequestOperationExtractor
{
    public static async Task<string> GetSoapOperation(HttpContext httpContext)
    {
        httpContext.Request.EnableBuffering();
        using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
        var soapRequest = await reader.ReadToEndAsync();
        httpContext.Request.Body.Position = 0;
        
        return GetSoapOperation(soapRequest);
    }
    
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
        
        if (xmlDoc.SelectSingleNode("//tem:AllowBellyRub", ns) != null)
        {
            return "AllowBellyRub";
        }

        throw new InvalidOperationException("Unknown SOAP operation");
    }
}