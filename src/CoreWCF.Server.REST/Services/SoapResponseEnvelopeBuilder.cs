using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CoreWCF.Server.REST.Services;

public static class SoapResponseEnvelopeBuilder
{
    private const string SOAPNS = @"http://schemas.xmlsoap.org/soap/envelope/";
    private const string TEMPURINGNSPREFIX = @"tem";
    private const string CORENSPREFIX = @"cor";
    private const string TEMPURINS = @"http://tempuri.org/";
    private const string CORENS = @"http://schemas.datacontract.org/2004/07/CoreWCF.Contracts";

    public static string GetSOAPPhotoResponse(byte[] photo)
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

            // <GetPhotoResponse xmlns="http://tempuri.org/">
            writer.WriteStartElement("GetPhotoResponse", TEMPURINS);
        
            // <GetPhotoResult>base64data</GetPhotoResult>
            writer.WriteStartElement("GetPhotoResult", TEMPURINS);
            writer.WriteString(Convert.ToBase64String(photo));
            writer.WriteEndElement(); // GetPhotoResult
        
            writer.WriteEndElement(); // GetPhotoResponse
            writer.WriteEndElement(); // Body
            writer.WriteEndElement(); // Envelope
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
    /*
    <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
        <s:Header>
            <CatLoverHeaderResponse>Thank you for being a cat lover!</CatLoverHeaderResponse>
        </s:Header>
        <s:Body>
            <BellyRubResponse xmlns="http://tempuri.org/">
                <Allowed>true</Allowed>
            </BellyRubResponse>
        </s:Body>
    </s:Envelope>
    */
    public static string GetSOAPBellyRubResponse(bool allowed)
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

            // <AllowBellyRubResponse xmlns="http://tempuri.org/">
            writer.WriteStartElement("AllowBellyRubResponse", TEMPURINS);
        
            // <AllowBellyRubResult>bool</AllowBellyRubResult>
            writer.WriteStartElement("AllowBellyRubResult", TEMPURINS);
            writer.WriteString(allowed.ToString().ToLowerInvariant());
            writer.WriteEndElement(); // AllowBellyRubResult
        
            writer.WriteEndElement(); // AllowBellyRubResponse
            writer.WriteEndElement(); // Body
            writer.WriteEndElement(); // Envelope
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public static string GetSOAPResponse<TResponse>(TResponse response)
    {
        using var stream = new MemoryStream();

        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            OmitXmlDeclaration = true,
            Indent = true
        };

        using (var writer = XmlWriter.Create(stream, settings))
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