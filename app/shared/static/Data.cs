using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Factions;

public static class Data
{
    public static IList<Player> Players { get; set; } = new List<Player>();

    public static void RemovePlayer(int id)
    {
        var playerToRemove = Players.SingleOrDefault(x => x.Id.Equals(id));
        if (playerToRemove is null)
        {
            return;
        }
        Players.Remove(playerToRemove);
    }

    public static void Reset()
    {
        Players = new List<Player>();
    }
}

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Faction { get; set; }
}