using System.Xml.Serialization;

namespace CoreWCF.Server.REST.RestWrappers;

[XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class GetCatTypesResponseEnvelope
{
    [XmlElement("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public GetCatTypesResponseBody Body { get; set; } = new();
}

public class GetCatTypesResponseBody
{
    [XmlElement("GetCatTypesResponse", Namespace = "http://tempuri.org/")]
    public GetCatTypesResponse? Response { get; set; }
}