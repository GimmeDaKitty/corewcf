using System.Xml.Serialization;
using CoreWCF.Server.REST.RestWrappers;

namespace CoreWCF.Server.REST.Services;

public static class ResponseXmlSerializer
{
    public static async Task<IResult> Serialize<T>(T response) where T : class
    {
        var xmlSerializer = new XmlSerializer(typeof(T));
        await using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, response);
        
        return Results.Content(stringWriter.ToString(), "text/xml; charset=utf-8");
    }
}