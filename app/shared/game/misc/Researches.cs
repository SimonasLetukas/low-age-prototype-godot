using System.Collections.Generic;
using Godot;
using LowAgeData.Domain.Researches;

/// <summary>
/// <para>
/// Exposes current researched items for each player via <see cref="GlobalRegistry"/> or directly. 
/// </para>
/// <para>
/// Changes to researched items are raised as events through <see cref="EventBus"/>. Usual places to trigger changes
/// are: <see cref="ResearchNode"/> ability or <see cref="ModifyResearchNode"/> effect. 
/// </para>
/// </summary>
public partial class Researches : Node2D
{
    private readonly Dictionary<Player, HashSet<ResearchId>> _researchByPlayer = [];
    private readonly Dictionary<ResearchId, Research> _researchById = [];
    
    public override void _Ready()
    {
        base._Ready();

        Initialize();
        
        GlobalRegistry.Instance.ProvideGetResearchByPlayer(GetResearchByPlayer);
        GlobalRegistry.Instance.ProvideGetResearchById(GetResearchById);

        EventBus.Instance.ResearchGained += OnResearchGained;
    }

    public override void _ExitTree()
    {
        EventBus.Instance.ResearchGained -= OnResearchGained;
        
        base._ExitTree();
    }

    public HashSet<ResearchId> GetResearchByPlayer(Player player) => _researchByPlayer[player];
    
    public Research GetResearchById(ResearchId id) => _researchById[id];

    private void Initialize()
    {
        foreach (var player in Players.Instance.GetAll())
        {
            _researchByPlayer[player] = [];
        }

        foreach (var blueprint in Data.Instance.Blueprint.Researches)
        {
            _researchById[blueprint.Id] = blueprint;
        }
    }
    
    private void OnResearchGained(Player player, ResearchId researchId) 
        => _researchByPlayer[player].Add(researchId);
}