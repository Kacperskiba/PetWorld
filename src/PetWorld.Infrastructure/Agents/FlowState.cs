namespace PetWorld.Infrastructure.Agents;

public class FlowState
{
    public int IterationCount { get; set; }
    public int MaxIterations { get; set; } = 3;
    public bool IsApproved { get; set; }
    public string CurrentResponse { get; set; } = string.Empty;
    public string CriticFeedback { get; set; } = string.Empty;
    public List<int> RecommendedProductIds { get; set; } = new();
}
