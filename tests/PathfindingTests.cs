using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Godot;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Common;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Tiles;
using low_age_prototype_common;
using TestProject1.Helpers;
using Xunit;
using Area = low_age_data.Domain.Common.Area;
using Vector2 = Godot.Vector2;

namespace low_age_tests
{
    public class PathfindingTests
    {
        private readonly Vector2 _mapSize = new Vector2(10, 10);
        private readonly Vector2 _startingPoint = new Vector2(0, 0);
        private const float SearchRange = 100f;

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
            SetupStructureInstance(structurePrimaryPosition, structureSize);
            
            var availablePoints = GetAvailablePointsFor(pathfindingSize);

            AssertThatExpectedPositionsAreNotContainedIn(availablePoints, 
                expectedOccupiedPositions, true);
        }
        
        // TODO

        // Complex scenarios: https://docs.google.com/spreadsheets/d/1xS9WhIgo9qVJ2o4FQVnh4gza8S7z9xdcyz47uD-CGVQ/edit?usp=sharing
        // Legend:
        //   o - high ground
        //   / - top ascendable
        //   . - bottom ascendable
        //   x - low ground occupied (o, / and . implicitly occupy low ground)
        //   ~ - places where low ground must be unoccupied

        public static IEnumerable<object[]> GetExpectedOccupationAndHighGroundForGates()
        {
            yield return new object[]
            {
                1,
                new Vector2<int>(4, 3), new Vector2<int>(4, 1), // Position & size
                new [] // High ground
                {
                    new Area(new Vector2<int>(0, 0), new Vector2<int>(4, 1))
                },
                new [] // Ascendable
                {
                    new Area(new Vector2<int>(1, 0), new Vector2<int>(2, 1))
                },
                
                // Expected:
                new[] // Occupation
                {
                    new Vector2(4, 3), new Vector2(5, 3), new Vector2(6, 3), new Vector2(7, 3),
                },
                new[] // High ground
                {
                    new Vector2(4, 3), new Vector2(5, 3), new Vector2(6, 3), new Vector2(7, 3),
                },
                Array.Empty<Vector2>()
                /*new[] // No high ground
                {
                    
                }*/
            };
        }

        [Theory]
        [MemberData(nameof(GetExpectedOccupationAndHighGroundForGates))]
        public void Pathfinding_ShouldCalculateHighGroundAndOccupation_ForGates(
            int pathfindingSize, 
            Vector2<int> structurePrimaryPosition, Vector2<int> structureSize,
            Area[] highGrounds, Area[] ascendables,
            Vector2[] expectedOccupiedPositions, Vector2[] expectedHighGroundPositions,
            Vector2[] expectedNoHighGroundPositions)
        {
            SetupStructureInstance(structurePrimaryPosition, structureSize);
            SetupHighGroundFor(_structure, highGrounds);
            SetupAscendableFor(_structure, ascendables);
            
            var availablePoints = GetAvailablePointsFor(pathfindingSize);

            AssertThatExpectedPositionsAreNotContainedIn(availablePoints, 
                expectedOccupiedPositions, true);
        }
        
        
        // TODO simulate adding and then removing structures + structures with highground
        // TODO also test walkable areas + highground
        // TODO also test diagonal connections for structures
        
        private void SetupStructureInstance(Vector2<int> structurePrimaryPosition, Vector2<int> structureSize)
        {
            var blueprint = _structureBlueprint
                .With(x => x.Size, structureSize)
                .With(x => x.WalkableAreas, new List<Area>())
                .Create();
            _structure.SetBlueprint(blueprint);
            _structure.EntityPrimaryPosition = structurePrimaryPosition.ToGodotVector2();
            _structure.Behaviours._Ready();
            _pathfinding.AddOccupation(_structure);
        }
        
        private void SetupHighGroundFor(StructureNode structure, Area[] highGrounds)
        {
            var highGroundAreas = highGrounds
                .Select(highGround => new HighGroundArea(highGround, new Vector2<int>(0, 0)))
                .ToList();
            var highGroundBlueprint = _fixture.For<HighGround>()
                .With(x => x.Sprite, "res://assets/sprites/structures/revs/boss post front indexed 2x3.png")
                .With(x => x.HighGroundAreas, highGroundAreas)
                .Create();
            structure.Behaviours.AddBehaviour(highGroundBlueprint, null);
        }
        
        private void SetupAscendableFor(StructureNode structure, Area[] ascendables)
        {
            var ascendableAreas = ascendables
                .Select(ascendable => new HighGroundArea(ascendable, new Vector2<int>(0, 0)))
                .ToList();
            var ascendableBlueprint = _fixture.For<Ascendable>()
                .With(x => x.Sprite, "res://assets/sprites/structures/revs/boss post front indexed 2x3.png")
                .With(x => x.Path, ascendableAreas)
                .Create();
            structure.Behaviours.AddBehaviour(ascendableBlueprint, null);
        }
        
        private List<Point> GetAvailablePointsFor(int pathfindingSize)
        {
            _pathfinding.ClearCache();
            var availablePoints = _pathfinding.GetAvailablePoints(
                _startingPoint,
                SearchRange,
                false,
                pathfindingSize).ToList();
            return availablePoints;
        }

        private void AssertThatExpectedPositionsAreNotContainedIn(
            List<Point> availablePoints,
            Vector2[] expectedPositions,
            bool? checkOnlyLowGround)
        {
            foreach (var position in Iterate.Positions(_mapSize))
            {
                if (expectedPositions.Contains(position))
                {
                    if (checkOnlyLowGround is null)
                    {
                        availablePoints
                            .Where(x => x.Position == position)
                            .Should()
                            .BeEmpty();
                        continue;
                    }

                    if (checkOnlyLowGround.Value)
                    {
                        availablePoints
                            .Where(x => x.Position == position && x.IsHighGround is false)
                            .Should()
                            .BeEmpty();
                        continue;
                    }
                    
                    availablePoints
                        .Where(x => x.Position == position && x.IsHighGround)
                        .Should()
                        .BeEmpty();
                }
                else
                    availablePoints
                        .Where(x => PointIsAtPosition(x, position, checkOnlyLowGround))
                        .Should()
                        .NotBeEmpty();
            }
        }

        private static bool PointIsAtPosition(Point point, Vector2 position, bool? checkOnlyLowGround)
        {
            if (checkOnlyLowGround is null)
                return point.Position == position;
            
            if (checkOnlyLowGround.Value)
                return point.IsHighGround is false && point.Position == position;

            return point.IsHighGround && point.Position == position;
        }
    }
}