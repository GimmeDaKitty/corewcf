using System.Xml.Serialization;

namespace CoreWCF.Server.REST.RestWrappers;

// [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
// public class GetPhotoResponseEnvelope
// {
//     [XmlElement("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
//     public GetCatTypesResponseBody Body { get; set; } = new();
// }
//
// public class GetPhotoResponseBody
// {
//     [XmlElement("GetPhotoResponse", Namespace = "http://tempuri.org/")]
//     public GetPhotoResponse GetPhotoResponse { get; set; } = new();
// }
//
// public class GetPhotoResponse
// {
//     [XmlElement("GetPhotoResult", Namespace = "http://tempuri.org/")]
//     public byte[] GetPhotoResult { get; set; }
// }

/// ORIGINAL, WORKING WITH
/// return SoapResponseBuilder.GetSOAPResponse(response);

// [XmlRoot("GetPhotoResponse", Namespace = "http://tempuri.org/")]
// public class GetPhotoResponse
// {
//     [XmlElement("GetPhotoResult", Namespace = "http://tempuri.org/")]
//     public byte[] GetPhotoResult { get; set; }
// }