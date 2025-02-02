using System.Collections.Generic;
using Godot;

public static class CollectionExtensions
{
    public static Godot.Collections.Array<T> ToGodotArray<[MustBeVariant] T>(this IEnumerable<T> collection) 
        => new (collection);
}