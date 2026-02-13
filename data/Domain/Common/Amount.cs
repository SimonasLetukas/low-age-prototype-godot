using LowAgeData.Domain.Common.Flags;

namespace LowAgeData.Domain.Common;

public class Amount
{
    public Amount(
        int flat, 
        float? multiplier = null, 
        AmountMultiplyOfFlag? multiplierOf = null,
        Location? multiplyTarget = null)
    {
        Flat = flat;
        Multiplier = multiplier;
        MultiplierOf = multiplierOf;
        MultiplyTarget = multiplyTarget ?? Location.Inherited;
    }

    /// <summary>
    /// Starting amount before any <see cref="Multiplier"/>s are added.
    /// </summary>
    public int Flat { get; }
    
    /// <summary>
    /// Factor of multiplication of a <see cref="MultiplierOf"/> value for a <see cref="MultiplyTarget"/>. The
    /// multiplication result is added to <see cref="Flat"/> amount. Usually, if there are multiple
    /// <see cref="Amount"/>s affecting the final result, the <see cref="Amount"/>s with any <see cref="Multiplier"/>
    /// value are applied last.
    ///
    /// Default = null (no multiplication).
    /// </summary>
    public float? Multiplier { get; }
    
    /// <summary>
    /// Select what value the <see cref="Multiplier"/> affects.
    ///
    /// Default = null.
    /// </summary>
    public AmountMultiplyOfFlag? MultiplierOf { get; }
    
    /// <summary>
    /// Specifies what is the target of <see cref="MultiplierOf"/> flag. Supported: <see cref="Location.Inherited"/>,
    /// <see cref="Location.Self"/>, <see cref="Location.Actor"/>, <see cref="Location.Source"/> or
    /// <see cref="Location.Origin"/>.
    ///
    /// Default = <see cref="Location.Inherited"/>.
    /// </summary>
    public Location MultiplyTarget { get; }
}