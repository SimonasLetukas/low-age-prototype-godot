using FluentAssertions;
using DijkstraMap;
using LowAgeCommon;
using MultipurposePathfinding;

namespace MultipurposePathfindingTests;

public class InitializationTerrainTests
{
    private readonly Vector2Int _startingPosition = new(0, 0);
    private readonly Configuration _config = new();
    private const float SearchRange = 100f;

    private readonly Pathfinding _pathfinding = new();
    private bool _pathfindingInitialized = false;

    private void OnPathfindingInitialized() => _pathfindingInitialized = true;

    public static IEnumerable<object[]> GetExpectedMountains()
    {
        yield return
        [
            1,
            new[]
            {
                new Vector2Int(4, 3), new Vector2Int(5, 3),
            },
            // Expected:
            new[]
            {
                new Vector2Int(4, 3), new Vector2Int(5, 3),
            }
        ];

        yield return
        [
            1,
            new[]
            {
                new Vector2Int(4, 3),
                new Vector2Int(4, 4),
            },
            // Expected:
            new[]
            {
                new Vector2Int(4, 3),
                new Vector2Int(4, 4),
            }
        ];

        yield return
        [
            2,
            new[]
            {
                new Vector2Int(4, 3), new Vector2Int(5, 3),
            },
            // Expected:
            new[]
            {
                new Vector2Int(3, 2), new Vector2Int(4, 2), new Vector2Int(5, 2),
                new Vector2Int(3, 3), new Vector2Int(4, 3), new Vector2Int(5, 3), 
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray()
        ];

        yield return
        [
            2,
            new[]
            {
                new Vector2Int(4, 3),
                new Vector2Int(4, 4),
            },
            // Expected:
            new[]
            {
                new Vector2Int(3, 2), new Vector2Int(4, 2),
                new Vector2Int(3, 3), new Vector2Int(4, 3),
                new Vector2Int(3, 4), new Vector2Int(4, 4),
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray()
        ];

        yield return
        [
            3,
            new[]
            {
                new Vector2Int(4, 3), new Vector2Int(5, 3),
            },
            // Expected:
            new[]
            {
                new Vector2Int(2, 1), new Vector2Int(3, 1), new Vector2Int(4, 1), new Vector2Int(5, 1),
                new Vector2Int(2, 2), new Vector2Int(3, 2), new Vector2Int(4, 2), new Vector2Int(5, 2),
                new Vector2Int(2, 3), new Vector2Int(3, 3), new Vector2Int(4, 3), new Vector2Int(5, 3),
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray()
        ];

        yield return
        [
            3,
            new[]
            {
                new Vector2Int(4, 3),
                new Vector2Int(4, 4),
            },
            // Expected:
            new[]
            {
                new Vector2Int(2, 1), new Vector2Int(3, 1), new Vector2Int(4, 1),
                new Vector2Int(2, 2), new Vector2Int(3, 2), new Vector2Int(4, 2),
                new Vector2Int(2, 3), new Vector2Int(3, 3), new Vector2Int(4, 3),
                new Vector2Int(2, 4), new Vector2Int(3, 4), new Vector2Int(4, 4),
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray()
        ];
    }

    [Theory]
    [MemberData(nameof(GetExpectedMountains))]
    public void Pathfinding_ShouldCalculateMountainsPathing_ForPathfindingSizes(
        int pathfindingSize,
        Vector2Int[] mountainPositions,
        Vector2Int[] expectedMountainPositions)
    {
        var initialPositionsAndTerrainIndexes = IterateVector2Int
            .Positions(_config.MapSize)
            .Select(position => (position, mountainPositions.Contains(position)
                ? new Terrain(1)
                : new Terrain(0)))
            .ToList();

        _pathfinding.Initialize(initialPositionsAndTerrainIndexes, _config);
        _pathfinding.FinishedInitializing += OnPathfindingInitialized;
        while (_pathfindingInitialized is false)
        {
            _pathfinding.IterateInitialization(0.01f);
        }

        _pathfinding.ClearCache();
        var availablePoints = _pathfinding.GetAvailablePoints(
            _startingPosition,
            SearchRange,
            false,
            Team.Default,
            pathfindingSize).ToList();

        foreach (var position in IterateVector2Int.Positions(_config.MapSize))
        {
            if (expectedMountainPositions.Contains(position))
                availablePoints.Where(x => x.Position == position).Should().BeEmpty();
            else
                availablePoints.Where(x => x.Position == position).Should().NotBeEmpty();
        }
    }
    
    public static IEnumerable<object[]> GetExpectedDiagonals()
    { 
        yield return
        [
            new Vector2Int(0, 0),
            new[]
            {
                new Vector2Int(1, 1), new Vector2Int(2, 2),
            },
            // Expected:
            true
        ];
        
        yield return
        [
            new Vector2Int(0, 0),
            new[]
            {
                new Vector2Int(1, 0), new Vector2Int(0, 1),
            },
            // Expected:
            false
        ];
        
        yield return
        [
            new Vector2Int(0, 2),
            new[]
            {
                new Vector2Int(0, 1), new Vector2Int(1, 2), new Vector2Int(1, 3),
            },
            // Expected:
            false
        ];
        
        yield return
        [
            new Vector2Int(3, 1),
            new[]
            {
                new Vector2Int(3, 0), new Vector2Int(2, 1), new Vector2Int(3, 2),
            },
            // Expected:
            false
        ];
        
        yield return
        [
            new Vector2Int(0, 0),
            new[]
            {
                new Vector2Int(1, 1), new Vector2Int(3, 0),
                new Vector2Int(1, 2), new Vector2Int(3, 1),
                new Vector2Int(1, 3), new Vector2Int(3, 2),
            },
            // Expected:
            true
        ];
    }

    [Theory]
    [MemberData(nameof(GetExpectedDiagonals))]
    public void Pathfinding_EndPositionShouldBeAccessibleAccordingToDiagonals_WithGivenMountainsInA4x4Map(
        Vector2Int startPosition, Vector2Int[] mountainPositions, bool expectedToBeAccessible)
    {
        var endPosition = new Vector2Int(3, 3);
        var mapSize = new Vector2Int(4, 4);
        _config.MapSize = mapSize;
        
        var initialPositionsAndTerrainIndexes = IterateVector2Int
            .Positions(_config.MapSize)
            .Select(position => (position, mountainPositions.Contains(position)
                ? new Terrain(1)
                : new Terrain(0)))
            .ToList();

        _pathfinding.Initialize(initialPositionsAndTerrainIndexes, _config);
        _pathfinding.FinishedInitializing += OnPathfindingInitialized;
        while (_pathfindingInitialized is false)
        {
            _pathfinding.IterateInitialization(0.01f);
        }

        _pathfinding.ClearCache();
        var availablePositions = _pathfinding
            .GetAvailablePoints(
                startPosition, 
                SearchRange,
            false,
                Team.Default,
                PathfindingSize.Default)
            .Select(x => x.Position)
            .ToList();

        if (expectedToBeAccessible)
            availablePositions.Should().Contain(endPosition);
        else
            availablePositions.Should().NotContain(endPosition);
    }
}