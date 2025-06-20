using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Responses
{
    public class UserPointResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("user")]
        public User? User { get; set; }
    }

    public class User
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("external_id")]
        public string? ExternalId { get; set; }

        [JsonPropertyName("opted_in")]
        public bool OptedIn { get; set; }

        [JsonPropertyName("activated")]
        public bool Activated { get; set; }

        [JsonPropertyName("proxy_ids")]
        public List<string>? ProxyIds { get; set; }

        [JsonPropertyName("identifiers")]
        public List<Identifier>? Identifiers { get; set; }

        [JsonPropertyName("available_points")]
        public decimal AvailablePoints { get; set; }

        [JsonPropertyName("test_points")]
        public decimal TestPoints { get; set; }

        [JsonPropertyName("unclaimed_achievement_count")]
        public int UnclaimedAchievementCount { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("dob")]
        public string? DOB { get; set; }

        [JsonPropertyName("created_at")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string? UpdatedAt { get; set; }

        [JsonPropertyName("suspended")]
        public bool Suspended { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("registered_at")]
        public string? RegisteredAt { get; set; }

        [JsonPropertyName("profile_photo_url")]
        public string? ProfilePhotoUrl { get; set; }

        [JsonPropertyName("test_account")]
        public bool TestAccount { get; set; }

        [JsonPropertyName("account_status")]
        public string? AccountStatus { get; set; }

        [JsonPropertyName("tier")]
        public string? Tier { get; set; }

        [JsonPropertyName("tier_system")]
        public string? TierSystem { get; set; }

        [JsonPropertyName("tier_points")]
        public decimal TierPoints { get; set; }

        [JsonPropertyName("next_tier_points")]
        public decimal NextTierPoints { get; set; }

        [JsonPropertyName("tier_ends_value")]
        public decimal TierEndsValue { get; set; }

        [JsonPropertyName("tier_entered_at")]
        public string? TierEnteredAt { get; set; }

        [JsonPropertyName("tier_resets_at")]
        public string? TierResetsAt { get; set; }

        [JsonPropertyName("tier_details")]
        public TierDetails? TierDetails { get; set; }

        [JsonPropertyName("referrer_code")]
        public string? ReferrerCode { get; set; }

        [JsonPropertyName("user_profile")]
        public UserProfile? UserProfile { get; set; }

        [JsonPropertyName("phone_numbers")]
        public List<PhoneNumber>? PhoneNumbers { get; set; }
    }

    public class Identifier
    {
        [JsonPropertyName("external_id")]
        public string? ExternalId { get; set; }

        [JsonPropertyName("external_id_type")]
        public string? ExternalIdType { get; set; }
    }

    public class PhoneNumber
    {
        [JsonPropertyName("phone_number")]
        public string? Number { get; set; }

        [JsonPropertyName("phone_type")]
        public string? PhoneType { get; set; }

        [JsonPropertyName("preference_flags")]
        public List<string>? PreferenceFlags { get; set; }

        [JsonPropertyName("verified_ownership")]
        public bool VerifiedOwnership { get; set; }
    }

    public class TierDetails
    {
        [JsonPropertyName("tier_levels")]
        public List<TierLevel>? TierLevels { get; set; }

        [JsonPropertyName("point_account_balances")]
        public PointAccountBalances? PointAccountBalances { get; set; }
    }

    public class TierLevel
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("tier_system_id")]
        public string? TierSystemId { get; set; }

        [JsonPropertyName("tier_level_id")]
        public string? TierLevelId { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("join_date")]
        public string? JoinDate { get; set; }

        [JsonPropertyName("next_maintenance_eval_date")]
        public string? NextMaintenanceEvalDate { get; set; }

        [JsonPropertyName("tier_overview")]
        public TierOverview? TierOverview { get; set; }

        [JsonPropertyName("next_tier_overview")]
        public TierOverview? NextTierOverview { get; set; }

        [JsonPropertyName("tier_progress")]
        public List<TierProgress>? TierProgress { get; set; }
    }

    public class TierOverview
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("tier_system_id")]
        public string? TierSystemId { get; set; }

        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("rank")]
        public int Rank { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }
    }

    public class TierProgress
    {
        [JsonPropertyName("rule_tree_id")]
        public string? RuleTreeId { get; set; }

        [JsonPropertyName("rules")]
        public List<RuleProgress>? Rules { get; set; }
    }

    public class RuleProgress
    {
        [JsonPropertyName("query_result")]
        public decimal? QueryResult { get; set; }

        [JsonPropertyName("rule_id")]
        public string? RuleId { get; set; }

        [JsonPropertyName("parent_id")]
        public string? ParentId { get; set; }

        [JsonPropertyName("tree_id")]
        public string? TreeId { get; set; }

        [JsonPropertyName("rule_passed")]
        public bool RulePassed { get; set; }

        [JsonPropertyName("rule")]
        public Rule? Rule { get; set; }

        [JsonPropertyName("discriminator")]
        public int Discriminator { get; set; }

        [JsonPropertyName("rule_tree_name")]
        public string? RuleTreeName { get; set; }
    }

    public class Rule
    {
        [JsonPropertyName("discriminator")]
        public int Discriminator { get; set; }

        [JsonPropertyName("target_balance")]
        public decimal? TargetBalance { get; set; }

        [JsonPropertyName("comparison")]
        public int? Comparison { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }

        [JsonPropertyName("parent_id")]
        public string? ParentId { get; set; }

        [JsonPropertyName("rank")]
        public int Rank { get; set; }

        [JsonPropertyName("is_new")]
        public bool IsNew { get; set; }

        [JsonPropertyName("constraints")]
        public List<Constraint>? Constraints { get; set; }
    }

    public class Constraint
    {
        [JsonPropertyName("discriminator")]
        public int Discriminator { get; set; }

        [JsonPropertyName("point_accounts")]
        public Dictionary<string, string>? PointAccounts { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("rule_id")]
        public string? RuleId { get; set; }

        [JsonPropertyName("is_new")]
        public bool IsNew { get; set; }

        [JsonPropertyName("rank")]
        public int Rank { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }
    }

    public class PointAccountBalances
    {
        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("summary")]
        public PointsSummary? Summary { get; set; }

        [JsonPropertyName("details")]
        public List<PointAccountDetail>? Details { get; set; }
    }

    public class PointsSummary
    {
        [JsonPropertyName("total_points")]
        public decimal TotalPoints { get; set; }

        [JsonPropertyName("life_time_points")]
        public decimal LifeTimePoints { get; set; }
    }

    public class PointAccountDetail
    {
        [JsonPropertyName("account_name")]
        public string? AccountName { get; set; }

        [JsonPropertyName("user_point_account_id")]
        public string? UserPointAccountId { get; set; }

        [JsonPropertyName("point_account_id")]
        public string? PointAccountId { get; set; }

        [JsonPropertyName("grouping_label")]
        public string? GroupingLabel { get; set; }

        [JsonPropertyName("available_balance")]
        public decimal AvailableBalance { get; set; }

        [JsonPropertyName("life_time_value")]
        public decimal LifeTimeValue { get; set; }
    }

    public class UserProfile
    {
        [JsonPropertyName("_version")]
        public int Version { get; set; }

        [JsonPropertyName("country_code")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("country_name")]
        public string? CountryName { get; set; }

        [JsonPropertyName("market_pref_email")]
        public bool MarketPrefEmail { get; set; }

        [JsonPropertyName("market_pref_sms")]
        public bool MarketPrefSms { get; set; }

        [JsonPropertyName("market_pref_push")]
        public bool MarketPrefPush { get; set; }

        [JsonPropertyName("previous_email")]
        public string? PreviousEmail { get; set; }

        [JsonPropertyName("active_status")]
        public string? ActiveStatus { get; set; }

        [JsonPropertyName("language_preference")]
        public string? LanguagePreference { get; set; }

        [JsonPropertyName("car_detail")]
        public List<object>? CarDetail { get; set; }
    }
} 