using System.Xml.Serialization;

namespace CoreWCF.Server.REST.RestWrappers;

[XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class GetCatTypesRequestEnvelope
{
    [XmlElement("Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public SoapHeader? Header { get; set; }

    [XmlElement("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public GetCatTypesRequestBody Body { get; set; } = new();
}

public class GetCatTypesRequestBody
{
    [XmlElement("GetCatTypesRequest", Namespace = "http://tempuri.org/")]
    public GetCatTypesRequest? Request { get; set; }
}