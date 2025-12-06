using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CoreWCF.Server.REST.Services;

public static class GenericSerializer
{
    public static async Task<string> Serialize<T>(T response)
    {
        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            OmitXmlDeclaration = true,
            Indent = true
        };
        
        var xmlSerializer = new XmlSerializer(typeof(T));
        using var stringWriter = new StringWriter();
        using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
        {
            xmlSerializer.Serialize(xmlWriter, response);
        }
        
        return stringWriter.ToString();
    }
}