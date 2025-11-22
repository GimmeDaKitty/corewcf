// using System.Xml.Serialization;
//
// namespace CoreWCF.Server.REST.RestWrappers;
//
// [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
// public class SoapEnvelope<T> where T : class
// {
//     [XmlElement("Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
//     public SoapHeader? Header { get; set; }
//
//     [XmlElement("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
//     public SoapRequestBody<T> RequestBody { get; set; } = new();
// }