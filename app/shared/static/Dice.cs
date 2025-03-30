using System.ComponentModel.DataAnnotations;
using System.Linq;

public static class Dice
{
    public static int Roll([Range(1, int.MaxValue)] int sides) => SharedRandom.Instance.Next(1, sides + 1);

    public static int[] RollMultiple(int sides, [Range(1, int.MaxValue)] int count) 
        => Enumerable.Range(0, count).Select(_ => Roll(sides)).ToArray();
}