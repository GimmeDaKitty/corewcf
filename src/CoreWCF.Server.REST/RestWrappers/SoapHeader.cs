using System.Xml.Serialization;

namespace CoreWCF.Server.REST.RestWrappers;

public class SoapHeader
{
    [XmlElement("CatLoverHeader", Namespace = "http://tempuri.org/")]
    public string? CatLoverHeader { get; set; }
}