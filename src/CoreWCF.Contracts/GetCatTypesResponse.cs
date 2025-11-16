namespace CoreWCF.Contracts;

[MessageContract]
public class GetCatTypesResponse
{
    [MessageHeader]
    public string ResponseId { get; set; }
    
    [MessageHeader]
    public int TotalCount { get; set; }
    
    [MessageBodyMember]
    public CatType[] CatTypes { get; set; }
}