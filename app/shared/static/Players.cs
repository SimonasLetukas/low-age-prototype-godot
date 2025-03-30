using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Factions;
using MultipurposePathfinding;

/// <summary>
/// Handles player data. Should mostly act as a read-only class, to change state <see cref="IGameEvent"/>
/// should be used so that the appropriate changes are distributed to all multiplayer peers.
/// </summary>
public partial class Players : Node
{
    public static Players Instance = null!;
    public override void _Ready()
    {
        base._Ready();
        
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        Instance ??= this;
    }
    
    public event Action<int> PlayerAdded = delegate { }; 
    public event Action<int> PlayerRemoved = delegate { };
    
    private List<Player> _players = [];
    
    public int Count => _players.Count;
    public bool AllReady => _players.All(x => x.Ready);
    public Player Current => Get(Multiplayer.GetUniqueId());

    public IList<Player> GetAll() => _players;
    
    public IList<int> GetAllIds() => _players.Select(p => p.Id).ToList();
    
    public Player Get(int id) => _players.Single(x => x.Id.Equals(id));
    
    public Player? TryGet(int id) => _players.FirstOrDefault(x => x.Id.Equals(id));
    
    public string GetName(int id) => _players.Single(x => x.Id.Equals(id)).Name;

    public bool IsCurrentPlayerEnemyTo(Player player) => Current.Team.IsEnemyTo(player.Team);
    
    public bool IsCurrentPlayerAllyTo(Player player) => Current.Team.IsAllyTo(player.Team);

    public bool IsActionAllowedForCurrentPlayerOn(EntityNode? entity) 
        => entity is null || entity.Player.Id.Equals(Current.Id);
    
    public Team GetNextAvailableTeam() => new(_players.Count + 1);
    
    public Team GetMaxAvailableTeam() => new(_players.Count);
    
    public IEnumerable<Team> GetAvailableTeams()
    {
        for (var team = 1; team <= _players.Count; team++)
            yield return new Team(team);
    }
    
    #region Setters (should only be accessed by the multiplayer-related nodes)

    public void Add(int playerId, string playerName, bool ready, FactionId faction, Team team)
    {
        if (_players.Any(x => x.Id == playerId))
            return; 
        
        _players.Add(new Player
        {
            Id = playerId,
            Name = playerName,
            Ready = ready,
            Faction = faction,
            Team = team,
        });
        PlayerAdded(playerId);
    }
    
    public void Remove(int id)
    {
        var playerToRemove = TryGet(id);
        if (playerToRemove is null)
        {
            return;
        }
        _players.Remove(playerToRemove);
        PlayerRemoved(id);
    }
    
    public void Reset()
    {
        _players = new List<Player>();
    }

    #endregion Setters (should only be accessed by the multiplayer-related nodes)
}

public class Player : IEquatable<Player>
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    
    public required FactionId Faction { get; set; }
    public required Team Team { get; set; }
    public required bool Ready { get; set; }

    public bool Equals(Player? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Player)obj);
    }

    public override int GetHashCode()
    {
        return Id;
    }
}