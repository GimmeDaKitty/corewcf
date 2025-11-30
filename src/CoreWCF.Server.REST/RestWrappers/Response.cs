using System.Xml.Serialization;
using CoreWCF.Contracts;

// TODO - BEA - REVISIT GETPHOTORESPONSEWRAPPER BECAUSE THIS ONE (TAKEN FROM PERSOONDOMEIN) WORKS
namespace CoreWCF.Server.REST.RestWrappers;

[XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
[XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class Response
{
    [XmlElement(Order = 0)]
    public Header Header { get; set; }
    [XmlElement(Order = 1)]
    public ResponseBody Body { get; set; }
}

[XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class ResponseBody
{
    [XmlElement(ElementName = "CatFactResponse", Namespace = "http://tempuri.org/")]
    public CatFactResponse CatFactResponse { get; set; }
}

[XmlRoot(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class Header
{
}