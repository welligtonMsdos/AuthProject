using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth10Api.Domain.Entities;

public class OutboxMessage
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public required string Type { get; set; } 
    public required string Content { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; } 
    public string? Error { get; set; } 
}
