using System.Collections.Generic;

public interface ITargetable
{
    HashSet<IAbilityNode> TargetedBy { get; }
    
    string ToString();
}