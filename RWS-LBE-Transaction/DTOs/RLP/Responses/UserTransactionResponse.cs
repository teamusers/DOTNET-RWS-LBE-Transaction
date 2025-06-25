using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Responses
{

    public sealed class UserTransactionResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; init; }

        [JsonPropertyName("result")]
        public List<Result>? Result { get; init; }

        [JsonPropertyName("grouping_field")]
        public string? GroupingField { get; init; }

        [JsonPropertyName("count")]
        public int? Count { get; init; }

        [JsonPropertyName("event_types")]
        public List<EventType>? EventTypes { get; init; }
    }

    public sealed class Result
    {
        [JsonPropertyName("event_stream_stream_id")]
        public Guid EventStreamStreamId { get; init; }

        [JsonPropertyName("event_stream_event_category_id")]
        public int EventCategoryId { get; init; }

        [JsonPropertyName("event_stream_event_type_id")]
        public int EventTypeId { get; init; }

        [JsonPropertyName("target_id")]
        public int TargetId { get; init; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; init; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; init; }

        [JsonPropertyName("event_stream_payload")]
        public JsonElement EventStreamPayload { get; init; }

        [JsonPropertyName("contexts")]
        public List<JsonElement>? Contexts { get; init; }
    }

    public sealed class EventType
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }

        [JsonPropertyName("name")]
        public string? Name { get; init; }

        [JsonPropertyName("slug")]
        public string? Slug { get; init; }

        [JsonPropertyName("event_stream_event_category_id")]
        public int EventCategoryId { get; init; }

        [JsonPropertyName("event_stream_stream_id")]
        public int StreamId { get; init; }
    }
}
