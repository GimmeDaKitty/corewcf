using System.Xml.Serialization;

namespace CoreWCF.Server.REST.RestWrappers;

[XmlRoot("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class SoapBody<T>
{
    [XmlElement(Namespace = "http://tempuri.org/")]
    public T? Request { get; set; }
    
    [XmlElement(Namespace = "http://tempuri.org/")]
    public T? Response { get; set; }
}