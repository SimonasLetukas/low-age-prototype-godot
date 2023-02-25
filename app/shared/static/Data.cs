using System.Collections.Generic;
using low_age_data.Domain.Factions;

public static class Data
{
    public static IList<Player> Players { get; set; } = new List<Player>();
}

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; }
    public FactionName Faction { get; set; }
}