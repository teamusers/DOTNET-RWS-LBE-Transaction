using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.VMS.Shared
{
    public class InterfaceRequestHeaderDT
    {
        [JsonPropertyName("SystemID")]
        public int SystemID { get; set; }
    }

    public class InterfaceResponseHeaderDT
    {
        [JsonPropertyName("FaultCodeID")]
        public long FaultCodeID { get; set; }

        [JsonPropertyName("FaultCodeDescription")]
        public string FaultCodeDescription { get; set; } = "";
    }

    public class VmsErrorResponse
    {
        [JsonPropertyName("InterfaceResponseHeaderDT")]
        public InterfaceResponseHeaderDT? InterfaceResponseHeaderDT { get; set; }
    }
}