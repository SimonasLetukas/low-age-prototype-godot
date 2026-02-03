using System;
using System.Collections.Generic;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;

public class IncomeProvider : IEquatable<IncomeProvider>
{
    public required Guid Id { get; init; }
    public required Guid EntityId { get; init; }
    public required Player Player { get; init; }
    public required BehaviourId ProviderType { get; init; }
    public required IList<Payment> ResourcesGained { get; init; }
    public required int DiminishingReturn { get; init; }
    public required IList<Payment> Cost { get; init; }
    public required bool WaitForAvailableStorage { get; init; }

    public bool Equals(IncomeProvider? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((IncomeProvider)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}