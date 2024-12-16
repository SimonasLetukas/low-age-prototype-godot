using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Godot;
using low_age_data.Domain.Tiles;
using Xunit;

namespace low_age_tests
{
    public class PathfindingInitializationTerrainTests
    {
        private readonly Vector2 _mapSize = new Vector2(10, 10);
        private readonly Vector2 _startingPoint = new Vector2(0, 0);
        private const float SearchRange = 100f;

        private readonly Pathfinding _pathfinding;
        private bool _pathfindingInitialized = false;
        
        public PathfindingInitializationTerrainTests()
        {
            Data.Instance.ReadBlueprint();

            _pathfinding = new Pathfinding();
            _pathfinding._Ready();
        }
        
        private void OnPathfindingInitialized() => _pathfindingInitialized = true;
        
        public static IEnumerable<object[]> GetExpectedMountains()
        {
            yield return new object[]
            {
                1,
                new[]
                {
                    new Vector2(4, 3), new Vector2(5, 3),
                },
                // Expected:
                new[]
                {
                    new Vector2(4, 3), new Vector2(5, 3),
                }
            };
            
            yield return new object[]
            {
                1,
                new[]
                {
                    new Vector2(4, 3),
                    new Vector2(4, 4),
                },
                // Expected:
                new[]
                {
                    new Vector2(4, 3),
                    new Vector2(4, 4),
                }
            };
            
            yield return new object[]
            {
                2,
                new[]
                {
                    new Vector2(4, 3), new Vector2(5, 3),
                },
                // Expected:
                new[]
                {
                    new Vector2(3, 2), new Vector2(4, 2), new Vector2(5, 2), 
                    new Vector2(3, 3), new Vector2(4, 3), new Vector2(5, 3), 
                }
            };
            
            yield return new object[]
            {
                2,
                new[]
                {
                    new Vector2(4, 3),
                    new Vector2(4, 4),
                },
                // Expected:
                new[]
                {
                    new Vector2(3, 2), new Vector2(4, 2), 
                    new Vector2(3, 3), new Vector2(4, 3), 
                    new Vector2(3, 4), new Vector2(4, 4), 
                }
            };
            
            yield return new object[]
            {
                3,
                new[]
                {
                    new Vector2(4, 3), new Vector2(5, 3),
                },
                // Expected:
                new[]
                {
                    new Vector2(2, 1), new Vector2(3, 1), new Vector2(4, 1), new Vector2(5, 1), 
                    new Vector2(2, 2), new Vector2(3, 2), new Vector2(4, 2), new Vector2(5, 2), 
                    new Vector2(2, 3), new Vector2(3, 3), new Vector2(4, 3), new Vector2(5, 3), 
                }
            };
            
            yield return new object[]
            {
                3,
                new[]
                {
                    new Vector2(4, 3),
                    new Vector2(4, 4),
                },
                // Expected:
                new[]
                {
                    new Vector2(2, 1), new Vector2(3, 1), new Vector2(4, 1),  
                    new Vector2(2, 2), new Vector2(3, 2), new Vector2(4, 2), 
                    new Vector2(2, 3), new Vector2(3, 3), new Vector2(4, 3),  
                    new Vector2(2, 4), new Vector2(3, 4), new Vector2(4, 4), 
                }
            }; 
        }
        
        [Theory]
        [MemberData(nameof(GetExpectedMountains))]
        public void Pathfinding_ShouldCalculateMountainsPathing_ForPathfindingSizes(
            int pathfindingSize, 
            Vector2[] mountainPositions,
            Vector2[] expectedMountainPositions)
        {
            var tiles = Iterate
                .Positions(_mapSize)
                .Select(position => (position, mountainPositions.Contains(position) 
                    ? TileId.Mountains 
                    : TileId.Grass))
                .ToList();
            _pathfinding.Initialize(_mapSize, tiles, Constants.Pathfinding.MaxSizeForPathfinding);
            _pathfinding.FinishedInitializing += OnPathfindingInitialized;
            while (_pathfindingInitialized is false)
            {
                _pathfinding._Process(1f);
            }
            
            _pathfinding.ClearCache();
            var availablePoints = _pathfinding.GetAvailablePoints(
                _startingPoint,
                SearchRange,
                false,
                pathfindingSize).ToList();
            
            foreach (var position in Iterate.Positions(_mapSize))
            {
                if (expectedMountainPositions.Contains(position))
                    availablePoints.Where(x => x.Position == position).Should().BeEmpty();
                else
                    availablePoints.Where(x => x.Position == position).Should().NotBeEmpty();
            }
        }
    }
}