using System;
using System.Runtime.Serialization;

namespace CoreWCF.Contracts
{
    [DataContract]
    public class CatLoverFault
    {
        [DataMember]
        public string? ErrorMessage { get; set; }
        
        [DataMember]
        public string? ErrorCode { get; set; }
        
        [DataMember]
        public DateTime ErrorTimestamp { get; set; }
    }
}

