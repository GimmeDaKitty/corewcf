using System;

namespace CoreWCF.Contracts;

[MessageContract]
public class GetCatTypesRequest
{
    [MessageHeader]
    public string? CatLoverHeader { get; set; }
    
    [MessageBodyMember]
    public string RequestId { get; set; }

    [MessageBodyMember] 
    public DateTime RequestTimestamp { get; set; } = DateTime.UtcNow;
    
    [MessageBodyMember]
    public bool LikesChildren { get; set; }
}