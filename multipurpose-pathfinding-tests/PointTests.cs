using FluentAssertions;
using low_age_prototype_common;
using multipurpose_pathfinding;

namespace multipurpose_pathfinding_tests;

public class PointTests
{
    [Fact]
    public void IsImpassible_ShouldReturnTrue_WhenOriginalTerrainHasInfiniteWeight()
    {
        var config = new Configuration();
        var point = new Point
        {
            Id = 0,
            Position = Vector2Int.One,
            HighGroundAscensionLevel = 0,
            IsHighGround = false,
            IsImpassable = false,
            OriginalTerrainIndex = 1,
            Configuration = config
        };

        point.IsImpassable.Should().BeTrue();
    }
    
    [Fact]
    public void IsImpassible_ShouldReturnTrue_WhenItWasSetAsTrue()
    {
        var config = new Configuration();
        var point = new Point
        {
            Id = 0,
            Position = Vector2Int.One,
            HighGroundAscensionLevel = 0,
            IsHighGround = false,
            IsImpassable = true,
            OriginalTerrainIndex = 0,
            Configuration = config
        };

        point.IsImpassable.Should().BeTrue();
    }
    
    [Fact]
    public void IsImpassible_ShouldReturnFalse_WhenItWasSetAsFalseAndTerrainWeightIsNotInfinite()
    {
        var config = new Configuration();
        var point = new Point
        {
            Id = 0,
            Position = Vector2Int.One,
            HighGroundAscensionLevel = 0,
            IsHighGround = false,
            IsImpassable = false,
            OriginalTerrainIndex = 0,
            Configuration = config
        };

        point.IsImpassable.Should().BeFalse();
    }
}