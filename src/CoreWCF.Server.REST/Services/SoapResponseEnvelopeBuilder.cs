using System.Text;
using System.Xml;

namespace CoreWCF.Server.REST.Services;

public static class SoapResponseEnvelopeBuilder
{
    private const string SOAPNS = @"http://schemas.xmlsoap.org/soap/envelope/";
    private const string TEMPURINGNSPREFIX = @"tem";
    private const string CORENSPREFIX = @"cor";
    private const string TEMPURINS = @"http://tempuri.org/";
    private const string CORENS = @"http://schemas.datacontract.org/2004/07/CoreWCF.Contracts";

    public static string GetCatFactResponse(string catFact)
    {
        using var stream = new MemoryStream();

        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            OmitXmlDeclaration = false,
            Indent = true
        };

        using (var writer = XmlWriter.Create(stream, settings))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("soapenv", "Envelope", SOAPNS);
            writer.WriteStartElement("soapenv", "Header", SOAPNS);
            writer.WriteEndElement(); // Header

            writer.WriteStartElement("soapenv", "Body", SOAPNS);
            writer.WriteAttributeString("xmlns", TEMPURINGNSPREFIX, null, TEMPURINS);

            // <CatFactResponse xmlns="http://tempuri.org/">
            writer.WriteStartElement("CatFactResponse", TEMPURINS);
        
            // <CatFact>factstring</CatFact>
            writer.WriteStartElement("CatFact", TEMPURINS);
            writer.WriteString(catFact);
            writer.WriteEndElement(); // CatFact
        
            writer.WriteEndElement(); // CatFactResponse
            writer.WriteEndElement(); // Body
            writer.WriteEndElement(); // Envelope
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}