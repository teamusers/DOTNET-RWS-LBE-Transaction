using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Responses
{
    public sealed class UserPointResponse
    {
        [JsonPropertyName("status")] public string Status { get; init; } = default!;
        [JsonPropertyName("user")]   public RlpUser? User { get; init; }
    }

    public sealed class RlpUser
    {
        [JsonPropertyName("id")]                         public string Id { get; init; } = default!;
        [JsonPropertyName("external_id")]                public string ExternalId { get; init; } = default!;
        [JsonPropertyName("opted_in")]                   public bool OptedIn { get; init; }
        [JsonPropertyName("activated")]                  public bool Activated { get; init; }
        [JsonPropertyName("proxy_ids")]                  public List<string> ProxyIds { get; init; } = new();
        [JsonPropertyName("identifiers")]                public List<UserIdentifier> Identifiers { get; init; } = new();
        [JsonPropertyName("available_points")]           public double AvailablePoints { get; init; }
        [JsonPropertyName("test_points")]                public double TestPoints { get; init; }
        [JsonPropertyName("unclaimed_achievement_count")]public int UnclaimedAchievementCount { get; init; }
        [JsonPropertyName("email")]                      public string? Email { get; init; }
        [JsonPropertyName("gender")]                     public string? Gender { get; init; }
        [JsonPropertyName("dob")]                        public string? DateOfBirth { get; init; }
        [JsonPropertyName("created_at")]                 public string? CreatedAt { get; init; }
        [JsonPropertyName("updated_at")]                 public string? UpdatedAt { get; init; }
        [JsonPropertyName("address")]                    public string? Address { get; init; }
        [JsonPropertyName("address2")]                   public string? Address2 { get; init; }
        [JsonPropertyName("city")]                       public string? City { get; init; }
        [JsonPropertyName("zip")]                        public string? Zip { get; init; }
        [JsonPropertyName("state")]                      public string? State { get; init; }
        [JsonPropertyName("country")]                    public string? Country { get; init; }
        [JsonPropertyName("suspended")]                  public bool Suspended { get; init; }
        [JsonPropertyName("last_name")]                  public string? LastName { get; init; }
        [JsonPropertyName("first_name")]                 public string? FirstName { get; init; }
        [JsonPropertyName("registered_at")]              public string? RegisteredAt { get; init; }
        [JsonPropertyName("profile_photo_url")]          public string? ProfilePhotoUrl { get; init; }
        [JsonPropertyName("account_status")]             public string? AccountStatus { get; init; }
        [JsonPropertyName("tier")]                       public string? Tier { get; init; }
        [JsonPropertyName("tier_system")]                public string? TierSystem { get; init; }
        [JsonPropertyName("tier_points")]                public double TierPoints { get; init; }
        [JsonPropertyName("tier_entered_at")]            public string? TierEnteredAt { get; init; }
        [JsonPropertyName("tier_resets_at")]             public string? TierResetsAt { get; init; }

        [JsonPropertyName("tier_details")]               public TierDetails? TierDetails { get; init; }
        [JsonPropertyName("referrer_code")]              public string? ReferrerCode { get; init; }
        [JsonPropertyName("user_profile")]               public UserProfile? UserProfile { get; init; }
        [JsonPropertyName("phone_numbers")]              public List<PhoneNumber>? PhoneNumbers { get; init; }
    }

    public sealed class UserIdentifier
    {
        [JsonPropertyName("external_id")]      public string ExternalId { get; init; } = default!;
        [JsonPropertyName("external_id_type")] public string ExternalIdType { get; init; } = default!;
    }

    public sealed class TierDetails
    {
        [JsonPropertyName("tier_levels")]             public List<TierLevel> TierLevels { get; init; } = new();
        [JsonPropertyName("point_account_balances")]  public PointAccountBalances? PointAccountBalances { get; init; }
    }

    public sealed class TierLevel
    {
        [JsonPropertyName("tier_system_id")] public string TierSystemId { get; init; } = default!;
        [JsonPropertyName("tier_level_id")]  public string TierLevelId { get; init; } = default!;
        [JsonPropertyName("user_id")]        public string UserId { get; init; } = default!;
        [JsonPropertyName("join_date")]      public string JoinDate { get; init; } = default!;
        [JsonPropertyName("tier_overview")]  public TierOverview? TierOverview { get; init; }
        [JsonPropertyName("tier_progress")]  public List<object> TierProgress { get; init; } = new(); // empty array for now
    }

    public sealed class TierOverview
    {
        [JsonPropertyName("id")]              public string Id { get; init; } = default!;
        [JsonPropertyName("tier_system_id")] public string TierSystemId { get; init; } = default!;
        [JsonPropertyName("retailer_id")]    public string RetailerId { get; init; } = default!;
        [JsonPropertyName("name")]           public string Name { get; init; } = default!;
        [JsonPropertyName("rank")]           public int Rank { get; init; }
        [JsonPropertyName("status")]         public int Status { get; init; }
    }

    public sealed class PointAccountBalances
    {
        [JsonPropertyName("retailer_id")] public string RetailerId { get; init; } = default!;
        [JsonPropertyName("user_id")]     public string UserId { get; init; } = default!;
        [JsonPropertyName("summary")]     public PointBalanceSummary? Summary { get; init; }
        [JsonPropertyName("details")]     public List<PointAccountDetail> Details { get; init; } = new();
    }

    public sealed class PointBalanceSummary
    {
        [JsonPropertyName("total_points")]     public double TotalPoints { get; init; }
        [JsonPropertyName("life_time_points")] public double LifeTimePoints { get; init; }
    }

    public sealed class PointAccountDetail
    {
        [JsonPropertyName("account_name")]          public string AccountName { get; init; } = default!;
        [JsonPropertyName("user_point_account_id")] public string UserPointAccountId { get; init; } = default!;
        [JsonPropertyName("point_account_id")]      public string PointAccountId { get; init; } = default!;
        [JsonPropertyName("grouping_label")]        public string GroupingLabel { get; init; } = default!;
        [JsonPropertyName("available_balance")]     public double AvailableBalance { get; init; }
        [JsonPropertyName("life_time_value")]       public double LifeTimeValue { get; init; }
    }

    public sealed class UserProfile
    {
        [JsonPropertyName("_version")]              public int Version { get; init; }
        [JsonPropertyName("country_code")]          public string CountryCode { get; init; } = default!;
        [JsonPropertyName("country_name")]          public string CountryName { get; init; } = default!;
        [JsonPropertyName("market_pref_email")]     public bool MarketPrefEmail { get; init; }
        [JsonPropertyName("market_pref_push")]      public bool MarketPrefPush { get; init; }
        [JsonPropertyName("active_status")]         public string ActiveStatus { get; init; } = default!;
        [JsonPropertyName("language_preference")]   public string LanguagePreference { get; init; } = default!;
        [JsonPropertyName("burn_pin")]              public string BurnPin { get; init; } = default!;
        [JsonPropertyName("car_detail")]            public List<CarDetail> CarDetail { get; init; } = new();
    }

    public sealed class CarDetail
    {
        [JsonPropertyName("car_plate")]     public string CarPlate { get; init; } = default!;
        [JsonPropertyName("iu_number")]     public string IuNumber { get; init; } = default!;
        [JsonPropertyName("is_sg")]         public bool IsSG { get; init; }
        [JsonPropertyName("last_updated")]  public string LastUpdated { get; init; } = default!;
    }

    public sealed class PhoneNumber
    {
        [JsonPropertyName("phone_number")]       public string Phone { get; init; } = default!;
        [JsonPropertyName("phone_type")]         public string PhoneType { get; init; } = default!;
        [JsonPropertyName("preference_flags")]   public List<string> PreferenceFlags { get; init; } = new();
        [JsonPropertyName("verified_ownership")] public bool VerifiedOwnership { get; init; }
    }
}
