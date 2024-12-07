using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Godot;
using low_age_data.Domain.Common;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Tiles;
using low_age_prototype_common;
using TestProject1.Helpers;
using Xunit;
using Area = low_age_data.Domain.Common.Area;

namespace low_age_tests
{
    public class PathfindingTests
    {
        private readonly Vector2 _mapSize = new Vector2(10, 10);
        private readonly Vector2 _startingPoint = new Vector2(0, 0);
        private readonly float _searchRange = 100f;
        
        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization
            {
                ConfigureMembers = true
            });
        
        private readonly FixtureCustomization<Structure> _structureBlueprint;
        private readonly StructureNode _structure;
        private readonly Pathfinding _pathfinding;
        private bool _pathfindingInitialized = false;

        public PathfindingTests()
        {
            Data.Instance.ReadBlueprint();
            _structureBlueprint = _fixture
                .For<Structure>()
                .With(x => x.Sprite, "res://assets/sprites/structures/revs/boss post front indexed 2x3.png")
                .With(x => x.BackSideSprite, "res://assets/sprites/structures/revs/boss post back indexed 2x3.png");
            _structure = StructureNode.Instance();
            _structure._Ready();
            _structure.Renderer._Ready();

            _pathfinding = new Pathfinding();
            _pathfinding._Ready();
            var tiles = Iterate
                .Positions(_mapSize)
                .Select(position => (position, TileId.Grass))
                .ToList();
            _pathfinding.Initialize(_mapSize, tiles, Constants.Pathfinding.MaxSizeForPathfinding);
            _pathfinding.FinishedInitializing += OnPathfindingInitialized;
            while (_pathfindingInitialized is false)
            {
                _pathfinding._Process(1f);
            }
        }

        private void OnPathfindingInitialized() => _pathfindingInitialized = true;

        public static IEnumerable<object[]> GetExpectedOccupations()
        {
            yield return new object[]
            {
                1,
                new Vector2<int>(4, 3), new Vector2<int>(2, 1),
                // Expected:
                new[]
                {
                    new Vector2(4, 3), new Vector2(5, 3),
                }
            };
            
            yield return new object[]
            {
                1,
                new Vector2<int>(4, 3), new Vector2<int>(1, 2),
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
                new Vector2<int>(4, 3), new Vector2<int>(2, 1),
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
                new Vector2<int>(4, 3), new Vector2<int>(1, 2),
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
                new Vector2<int>(4, 3), new Vector2<int>(2, 1),
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
                new Vector2<int>(4, 3), new Vector2<int>(1, 2),
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
        [MemberData(nameof(GetExpectedOccupations))]
        public void Pathfinding_ShouldCalculateOccupation_ForPathfindingSizes(
            int pathfindingSize, 
            Vector2<int> structurePrimaryPosition, Vector2<int> structureSize,
            Vector2[] expectedOccupiedPositions)
        {
            var blueprint = _structureBlueprint
                .With(x => x.Size, structureSize)
                .With(x => x.WalkableAreas, new List<Area>())
                .Create();
            _structure.SetBlueprint(blueprint);
            _structure.EntityPrimaryPosition = structurePrimaryPosition.ToGodotVector2();
            
            _pathfinding.AddOccupation(_structure);
            _pathfinding.ClearCache();
            var availablePoints = _pathfinding.GetAvailablePoints(
                _startingPoint,
                _searchRange,
                false,
                pathfindingSize).ToList();

            foreach (var position in Iterate.Positions(_mapSize))
            {
                if (expectedOccupiedPositions.Contains(position))
                    availablePoints.Where(x => x.Position == position).Should().BeEmpty();
                else
                    availablePoints.Where(x => x.Position == position).Should().NotBeEmpty();
            }
        }
        
        public static IEnumerable<object[]> GetExpected2XOccupations()
        {
            yield return new object[]
            {
                new Vector2<int>(4, 3), new Vector2<int>(2, 1),
                // Expected:
                new[]
                {
                    new Vector2(3, 2), new Vector2(4, 2), new Vector2(5, 2), 
                    new Vector2(3, 3), new Vector2(4, 3), new Vector2(5, 3), 
                }
            };
            
            yield return new object[]
            {
                new Vector2<int>(4, 3), new Vector2<int>(1, 2),
                // Expected:
                new[]
                {
                    new Vector2(3, 2), new Vector2(4, 2), 
                    new Vector2(3, 3), new Vector2(4, 3), 
                    new Vector2(3, 4), new Vector2(4, 4), 
                }
            };
        }
        
        // TODO

        // Complex scenarios: https://docs.google.com/spreadsheets/d/1xS9WhIgo9qVJ2o4FQVnh4gza8S7z9xdcyz47uD-CGVQ/edit?usp=sharing
        // Legend:
        //   o - high ground
        //   / - top ascendable
        //   . - bottom ascendable
        //   x - low ground occupied (o, / and . implicitly occupy low ground)
        //   ~ - places where low ground must be unoccupied

        public static IEnumerable<object[]> GetExpected1XLongStairsByOccupationAnd() // TODO
        {
            yield return new object[]
            {
                Array.Empty<Vector2>(),
                // Expected:
                Array.Empty<Rect2>()
            };
        }

        /*
        [Theory]
        public void Pathfinding_ShouldCalculateHighGroundAndOccupation_For1XSizes_WithLongStairs(
            Vector2<int> position, Vector2<int> size, HighGroundArea[] highGroundAreas, HighGroundArea[] ascendableAreas,
            Vector2<int>[] expectedOccupiedPositions, Vector2<int>[] expectedHighGroundPositions)
        {

        }

        [Theory]
        public void Pathfinding_ShouldCalculateHighGroundAndOccupation_For2XSizes_WithLongStairs()
        {

        }

        [Theory]
        public void Pathfinding_ShouldCalculateHighGroundAndOccupation_For3XSizes_WithLongStairs()
        {

        }

        [Theory]
        public void Pathfinding_ShouldCalculateHighGroundAndOccupation_For1XSizes_WithGates()
        {

        }

        [Theory]
        public void Pathfinding_ShouldCalculateHighGroundAndOccupation_For2XSizes_WithGates()
        {

        }

        [Theory]
        public void Pathfinding_ShouldCalculateHighGroundAndOccupation_For3XSizes_WithGates()
        {

        }

        [Theory]
        public void Pathfinding_ShouldCalculateHighGroundAndOccupation_For1XSizes_WithEnclosedBase()
        {

        }

        [Theory]
        public void Pathfinding_ShouldCalculateHighGroundAndOccupation_For2XSizes_WithEnclosedBase()
        {

        }

        [Theory]
        public void Pathfinding_ShouldCalculateHighGroundAndOccupation_For3XSizes_WithEnclosedBase()
        {

        }

        */
        
        // TODO simulate adding and then removing structures + structures with highground
        // TODO also test walkable areas + highground
        // TODO also test diagonal connections for structures
    }
}