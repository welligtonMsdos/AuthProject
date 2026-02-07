using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth10Api.Domain.Entities;

public class OutboxMessage
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public string Type { get; set; } // Tipo do evento (ex: "UserCreated")
    public string Content { get; set; } // O DTO serializado em JSON
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; } // Nulo se ainda não foi enviado
    public string? Error { get; set; } // Para registrar falhas se houver
}
