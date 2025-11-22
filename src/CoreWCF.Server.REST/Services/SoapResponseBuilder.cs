using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CoreWCF.Server.REST.Services;

public static class SoapResponseBuilder
{
    private const string SOAPNS = @"http://schemas.xmlsoap.org/soap/envelope/";
    private const string TEMPURINGNSPREFIX = @"tem";
    private const string CORENSPREFIX = @"cor";
    private const string TEMPURINS = @"http://tempuri.org/";
    private const string CORENS = @"http://schemas.datacontract.org/2004/07/CoreWCF.Contracts";
    
    public static string GetSOAPResponse<TResponse>(TResponse response)
    {
        using var stream = new MemoryStream();
        
        Encoding utf8 = new UTF8Encoding(false); // omit BOM
        using (var writer = new XmlTextWriter(stream, utf8))
        {
            // <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
            writer.WriteStartDocument();
            writer.WriteStartElement("soapenv", "Envelope", SOAPNS);
            writer.WriteStartElement("soapenv", "Header", SOAPNS); // SOAP Header
            writer.WriteEndElement(); // Header
        
            //<s:Body xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:tem="http://tempuri.org/" xmlns:cor="http://schemas.datacontract.org/2004/07/CoreWCF.Contracts">
            writer.WriteStartElement("soapenv", "Body", SOAPNS);
            writer.WriteAttributeString("xmlns", TEMPURINGNSPREFIX, null, TEMPURINS);
            writer.WriteAttributeString("xmlns", CORENSPREFIX, null, CORENS);
                
            new XmlSerializer(typeof(TResponse)).Serialize(writer, response);
            writer.WriteEndElement(); // Body
            writer.WriteEndElement(); // Envelope
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}