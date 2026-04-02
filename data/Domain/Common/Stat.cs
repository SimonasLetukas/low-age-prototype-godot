namespace LowAgeData.Domain.Common;

public abstract class Stat
{
    protected Stat(int maxAmount, bool hasCurrent, bool? allowsOverflow = null)
    {
        MaxAmount = maxAmount;
        HasCurrent = hasCurrent;
        AllowsOverflow = allowsOverflow ?? false;
    }

    public int MaxAmount { get; }
    public bool HasCurrent { get; }
    public bool AllowsOverflow { get; }
}