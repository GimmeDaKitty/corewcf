namespace CoreWCF.Contracts;

[MessageContract]
public class CatFactResponse
{
    [MessageBodyMember]
    public string Fact { get; set; }
}