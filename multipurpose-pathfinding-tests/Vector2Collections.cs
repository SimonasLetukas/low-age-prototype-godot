using low_age_prototype_common;

namespace multipurpose_pathfinding_tests;

public static class Vector2Collections
{
    public static readonly Vector2<int>[] Size2BoundariesFor10X10 =
    [
        new(9, 0), new(9, 1), new(9, 2), new(9, 3), new(9, 4), new(9, 5), new(9, 6), new(9, 7), new(9, 8), new(9, 9),
        new(0, 9), new(1, 9), new(2, 9), new(3, 9), new(4, 9), new(5, 9), new(6, 9), new(7, 9), new(8, 9)
    ];

    public static readonly Vector2<int>[] Size3BoundariesFor10X10 =
    [
        new(8, 0), new(8, 1), new(8, 2), new(8, 3), new(8, 4), new(8, 5), new(8, 6), new(8, 7), new(8, 8), new(8, 9),
        new(9, 0), new(9, 1), new(9, 2), new(9, 3), new(9, 4), new(9, 5), new(9, 6), new(9, 7), new(9, 8), new(9, 9),
        new(0, 8), new(1, 8), new(2, 8), new(3, 8), new(4, 8), new(5, 8), new(6, 8), new(7, 8), 
        new(0, 9), new(1, 9), new(2, 9), new(3, 9), new(4, 9), new(5, 9), new(6, 9), new(7, 9), 
    ];
}