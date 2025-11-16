using System.Xml.Serialization;

namespace CoreWCF.Server.REST;

// Needed to mimick the response wrapping of CoreWCF
[XmlRoot("GetPhotoResponse", Namespace = "http://tempuri.org/")]
public class GetPhotoResponse
{
   [XmlElement("GetPhotoResult")]
   public byte[] GetPhotoResult { get; set; }
}