namespace RWS_LBE_Transaction.DTOs.Configurations
{
    public class RlpApiConfig
    {
        public string RetailerId { get; set; } = string.Empty;
        public RlpApiCoreConfig? Core { get; set; }
        public RlpApiOffersConfig? Offers { get; set; }
    }

    public class RlpApiCoreConfig
    {
        public string Host { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
    }

        public class RlpApiOffersConfig
    {
        public string Host { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
    }
}