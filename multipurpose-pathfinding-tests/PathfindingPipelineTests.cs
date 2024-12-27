using System.Numerics;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using low_age_dijkstra;
using low_age_prototype_common;
using multipurpose_pathfinding;

namespace multipurpose_pathfinding_tests;

public class PathfindingPipelineTests
{
    private readonly Configuration _config = new();
    private readonly Vector2<int> _startingPoint = new(0, 0);
    private const float SearchRange = 100f;

    private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization
        {
            ConfigureMembers = true
        });
    
    private readonly MultipurposePathfinding _pathfinding;
    private bool _pathfindingInitialized = false;
    
    private readonly Dictionary<Guid, PathfindingEntity> _entities = new();
    private readonly Dictionary<Guid, IEnumerable<Vector2<int>>> _walkablePositionsByEntityId = new();
    private readonly Dictionary<Guid, IList<IEnumerable<Vector2<int>>>> _ascendablesByEntityId = new();
    private readonly Dictionary<Guid, IEnumerable<Vector2<int>>> _highGroundsByEntityId = new();
    
    public PathfindingPipelineTests()
    {
        _config.MapSize = new Vector2<int>(10, 10);
        _pathfinding = new MultipurposePathfinding();
        var initialPositionsAndTerrainIndexes = IterateVector2Int
            .Positions(_config.MapSize)
            .Select(position => (position, new Terrain(0)))
            .ToList();
        _pathfinding.Initialize(initialPositionsAndTerrainIndexes, _config);
        _pathfinding.FinishedInitializing += OnPathfindingInitialized;
        while (_pathfindingInitialized is false)
        {
            _pathfinding.IterateInitialization(0.0001f);
        }
    }
    
    private void OnPathfindingInitialized() => _pathfindingInitialized = true;
    
    public static IEnumerable<object[]> GetExpectedOccupations()
        {
            yield return
            [
                new PathfindingSize(1),
                new Vector2<int>(4, 3), new Vector2<int>(2, 1),
                // Expected:
                new[]
                {
                    new Vector2<int>(4, 3), new Vector2<int>(5, 3),
                }
            ];
            
            yield return
            [
                new PathfindingSize(1),
                new Vector2<int>(4, 3), new Vector2<int>(1, 2),
                // Expected:
                new[]
                {
                    new Vector2<int>(4, 3),
                    new Vector2<int>(4, 4),
                }
            ];
            
            yield return
            [
                new PathfindingSize(2),
                new Vector2<int>(4, 3), new Vector2<int>(2, 1),
                // Expected:
                new[]
                {
                    new Vector2<int>(3, 2), new Vector2<int>(4, 2), new Vector2<int>(5, 2), 
                    new Vector2<int>(3, 3), new Vector2<int>(4, 3), new Vector2<int>(5, 3), 
                }
            ];
            
            yield return
            [
                new PathfindingSize(2),
                new Vector2<int>(4, 3), new Vector2<int>(1, 2),
                // Expected:
                new[]
                {
                    new Vector2<int>(3, 2), new Vector2<int>(4, 2), 
                    new Vector2<int>(3, 3), new Vector2<int>(4, 3), 
                    new Vector2<int>(3, 4), new Vector2<int>(4, 4), 
                }
            ];
            
            yield return
            [
                new PathfindingSize(3),
                new Vector2<int>(4, 3), new Vector2<int>(2, 1),
                // Expected:
                new[]
                {
                    new Vector2<int>(2, 1), new Vector2<int>(3, 1), new Vector2<int>(4, 1), new Vector2<int>(5, 1), 
                    new Vector2<int>(2, 2), new Vector2<int>(3, 2), new Vector2<int>(4, 2), new Vector2<int>(5, 2), 
                    new Vector2<int>(2, 3), new Vector2<int>(3, 3), new Vector2<int>(4, 3), new Vector2<int>(5, 3), 
                }
            ];
            
            yield return
            [
                new PathfindingSize(3),
                new Vector2<int>(4, 3), new Vector2<int>(1, 2),
                // Expected:
                new[]
                {
                    new Vector2<int>(2, 1), new Vector2<int>(3, 1), new Vector2<int>(4, 1),  
                    new Vector2<int>(2, 2), new Vector2<int>(3, 2), new Vector2<int>(4, 2), 
                    new Vector2<int>(2, 3), new Vector2<int>(3, 3), new Vector2<int>(4, 3),  
                    new Vector2<int>(2, 4), new Vector2<int>(3, 4), new Vector2<int>(4, 4), 
                }
            ];
        }
    
    [Theory]
    [MemberData(nameof(GetExpectedOccupations))]
    public void Pathfinding_ShouldCalculateSimpleOccupation_ForPathfindingSizes(
        PathfindingSize pathfindingSize, 
        Vector2<int> entityPrimaryPosition, Vector2<int> entitySize,
        Vector2<int>[] expectedOccupiedPositions)
    {
        SetupEntityInstance(entityPrimaryPosition, entitySize);
            
        var availablePoints = GetAvailablePointsFor(pathfindingSize);

        ExpectedPositionsShouldNotBeContainedIn(availablePoints, 
            expectedOccupiedPositions, CheckElevation.LowGround);
    }

    #region ComplexScenarios

    // Complex scenarios: https://docs.google.com/spreadsheets/d/1xS9WhIgo9qVJ2o4FQVnh4gza8S7z9xdcyz47uD-CGVQ/edit?usp=sharing
    // Legend:
    //   o - high ground
    //   / - top ascendable
    //   . - bottom ascendable
    //   x - low ground occupied (o, / and . implicitly occupy low ground)
    //   X - high ground occupied
    //   ~ - places where low ground must be unoccupied
    //   - - no connection is allowed between these points
    
    public static IEnumerable<object[]> GetExpectedOccupationAndHighGroundForGates()
    {
        yield return
        [
            new PathfindingSize(1),
            new Vector2<int>(4, 3), new Vector2<int>(4, 1), // Position & size
            new [] // Ascendable
            {
                new Area(new Vector2<int>(5, 3), new Vector2<int>(2, 1))
            },
            new [] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(4, 1))
            },
                
            // Expected:
            new[] // Occupation: low ground
            {
                new Vector2<int>(4, 3), new Vector2<int>(5, 3), new Vector2<int>(6, 3), new Vector2<int>(7, 3),
            },
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new[] // Free high ground
            {
                new Vector2<int>(4, 3), new Vector2<int>(5, 3), new Vector2<int>(6, 3), new Vector2<int>(7, 3),
            }
        ];
        
        yield return
        [
            new PathfindingSize(1),
            new Vector2<int>(4, 3), new Vector2<int>(1, 4), // Position & size
            new [] // Ascendable
            {
                new Area(new Vector2<int>(4, 4), new Vector2<int>(1, 2))
            },
            new [] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(1, 4))
            },
                
            // Expected:
            new[] // Occupation: low ground
            {
                new Vector2<int>(4, 3), 
                new Vector2<int>(4, 4), 
                new Vector2<int>(4, 5), 
                new Vector2<int>(4, 6),
            },
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new[] // Free high ground
            {
                new Vector2<int>(4, 3), 
                new Vector2<int>(4, 4), 
                new Vector2<int>(4, 5), 
                new Vector2<int>(4, 6),
            }
        ];
        
        yield return
        [
            new PathfindingSize(2),
            new Vector2<int>(4, 3), new Vector2<int>(4, 1), // Position & size
            new [] // Ascendable
            {
                new Area(new Vector2<int>(5, 3), new Vector2<int>(2, 1))
            },
            new [] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(4, 1))
            },
                
            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                // TODO new(3, 2), new(4, 2), new(6, 2), new(7, 2),
                new(3, 2), new(4, 2), new(5, 2), new(6, 2), new(7, 2),
                new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(7, 3),
            },
            new Vector2<int>[] // Occupation: high ground
            {
                // TODO new(4, 3), new(6, 3), new(7, 3)
                new(4, 3), new(5, 3), new(6, 3), new(7, 3)
            },
            new Vector2<int>[] // Free high ground
            {
                // TODO new(5, 3),
            }
        ];
        
        yield return
        [
            new PathfindingSize(2),
            new Vector2<int>(4, 3), new Vector2<int>(1, 4), // Position & size
            new [] // Ascendable
            {
                new Area(new Vector2<int>(4, 4), new Vector2<int>(1, 2))
            },
            new [] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(1, 4))
            },
                
            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(3, 2), new(4, 2),
                new(3, 3), new(4, 3), 
                new(3, 4), new(4, 4), // TODO remove new(3, 4)
                new(3, 5), new(4, 5), 
                new(3, 6), new(4, 6),
            },
            new Vector2<int>[] // Occupation: high ground
            {
                new(4, 3), 
                new(4, 4), // TODO remove
                new(4, 5), 
                new(4, 6),
            },
            new Vector2<int>[] // Free high ground
            {
                // TODO new(4, 4)
            }
        ];
        
        yield return
        [
            new PathfindingSize(3),
            new Vector2<int>(4, 3), new Vector2<int>(4, 1), // Position & size
            new [] // Ascendable
            {
                new Area(new Vector2<int>(5, 3), new Vector2<int>(2, 1))
            },
            new [] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(4, 1))
            },
                
            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(2, 1), new(3, 1), new(4, 1), new(5, 1), new(6, 1), new(7, 1),
                new(2, 2), new(3, 2), new(4, 2), new(5, 2), new(6, 2), new(7, 2),
                new(2, 3), new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(7, 3),
            },
            new Vector2<int>[] // Occupation: high ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3)
            },
            Array.Empty<Vector2<int>>() // Free high ground
        ];
        
        yield return
        [
            new PathfindingSize(3),
            new Vector2<int>(4, 3), new Vector2<int>(1, 4), // Position & size
            new [] // Ascendable
            {
                new Area(new Vector2<int>(4, 4), new Vector2<int>(1, 2))
            },
            new [] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(1, 4))
            },
                
            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(2, 1), new(3, 1), new(4, 1),
                new(2, 2), new(3, 2), new(4, 2),
                new(2, 3), new(3, 3), new(4, 3),
                new(2, 4), new(3, 4), new(4, 4),
                new(2, 5), new(3, 5), new(4, 5),
                new(2, 6), new(3, 6), new(4, 6),
            },
            new Vector2<int>[] // Occupation: high ground
            {
                new(4, 3), 
                new(4, 4),
                new(4, 5), 
                new(4, 6),
            },
            Array.Empty<Vector2<int>>() // Free high ground
        ];
    }
    
    [Theory]
    [MemberData(nameof(GetExpectedOccupationAndHighGroundForGates))]
    public void Pathfinding_ShouldCalculateHighGroundAndOccupation_ForGates(
        PathfindingSize pathfindingSize, 
        Vector2<int> entityPrimaryPosition, Vector2<int> entitySize,
        Area[] ascendables,
        Area[] highGrounds, 
        Vector2<int>[] expectedOccupiedLowGroundPositions, 
        Vector2<int>[] expectedOccupiedHighGroundPositions,
        Vector2<int>[] expectedFreeHighGroundPositions)
    {
        SetupEntityInstance(entityPrimaryPosition, entitySize, Team.Default, false, null, 
            ascendables.Select(x => x.ToVectors()).ToList(), 
            highGrounds.SelectMany(x => x.ToVectors()).ToHashSet());
            
        var availablePoints = GetAvailablePointsFor(pathfindingSize);

        ExpectedPositionsShouldNotBeContainedIn(availablePoints, 
            expectedOccupiedLowGroundPositions, CheckElevation.LowGround);
        
        ExpectedPositionsShouldNotBeContainedIn(availablePoints, 
            expectedOccupiedHighGroundPositions, CheckElevation.HighGround);

        expectedFreeHighGroundPositions.Should().BeSubsetOf(
            availablePoints.Where(x => x.IsHighGround).Select(x => x.Position));
    }
    
    // TODO simulate adding and then removing structures + structures with highground
    // TODO also test walkable areas + highground
    // TODO also test diagonal connections for structures

    #endregion ComplexScenarios

    #region Helpers
    
    private List<Point> GetAvailablePointsFor(PathfindingSize pathfindingSize, 
        Team? team = null, bool? lookingFromHighGround = null)
    {
        _pathfinding.ClearCache();
        var availablePoints = _pathfinding.GetAvailablePoints(
            _startingPoint,
            SearchRange,
            lookingFromHighGround ?? false,
            team ?? Team.Default,
            pathfindingSize).ToList();
        return availablePoints;
    }

    private enum CheckElevation
    {
        Any,
        LowGround,
        HighGround
    }

    private void ExpectedPositionsShouldNotBeContainedIn(
        List<Point> availablePoints,
        Vector2<int>[] expectedPositions,
        CheckElevation checkElevation)
    {
        var foundExpectedPositions = new HashSet<Vector2<int>>();
        
        foreach (var position in IterateVector2Int.Positions(_config.MapSize))
        {
            if (expectedPositions.Contains(position))
            {
                if (checkElevation is CheckElevation.Any)
                {
                    var foundPositions = availablePoints
                        .Where(x => x.Position.Equals(position))
                        .Select(x => x.Position);
                    foreach (var pos in foundPositions)
                    {
                        foundExpectedPositions.Add(pos);
                    }
                    continue;
                }

                if (checkElevation is CheckElevation.LowGround)
                {
                    var foundPositions = availablePoints
                        .Where(x => x.Position.Equals(position) && x.IsHighGround is false)
                        .Select(x => x.Position);
                    foreach (var pos in foundPositions)
                    {
                        foundExpectedPositions.Add(pos);
                    }
                    continue;
                }
                
                var foundPos = availablePoints
                    .Where(x => x.Position.Equals(position) && x.IsHighGround)
                    .Select(x => x.Position);
                foreach (var pos in foundPos)
                {
                    foundExpectedPositions.Add(pos);
                }
            }
            else if (checkElevation is CheckElevation.LowGround) 
                availablePoints
                    .Where(x => x.IsHighGround is false && x.Position == position)
                    .Should()
                    .NotBeEmpty("Remaining low ground has to be unoccupied");
        }

        foundExpectedPositions.Should().BeSubsetOf(Array.Empty<Vector2<int>>());
    }
    
    private void SetupEntityInstance(Vector2<int> entityPrimaryPosition, Vector2<int> entitySize, 
        Team? team = null, bool? isOnHighGround = null, IEnumerable<Vector2<int>>? walkablePositions = null,
        IList<IEnumerable<Vector2<int>>>? ascendablePositions = null,
        IEnumerable<Vector2<int>>? highGroundPositions = null)
    {
        var entityId = Guid.NewGuid();
        var entity = new PathfindingEntity(entityId, entityPrimaryPosition, entitySize, team ?? Team.Default, 
            isOnHighGround ?? false, (p, t) => CanBeMovedThroughAt(p, t, entityId));
        
        _entities[entityId] = entity;
        _walkablePositionsByEntityId[entityId] = walkablePositions ?? new List<Vector2<int>>();
        _ascendablesByEntityId[entityId] = ascendablePositions 
                                           ?? new List<IEnumerable<Vector2<int>>> { new List<Vector2<int>>() };
        _highGroundsByEntityId[entityId] = highGroundPositions ?? new List<Vector2<int>>();
        
        _pathfinding.AddOrUpdateEntity(entity);
        _pathfinding.AddOccupation(entity.Id);
        if (ascendablePositions != null)
            _pathfinding.AddAscendableHighGround(entityId, ascendablePositions);
        if (highGroundPositions != null)
            _pathfinding.AddHighGround(entityId, highGroundPositions);
    }

    private bool CanBeMovedThroughAt(Point point, Team forTeam, Guid entityId)
    {
        if (_walkablePositionsByEntityId[entityId].Any(point.Position.Equals))
            return true;

        if (HasHighGroundAt(point, forTeam, entityId))
            return true;
        
        if (point.Position.IsInBoundsOf(
                _entities[entityId].Position, 
                _entities[entityId].Position + _entities[entityId].Size))
            return false;

        return true;
    }

    private bool HasHighGroundAt(Point point, Team forTeam, Guid entityId)
    {
        if (point.IsHighGround is false)
            return false;
        
        var position = point.Position;
        
        if (position.IsInBoundsOf(
                _entities[entityId].Position, 
                _entities[entityId].Position + _entities[entityId].Size) 
            is false)
            return false;

        if (CanBeMovedOnAtAscendable(position, forTeam, entityId) 
            && CanBeMovedOnAtHighGround(position, entityId))
            return true;

        return false;
    }

    private bool CanBeMovedOnAtAscendable(Vector2<int> position, Team forTeam, Guid entityId)
    {
        if (_entities[entityId].Team.Equals(forTeam) is false)
            return false;

        var ascendablePositions = _ascendablesByEntityId[entityId]
            .SelectMany(inner => inner)
            .ToHashSet();

        return ascendablePositions.Contains(position);
    }

    private bool CanBeMovedOnAtHighGround(Vector2<int> position, Guid entityId) 
        => _highGroundsByEntityId[entityId].Contains(position);

    #endregion Helpers
}