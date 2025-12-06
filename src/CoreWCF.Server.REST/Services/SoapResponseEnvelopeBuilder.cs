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

            // <Fact>factstring</Fact>
            writer.WriteStartElement("Fact", TEMPURINS);
            writer.WriteString(catFact);
            writer.WriteEndElement(); // Fact

            writer.WriteEndElement(); // CatFactResponse
            writer.WriteEndElement(); // Body
            writer.WriteEndElement(); // Envelope
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

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

    public static string GetCatTypesResponse(GetCatTypesResponse catTypesResponse)
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
            writer.WriteStartElement("s", "Envelope", SOAPNS);

            // Header
            writer.WriteStartElement("s", "Header", SOAPNS);

            writer.WriteStartElement("h", "ResponseId", TEMPURINS);
            writer.WriteString(catTypesResponse.ResponseId);
            writer.WriteEndElement(); // ResponseId

            writer.WriteStartElement("h", "TotalCount", TEMPURINS);
            writer.WriteString(catTypesResponse.TotalCount.ToString());
            writer.WriteEndElement(); // TotalCount

            writer.WriteStartElement("CatLoverHeaderResponse");
            writer.WriteString("Thank you for being a cat lover!");
            writer.WriteEndElement(); // CatLoverHeaderResponse

            writer.WriteEndElement(); // Header

            // Body
            writer.WriteStartElement("s", "Body", SOAPNS);

            writer.WriteStartElement("GetCatTypesResponse", TEMPURINS);

            writer.WriteStartElement("CatTypes", TEMPURINS);
            writer.WriteAttributeString("xmlns", "a", null, CORENS);
            writer.WriteAttributeString("xmlns", "i", null, "http://www.w3.org/2001/XMLSchema-instance");

            foreach (var catType in catTypesResponse.CatTypes)
            {
                writer.WriteStartElement("a", "CatType", CORENS);

                writer.WriteStartElement("a", "LikesChildren", CORENS);
                writer.WriteString(catType.LikesChildren.ToString().ToLower());
                writer.WriteEndElement(); // LikesChildren

                writer.WriteStartElement("a", "RaceName", CORENS);
                writer.WriteString(catType.RaceName);
                writer.WriteEndElement(); // RaceName

                writer.WriteEndElement(); // CatType
            }

            writer.WriteEndElement(); // CatTypes
            writer.WriteEndElement(); // GetCatTypesResponse
            writer.WriteEndElement(); // Body
            writer.WriteEndElement(); // Envelope
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}