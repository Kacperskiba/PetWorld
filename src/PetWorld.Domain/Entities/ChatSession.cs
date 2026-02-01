namespace PetWorld.Domain.Entities;

public class ChatSession
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string UserQuestion { get; set; } = string.Empty;
    public string FinalResponse { get; set; } = string.Empty;
    public int IterationCount { get; set; }
    public bool IsCompleted { get; set; }

    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
