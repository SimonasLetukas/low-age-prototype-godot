using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using low_age_dijkstra;
using low_age_prototype_common;
using multipurpose_pathfinding;

namespace multipurpose_pathfinding_tests;

// TODO add a scenario for handling units standing on top of high ground

[SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
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
        _config.MaxNumberOfTeams = 2;
        _pathfinding = new MultipurposePathfinding();
        var initialPositionsAndTerrainIndexes = IterateVector2Int
            .Positions(_config.MapSize)
            .Select(position => (position, new Terrain(0)))
            .ToList();
        _pathfinding.Initialize(initialPositionsAndTerrainIndexes, _config);
        _pathfinding.FinishedInitializing += OnPathfindingInitialized;
        while (_pathfindingInitialized is false)
        {
            _pathfinding.IterateInitialization(0.01f);
        }
    }

    private void OnPathfindingInitialized() => _pathfindingInitialized = true;

    #region Simple Scenarios: Occupation

    public static IEnumerable<object[]> GetExpectedOccupations()
    {
        yield return
        [
            1, // Scenario
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
            2, // Scenario
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
            3, // Scenario
            new PathfindingSize(2),
            new Vector2<int>(4, 3), new Vector2<int>(2, 1),
            // Expected:
            new[]
            {
                new Vector2<int>(3, 2), new Vector2<int>(4, 2), new Vector2<int>(5, 2),
                new Vector2<int>(3, 3), new Vector2<int>(4, 3), new Vector2<int>(5, 3),
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray()
        ];

        yield return
        [
            4, // Scenario
            new PathfindingSize(2),
            new Vector2<int>(4, 3), new Vector2<int>(1, 2),
            // Expected:
            new[]
            {
                new Vector2<int>(3, 2), new Vector2<int>(4, 2),
                new Vector2<int>(3, 3), new Vector2<int>(4, 3),
                new Vector2<int>(3, 4), new Vector2<int>(4, 4),
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray()
        ];

        yield return
        [
            5, // Scenario
            new PathfindingSize(3),
            new Vector2<int>(4, 3), new Vector2<int>(2, 1),
            // Expected:
            new[]
            {
                new Vector2<int>(2, 1), new Vector2<int>(3, 1), new Vector2<int>(4, 1), new Vector2<int>(5, 1),
                new Vector2<int>(2, 2), new Vector2<int>(3, 2), new Vector2<int>(4, 2), new Vector2<int>(5, 2),
                new Vector2<int>(2, 3), new Vector2<int>(3, 3), new Vector2<int>(4, 3), new Vector2<int>(5, 3),
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray()
        ];

        yield return
        [
            5, // Scenario
            new PathfindingSize(3),
            new Vector2<int>(4, 3), new Vector2<int>(1, 2),
            // Expected:
            new[]
            {
                new Vector2<int>(2, 1), new Vector2<int>(3, 1), new Vector2<int>(4, 1),
                new Vector2<int>(2, 2), new Vector2<int>(3, 2), new Vector2<int>(4, 2),
                new Vector2<int>(2, 3), new Vector2<int>(3, 3), new Vector2<int>(4, 3),
                new Vector2<int>(2, 4), new Vector2<int>(3, 4), new Vector2<int>(4, 4),
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray()
        ];
    }

    [Theory]
    [MemberData(nameof(GetExpectedOccupations))]
    public void Pathfinding_ShouldCalculateSimpleOccupation_ForPathfindingSizes(
        int scenario,
        PathfindingSize pathfindingSize,
        Vector2<int> entityPrimaryPosition, Vector2<int> entitySize,
        Vector2<int>[] expectedOccupiedPositions)
    {
        SetupAddingEntityInstance(entityPrimaryPosition, entitySize);

        var availablePoints = GetAvailablePointsFor(pathfindingSize);

        ExpectedPositionsShouldNotBeContainedIn(availablePoints,
            expectedOccupiedPositions, CheckElevation.LowGround,
            scenario.ToString());
    }

    #endregion Simple Scenarios: Occupation

    // Complex scenarios: https://docs.google.com/spreadsheets/d/1xS9WhIgo9qVJ2o4FQVnh4gza8S7z9xdcyz47uD-CGVQ/edit?usp=sharing
    // Legend:
    //   o - high ground
    //   / - top ascendable
    //   . - bottom ascendable
    //   x - low ground occupied (o, / and . implicitly occupy low ground)
    //   X - high ground occupied
    //   ~ - places where low ground must be unoccupied
    //   - - no connection is allowed between these points
    //   w - walkable positions
    //   # - inaccessible

    #region Complex Scenarios: 1. Long Stairs

    public static IEnumerable<object[]> GetExpectedOccupationAndHighGroundForLongStairs()
    {
        yield return
        [
            "B2", // Scenario
            new PathfindingSize(1),
            new Vector2<int>(4, 3), new Vector2<int>(3, 5), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 4), new Vector2<int>(3, 1)),
                new Area(new Vector2<int>(4, 3), new Vector2<int>(3, 1)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 5), new Vector2<int>(3, 3))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3),
                new(4, 4), new(5, 4), new(6, 4),
                new(4, 5), new(5, 5), new(6, 5),
                new(4, 6), new(5, 6), new(6, 6),
                new(4, 7), new(5, 7), new(6, 7),
            },
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3), new(6, 3),
                new(4, 4), new(5, 4), new(6, 4),
                new(4, 5), new(5, 5), new(6, 5),
                new(4, 6), new(5, 6), new(6, 6),
                new(4, 7), new(5, 7), new(6, 7),
            }
        ];
        
        yield return
        [
            "N2", // Scenario
            new PathfindingSize(1),
            new Vector2<int>(3, 3), new Vector2<int>(5, 3), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(6, 3), new Vector2<int>(1, 3)),
                new Area(new Vector2<int>(7, 3), new Vector2<int>(1, 3)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(3, 3), new Vector2<int>(3, 3))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5),
            },
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new Vector2<int>[] // Free high ground
            {
                new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5),
            }
        ];

        yield return
        [
            "Z2", // Scenario
            new PathfindingSize(1),
            new Vector2<int>(4, 3), new Vector2<int>(3, 5), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 6), new Vector2<int>(3, 1)),
                new Area(new Vector2<int>(4, 7), new Vector2<int>(3, 1)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(3, 3))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3),
                new(4, 4), new(5, 4), new(6, 4),
                new(4, 5), new(5, 5), new(6, 5),
                new(4, 6), new(5, 6), new(6, 6),
                new(4, 7), new(5, 7), new(6, 7),
            },
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3), new(6, 3),
                new(4, 4), new(5, 4), new(6, 4),
                new(4, 5), new(5, 5), new(6, 5),
                new(4, 6), new(5, 6), new(6, 6),
                new(4, 7), new(5, 7), new(6, 7),
            }
        ];

        yield return
        [
            "AL2", // Scenario
            new PathfindingSize(1),
            new Vector2<int>(3, 3), new Vector2<int>(5, 3), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(1, 3)),
                new Area(new Vector2<int>(3, 3), new Vector2<int>(1, 3)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(5, 3), new Vector2<int>(3, 3))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5),
            },
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new Vector2<int>[] // Free high ground
            {
                new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5),
            }
        ];
        
        yield return
        [
            "B14", // Scenario
            new PathfindingSize(2),
            new Vector2<int>(4, 3), new Vector2<int>(3, 5), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 4), new Vector2<int>(3, 1)),
                new Area(new Vector2<int>(4, 3), new Vector2<int>(3, 1)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 5), new Vector2<int>(3, 3))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3),
                new(4, 4), new(5, 4), new(6, 4),
                new(4, 5), new(5, 5), new(6, 5),
                new(4, 6), new(5, 6), new(6, 6),
                new(4, 7), new(5, 7), new(6, 7),

                new(3, 3), new(3, 4), new(3, 5), new(3, 6), new(3, 7),
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(6, 3),
                new(6, 4),
                new(6, 5),
                new(6, 6),
                new(4, 7), new(5, 7), new(6, 7),
            },
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3),
                new(4, 4), new(5, 4),
                new(4, 5), new(5, 5),
                new(4, 6), new(5, 6),
            }
        ];

        yield return
        [
            "N14", // Scenario
            new PathfindingSize(2),
            new Vector2<int>(3, 3), new Vector2<int>(5, 3), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(6, 3), new Vector2<int>(1, 3)),
                new Area(new Vector2<int>(7, 3), new Vector2<int>(1, 3)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(3, 3), new Vector2<int>(3, 3))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5),

                new(2, 2), new(3, 2), new(4, 2), new(5, 2), new(6, 2),
                new(2, 3), new(2, 4), new(2, 5),
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(3, 5), new(4, 5), new(5, 5), new(6, 5),
            },
            new Vector2<int>[] // Free high ground
            {
                new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(7, 5),
            }
        ];

        yield return
        [
            "Z14", // Scenario
            new PathfindingSize(2),
            new Vector2<int>(4, 3), new Vector2<int>(3, 5), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 6), new Vector2<int>(3, 1)),
                new Area(new Vector2<int>(4, 7), new Vector2<int>(3, 1)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(3, 3))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3),
                new(4, 4), new(5, 4), new(6, 4),
                new(4, 5), new(5, 5), new(6, 5),
                new(4, 6), new(5, 6), new(6, 6),
                new(4, 7), new(5, 7), new(6, 7),

                new(3, 2), new(4, 2), new(5, 2), new(6, 2),
                new(3, 3), new(3, 4), new(3, 5), new(3, 6),
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(6, 3), new(6, 4), new(6, 5), new(6, 6),
            },
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3),
                new(4, 4), new(5, 4),
                new(4, 5), new(5, 5),
                new(4, 6), new(5, 6),
                new(4, 7), new(5, 7), new(6, 7),
            }
        ];

        yield return
        [
            "AL14", // Scenario
            new PathfindingSize(2),
            new Vector2<int>(3, 3), new Vector2<int>(5, 3), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(1, 3)),
                new Area(new Vector2<int>(3, 3), new Vector2<int>(1, 3)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(5, 3), new Vector2<int>(3, 3))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5),

                new(3, 2), new(4, 2), new(5, 2), new(6, 2), new(7, 2),
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(7, 3), new(7, 4),
                new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5),
            },
            new Vector2<int>[] // Free high ground
            {
                new(3, 3), new(4, 3), new(5, 3), new(6, 3),
                new(3, 4), new(4, 4), new(5, 4), new(6, 4),
            }
        ];

        yield return
        [
            "B26", // Scenario
            new PathfindingSize(3),
            new Vector2<int>(4, 3), new Vector2<int>(3, 5), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 4), new Vector2<int>(3, 1)),
                new Area(new Vector2<int>(4, 3), new Vector2<int>(3, 1)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 5), new Vector2<int>(3, 3))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3),
                new(4, 4), new(5, 4), new(6, 4),
                new(4, 5), new(5, 5), new(6, 5),
                new(4, 6), new(5, 6), new(6, 6),
                new(4, 7), new(5, 7), new(6, 7),

                new(2, 2), new(3, 2), new(5, 2), new(6, 2),
                new(2, 3), new(3, 3),
                new(2, 4), new(3, 4),
                new(2, 5), new(3, 5),
                new(2, 6), new(3, 6),
                new(2, 7), new(3, 7),
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(5, 3), new(6, 3),
                new(5, 4), new(6, 4),
                new(5, 5), new(6, 5),
                new(4, 6), new(5, 6), new(6, 6),
                new(4, 7), new(5, 7), new(6, 7),
            },
            new Vector2<int>[] // Free high ground
            {
                new(4, 3),
                new(4, 4),
                new(4, 5),
            }
        ];

        yield return
        [
            "N26", // Scenario
            new PathfindingSize(3),
            new Vector2<int>(3, 3), new Vector2<int>(5, 3), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(6, 3), new Vector2<int>(1, 3)),
                new Area(new Vector2<int>(7, 3), new Vector2<int>(1, 3)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(3, 3), new Vector2<int>(3, 3))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5),

                new(1, 1), new(2, 1), new(3, 1), new(4, 1), new(5, 1), new(6, 1),
                new(1, 2), new(2, 2), new(3, 2), new(4, 2), new(5, 2), new(6, 2),
                new(1, 3), new(2, 3),
                new(1, 4), new(2, 4),
                new(1, 5), new(2, 5),
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(3, 4), new(4, 4), new(5, 4), new(6, 4),
                new(3, 5), new(4, 5), new(5, 5), new(6, 5),
            },
            new Vector2<int>[] // Free high ground
            {
                new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(7, 4),
                new(7, 5),
            }
        ];

        yield return
        [
            "Z26", // Scenario
            new PathfindingSize(3),
            new Vector2<int>(4, 3), new Vector2<int>(3, 5), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 6), new Vector2<int>(3, 1)),
                new Area(new Vector2<int>(4, 7), new Vector2<int>(3, 1)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(3, 3))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3),
                new(4, 4), new(5, 4), new(6, 4),
                new(4, 5), new(5, 5), new(6, 5),
                new(4, 6), new(5, 6), new(6, 6),
                new(4, 7), new(5, 7), new(6, 7),

                new(2, 1), new(3, 1), new(4, 1), new(5, 1), new(6, 1),
                new(2, 2), new(3, 2), new(4, 2), new(5, 2), new(6, 2),
                new(2, 3), new(3, 3),
                new(2, 4), new(3, 4),
                new(2, 5), new(3, 5),
                new(2, 6), new(3, 6),
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(5, 3), new(6, 3),
                new(5, 4), new(6, 4),
                new(5, 5), new(6, 5),
                new(5, 6), new(6, 6),
            },
            new Vector2<int>[] // Free high ground
            {
                new(4, 3),
                new(4, 4),
                new(4, 5),
                new(4, 6),
                new(4, 7), new(5, 7), new(6, 7),
            }
        ];

        yield return
        [
            "AL26", // Scenario
            new PathfindingSize(3),
            new Vector2<int>(3, 3), new Vector2<int>(5, 3), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(1, 3)),
                new Area(new Vector2<int>(3, 3), new Vector2<int>(1, 3)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(5, 3), new Vector2<int>(3, 3))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5),

                new(2, 1), new(3, 1), new(4, 1), new(5, 1), new(6, 1), new(7, 1),
                new(2, 2), new(3, 2), new(4, 2), new(5, 2), new(6, 2), new(7, 2),
                new(2, 4), new(2, 5),
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(6, 3), new(7, 3),
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5),
            },
            new Vector2<int>[] // Free high ground
            {
                new(3, 3), new(4, 3), new(5, 3),
            }
        ];
    }

    [Theory]
    [MemberData(nameof(GetExpectedOccupationAndHighGroundForLongStairs))]
    public void Pathfinding_ShouldCalculateHighGroundAndOccupation_ForLongStairs(
        string scenario,
        PathfindingSize pathfindingSize,
        Vector2<int> entityPrimaryPosition, Vector2<int> entitySize,
        Area[] ascendables,
        Area[] highGrounds,
        Vector2<int>[] expectedOccupiedLowGroundPositions,
        Vector2<int>[] expectedOccupiedHighGroundPositions,
        Vector2<int>[] expectedFreeHighGroundPositions)
    {
        SetupAddingEntityInstance(entityPrimaryPosition, entitySize, Team.Default, false, null,
            ascendables.Select(x => x.ToVectors()).ToList(),
            highGrounds.SelectMany(x => x.ToVectors()).ToHashSet());

        var availablePoints = GetAvailablePointsFor(pathfindingSize);

        ExpectedPositionsShouldNotBeContainedIn(availablePoints,
            expectedOccupiedLowGroundPositions, CheckElevation.LowGround,
            scenario);

        ExpectedPositionsShouldNotBeContainedIn(availablePoints,
            expectedOccupiedHighGroundPositions, CheckElevation.HighGround,
            scenario);
        
        ExpectedHighGroundPositionsShouldBeAccessibleIn(availablePoints, 
            expectedFreeHighGroundPositions, scenario);
    }

    #endregion Complex Scenarios: 1. Long Stairs

    #region Complex Scenarios: 2. Gates

    public static IEnumerable<object[]> GetExpectedOccupationAndHighGroundForGates()
    {
        yield return
        [
            "B38", // Scenario
            new PathfindingSize(1),
            new Vector2<int>(4, 3), new Vector2<int>(4, 1), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(5, 3), new Vector2<int>(2, 1))
            },
            new[] // High ground
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
            "N38", // Scenario
            new PathfindingSize(1),
            new Vector2<int>(4, 3), new Vector2<int>(1, 4), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 4), new Vector2<int>(1, 2))
            },
            new[] // High ground
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
            "B50", // Scenario
            new PathfindingSize(2),
            new Vector2<int>(4, 3), new Vector2<int>(4, 1), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(5, 3), new Vector2<int>(2, 1))
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(4, 1))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(3, 2), new(4, 2), new(6, 2), new(7, 2),
                new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(7, 3),
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(4, 3), new(6, 3), new(7, 3),
            },
            new Vector2<int>[] // Free high ground
            {
                new(5, 3),
            }
        ];

        yield return
        [
            "N50", // Scenario
            new PathfindingSize(2),
            new Vector2<int>(4, 3), new Vector2<int>(1, 4), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 4), new Vector2<int>(1, 2))
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(1, 4))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(3, 2), new(4, 2),
                new(3, 3), new(4, 3),
                new(4, 4),
                new(3, 5), new(4, 5),
                new(3, 6), new(4, 6),
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(4, 3),
                new(4, 5),
                new(4, 6),
            },
            new Vector2<int>[] // Free high ground
            {
                new(4, 4),
            }
        ];

        yield return
        [
            "B62", // Scenario
            new PathfindingSize(3),
            new Vector2<int>(4, 3), new Vector2<int>(4, 1), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(5, 3), new Vector2<int>(2, 1))
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(4, 1))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(2, 1), new(3, 1), new(4, 1), new(5, 1), new(6, 1), new(7, 1),
                new(2, 2), new(3, 2), new(4, 2), new(5, 2), new(6, 2), new(7, 2),
                new(2, 3), new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(7, 3),
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3)
            },
            Array.Empty<Vector2<int>>() // Free high ground
        ];

        yield return
        [
            "N62", // Scenario
            new PathfindingSize(3),
            new Vector2<int>(4, 3), new Vector2<int>(1, 4), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 4), new Vector2<int>(1, 2))
            },
            new[] // High ground
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
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
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
        string scenario,
        PathfindingSize pathfindingSize,
        Vector2<int> entityPrimaryPosition, Vector2<int> entitySize,
        Area[] ascendables,
        Area[] highGrounds,
        Vector2<int>[] expectedOccupiedLowGroundPositions,
        Vector2<int>[] expectedOccupiedHighGroundPositions,
        Vector2<int>[] expectedFreeHighGroundPositions)
    {
        SetupAddingEntityInstance(entityPrimaryPosition, entitySize, Team.Default, false, null,
            ascendables.Select(x => x.ToVectors()).ToList(),
            highGrounds.SelectMany(x => x.ToVectors()).ToHashSet());

        var availablePoints = GetAvailablePointsFor(pathfindingSize);

        ExpectedPositionsShouldNotBeContainedIn(availablePoints,
            expectedOccupiedLowGroundPositions, CheckElevation.LowGround,
            scenario);

        ExpectedPositionsShouldNotBeContainedIn(availablePoints,
            expectedOccupiedHighGroundPositions, CheckElevation.HighGround,
            scenario);
        
        ExpectedHighGroundPositionsShouldBeAccessibleIn(availablePoints, 
            expectedFreeHighGroundPositions, scenario);
    }

    #endregion Complex Scenarios: 2. Gates

    #region Complex Scenarios: 3. Enclosed Base

    public static IEnumerable<object[]> GetExpectedOccupationAndHighGroundForEnclosedBase()
    {
        yield return
        [
            "B74", // Scenario
            new PathfindingSize(1),
            new Vector2<int>(4, 3), new Vector2<int>(5, 4), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(5, 3), new Vector2<int>(3, 1)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(5, 4))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3), new(8, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4), new(8, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5), new(8, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6), new(8, 6),
            },
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3), new(8, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4), new(8, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5), new(8, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6), new(8, 6),
            }
        ];

        yield return
        [
            "N74", // Scenario
            new PathfindingSize(1),
            new Vector2<int>(4, 3), new Vector2<int>(4, 5), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(7, 4), new Vector2<int>(1, 3)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(4, 5))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6),
                new(4, 7), new(5, 7), new(6, 7), new(7, 7),
            },
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6),
                new(4, 7), new(5, 7), new(6, 7), new(7, 7),
            }
        ];

        yield return
        [
            "Z74", // Scenario
            new PathfindingSize(1),
            new Vector2<int>(4, 3), new Vector2<int>(5, 4), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(5, 6), new Vector2<int>(3, 1)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(5, 4))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3), new(8, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4), new(8, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5), new(8, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6), new(8, 6),
            },
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3), new(8, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4), new(8, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5), new(8, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6), new(8, 6),
            }
        ];

        yield return
        [
            "AL74", // Scenario
            new PathfindingSize(1),
            new Vector2<int>(4, 3), new Vector2<int>(4, 5), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 4), new Vector2<int>(1, 3)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(4, 5))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6),
                new(4, 7), new(5, 7), new(6, 7), new(7, 7),
            },
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6),
                new(4, 7), new(5, 7), new(6, 7), new(7, 7),
            }
        ];

        yield return
        [
            "B86", // Scenario
            new PathfindingSize(2),
            new Vector2<int>(4, 3), new Vector2<int>(5, 4), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(5, 3), new Vector2<int>(3, 1)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(5, 4))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3), new(8, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4), new(8, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5), new(8, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6), new(8, 6),

                new(3, 2), new(4, 2), new(7, 2), new(8, 2),
                new(3, 3), new(3, 4), new(3, 5), new(3, 6),
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(8, 3), new(8, 4), new(8, 5), new(8, 6),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6),
            },
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5),
            }
        ];

        yield return
        [
            "N86", // Scenario
            new PathfindingSize(2),
            new Vector2<int>(4, 3), new Vector2<int>(4, 5), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(7, 4), new Vector2<int>(1, 3)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(4, 5))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6),
                new(4, 7), new(5, 7), new(6, 7), new(7, 7),

                new(3, 2), new(4, 2), new(5, 2), new(6, 2), new(7, 2),
                new(3, 3), new(3, 4), new(3, 5), new(3, 6), new(3, 7),
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(7, 3),
                new(7, 6),
                new(4, 7), new(5, 7), new(6, 7), new(7, 7),
            },
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3), new(6, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5),
                new(4, 6), new(5, 6), new(6, 6),
            }
        ];

        yield return
        [
            "Z86", // Scenario
            new PathfindingSize(2),
            new Vector2<int>(4, 3), new Vector2<int>(5, 4), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(5, 6), new Vector2<int>(3, 1)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(5, 4))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3), new(8, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4), new(8, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5), new(8, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6), new(8, 6),

                new(3, 2), new(4, 2), new(5, 2), new(6, 2), new(7, 2), new(8, 2),
                new(3, 3), new(3, 4), new(3, 5), new(3, 6),
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(8, 3), new(8, 4), new(8, 5),
                new(4, 6), new(7, 6), new(8, 6),
            },
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5),
                new(5, 6), new(6, 6),
            }
        ];

        yield return
        [
            "AL86", // Scenario
            new PathfindingSize(2),
            new Vector2<int>(4, 3), new Vector2<int>(4, 5), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 4), new Vector2<int>(1, 3)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(4, 5))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6),
                new(4, 7), new(5, 7), new(6, 7), new(7, 7),

                new(3, 2), new(4, 2), new(5, 2), new(6, 2), new(7, 2),
                new(3, 3), new(3, 6), new(3, 7),
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(7, 3), new(7, 4), new(7, 5), new(7, 6), new(7, 7),
                new(4, 7), new(5, 7), new(6, 7),
            },
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3), new(6, 3),
                new(4, 4), new(5, 4), new(6, 4),
                new(4, 5), new(5, 5), new(6, 5),
                new(4, 6), new(5, 6), new(6, 6),
            }
        ];

        yield return
        [
            "B98", // Scenario
            new PathfindingSize(3),
            new Vector2<int>(4, 3), new Vector2<int>(5, 4), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(5, 3), new Vector2<int>(3, 1)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(5, 4))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3), new(8, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4), new(8, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5), new(8, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6), new(8, 6),

                new(2, 1), new(3, 1), new(4, 1), new(6, 1), new(7, 1),
                new(2, 2), new(3, 2), new(4, 2), new(6, 2), new(7, 2),
                new(2, 3), new(3, 3),
                new(2, 4), new(3, 4),
                new(2, 5), new(3, 5),
                new(2, 6), new(3, 6),
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(7, 3), new(8, 3),
                new(7, 4), new(8, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5), new(8, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6), new(8, 6),
            },
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3), new(6, 3),
                new(4, 4), new(5, 4), new(6, 4),
            }
        ];

        yield return
        [
            "N98", // Scenario
            new PathfindingSize(3),
            new Vector2<int>(3, 3), new Vector2<int>(4, 5), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(6, 4), new Vector2<int>(1, 3)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(3, 3), new Vector2<int>(4, 5))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(3, 3), new(4, 3), new(5, 3), new(6, 3),
                new(3, 4), new(4, 4), new(5, 4), new(6, 4),
                new(3, 5), new(4, 5), new(5, 5), new(6, 5),
                new(3, 6), new(4, 6), new(5, 6), new(6, 6),
                new(3, 7), new(4, 7), new(5, 7), new(6, 7),

                new(1, 1), new(2, 1), new(3, 1), new(4, 1), new(5, 1), new(6, 1),
                new(1, 2), new(2, 2), new(3, 2), new(4, 2), new(5, 2), new(6, 2),
                new(1, 3), new(2, 3),
                new(1, 4), new(2, 4),
                new(1, 5), new(2, 5),
                new(1, 6), new(2, 6),
                new(1, 7), new(2, 7),
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(5, 3), new(6, 3),
                new(5, 5), new(6, 5),
                new(3, 6), new(4, 6), new(5, 6), new(6, 6),
                new(3, 7), new(4, 7), new(5, 7), new(6, 7),
            },
            new Vector2<int>[] // Free high ground
            {
                new(3, 3), new(4, 3),
                new(3, 4), new(4, 4), new(5, 4), new(6, 4),
                new(3, 5), new(4, 5),
            }
        ];

        yield return
        [
            "Z98", // Scenario
            new PathfindingSize(3),
            new Vector2<int>(4, 3), new Vector2<int>(5, 4), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(5, 6), new Vector2<int>(3, 1)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(5, 4))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3), new(8, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4), new(8, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5), new(8, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6), new(8, 6),

                new(2, 1), new(3, 1), new(4, 1), new(5, 1), new(6, 1), new(7, 1),
                new(2, 2), new(3, 2), new(4, 2), new(5, 2), new(6, 2), new(7, 2),
                new(2, 3), new(3, 3),
                new(2, 4), new(3, 4),
                new(2, 5), new(3, 5),
                new(2, 6), new(3, 6),
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(7, 3), new(8, 3),
                new(7, 4), new(8, 4),
                new(4, 5), new(6, 5), new(7, 5), new(8, 5),
                new(4, 6), new(6, 6), new(7, 6), new(8, 6),
            },
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3), new(6, 3),
                new(4, 4), new(5, 4), new(6, 4),
                new(5, 5),
                new(5, 6),
            }
        ];

        yield return
        [
            "AL98", // Scenario
            new PathfindingSize(3),
            new Vector2<int>(4, 3), new Vector2<int>(4, 5), // Position & size
            new[] // Ascendable
            {
                new Area(new Vector2<int>(4, 4), new Vector2<int>(1, 3)),
            },
            new[] // High ground
            {
                new Area(new Vector2<int>(4, 3), new Vector2<int>(4, 5))
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 3), new(5, 3), new(6, 3), new(7, 3),
                new(4, 4), new(5, 4), new(6, 4), new(7, 4),
                new(4, 5), new(5, 5), new(6, 5), new(7, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6),
                new(4, 7), new(5, 7), new(6, 7), new(7, 7),

                new(2, 1), new(3, 1), new(4, 1), new(5, 1), new(6, 1), new(7, 1),
                new(2, 2), new(3, 2), new(4, 2), new(5, 2), new(6, 2), new(7, 2),
                new(2, 3), new(3, 3),
                new(2, 5), new(3, 5),
                new(2, 6), new(3, 6),
                new(2, 7), new(3, 7),
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(6, 3), new(7, 3),
                new(6, 4), new(7, 4),
                new(6, 5), new(7, 5),
                new(4, 6), new(5, 6), new(6, 6), new(7, 6),
                new(4, 7), new(5, 7), new(6, 7), new(7, 7),
            },
            new Vector2<int>[] // Free high ground
            {
                new(4, 3), new(5, 3),
                new(4, 4), new(5, 4),
                new(4, 5), new(5, 5),
            }
        ];
    }

    [Theory]
    [MemberData(nameof(GetExpectedOccupationAndHighGroundForEnclosedBase))]
    public void Pathfinding_ShouldCalculateHighGroundAndOccupation_ForEnclosedBase(
        string scenario,
        PathfindingSize pathfindingSize,
        Vector2<int> entityPrimaryPosition, Vector2<int> entitySize,
        Area[] ascendables,
        Area[] highGrounds,
        Vector2<int>[] expectedOccupiedLowGroundPositions,
        Vector2<int>[] expectedOccupiedHighGroundPositions,
        Vector2<int>[] expectedFreeHighGroundPositions)
    {
        SetupAddingEntityInstance(entityPrimaryPosition, entitySize, Team.Default, false, null,
            ascendables.Select(x => x.ToVectors()).ToList(),
            highGrounds.SelectMany(x => x.ToVectors()).ToHashSet());

        var availablePoints = GetAvailablePointsFor(pathfindingSize);
        
        ExpectedPositionsShouldNotBeContainedIn(availablePoints,
            expectedOccupiedLowGroundPositions, CheckElevation.LowGround,
            scenario);

        ExpectedPositionsShouldNotBeContainedIn(availablePoints,
            expectedOccupiedHighGroundPositions, CheckElevation.HighGround,
            scenario);
        
        ExpectedHighGroundPositionsShouldBeAccessibleIn(availablePoints, 
            expectedFreeHighGroundPositions, scenario);
    }

    #endregion Complex Scenarios: 3. Enclosed Base

    #region Complex Scenarios: 4. Diagonals

    public static IEnumerable<object[]> GetExpectedConnectionsForDiagonals()
    {
        yield return
        [
            "B110", // Scenario
            new PathfindingSize(1),
            new (Vector2<int>, Vector2<int>, Area[], Area[])[]
            {
                (
                    new Vector2<int>(7, 2),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(8, 3),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(3, 4),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(4, 5),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(6, 7),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(5, 8),
                    new Vector2<int>(1, 1), [], []
                ),
            },
            // Expected:
            new[] // Non connected diagonal pairs
            {
                ((new Vector2<int>(8, 2), false), (new Vector2<int>(7, 3), false)), 
                ((new Vector2<int>(4, 4), false), (new Vector2<int>(3, 5), false)), 
                ((new Vector2<int>(5, 7), false), (new Vector2<int>(6, 8), false)), 
            },
            new[] // Connected diagonal pairs
            {
                ((new Vector2<int>(7, 1), false), (new Vector2<int>(6, 2), false)), 
            },
        ];
        
        yield return
        [
            "B122", // Scenario
            new PathfindingSize(2),
            new (Vector2<int>, Vector2<int>, Area[], Area[])[]
            {
                (
                    new Vector2<int>(7, 2),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(8, 3),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(3, 4),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(4, 5),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(6, 7),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(5, 8),
                    new Vector2<int>(1, 1), [], []
                ),
            },
            // Expected:
            new[] // Non connected diagonal pairs
            {
                ((new Vector2<int>(5, 5), false), (new Vector2<int>(4, 6), false)), 
            },
            new[] // Connected diagonal pairs
            {
                ((new Vector2<int>(3, 2), false), (new Vector2<int>(4, 3), false)), 
            },
        ];
        
        yield return
        [
            "B134", // Scenario
            new PathfindingSize(3),
            new (Vector2<int>, Vector2<int>, Area[], Area[])[]
            {
                (
                    new Vector2<int>(7, 2),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(8, 3),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(3, 4),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(4, 5),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(6, 7),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(5, 8),
                    new Vector2<int>(1, 1), [], []
                ),
            },
            // Expected:
            new[] // Non connected diagonal pairs
            {
                ((new Vector2<int>(4, 2), false), (new Vector2<int>(5, 3), false)), 
            },
            new[] // Connected diagonal pairs
            {
                ((new Vector2<int>(3, 1), false), (new Vector2<int>(4, 2), false)), 
            },
        ];

        yield return
        [
            "N110", // Scenario
            new PathfindingSize(1),
            new (Vector2<int>, Vector2<int>, Area[], Area[])[]
            {
                (
                    new Vector2<int>(2, 2),
                    new Vector2<int>(3, 4),
                    [
                        new Area(new Vector2<int>(2, 2), new Vector2<int>(3, 1))
                    ],
                    [
                        new Area(new Vector2<int>(2, 2), new Vector2<int>(3, 4))
                    ]
                ),
                (
                    new Vector2<int>(3, 4),
                    new Vector2<int>(3, 3),
                    [],
                    [
                        new Area(new Vector2<int>(3, 4), new Vector2<int>(3, 3))
                    ]
                ),
                (
                    new Vector2<int>(5, 6),
                    new Vector2<int>(2, 2),
                    [],
                    [
                        new Area(new Vector2<int>(5, 6), new Vector2<int>(2, 2))
                    ]
                ),
                (
                    new Vector2<int>(4, 8),
                    new Vector2<int>(1, 1),
                    [],
                    [
                        new Area(new Vector2<int>(4, 8), new Vector2<int>(1, 1))
                    ]
                ),
            },
            // Expected:
            new[] // Non connected diagonal pairs
            {
                ((new Vector2<int>(5, 7), true), (new Vector2<int>(4, 8), true)), 
            },
            new[] // Connected diagonal pairs
            {
                ((new Vector2<int>(4, 6), true), (new Vector2<int>(5, 7), true)), 
                ((new Vector2<int>(1, 1), false), (new Vector2<int>(2, 2), true)), 
            }
        ];
        
        yield return
        [
            "N122", // Scenario
            new PathfindingSize(2),
            new (Vector2<int>, Vector2<int>, Area[], Area[])[]
            {
                (
                    new Vector2<int>(2, 2),
                    new Vector2<int>(3, 4),
                    [
                        new Area(new Vector2<int>(2, 2), new Vector2<int>(3, 1))
                    ],
                    [
                        new Area(new Vector2<int>(2, 2), new Vector2<int>(3, 4))
                    ]
                ),
                (
                    new Vector2<int>(3, 4),
                    new Vector2<int>(3, 3),
                    [],
                    [
                        new Area(new Vector2<int>(3, 4), new Vector2<int>(3, 3))
                    ]
                ),
                (
                    new Vector2<int>(5, 6),
                    new Vector2<int>(2, 2),
                    [],
                    [
                        new Area(new Vector2<int>(5, 6), new Vector2<int>(2, 2))
                    ]
                ),
                (
                    new Vector2<int>(4, 8),
                    new Vector2<int>(1, 1),
                    [],
                    [
                        new Area(new Vector2<int>(4, 8), new Vector2<int>(1, 1))
                    ]
                ),
            },
            // Expected:
            new[] // Non connected diagonal pairs
            {
                ((new Vector2<int>(4, 5), true), (new Vector2<int>(5, 6), true)), 
            },
            new[] // Connected diagonal pairs
            {
                ((new Vector2<int>(2, 4), true), (new Vector2<int>(3, 5), true)), 
                ((new Vector2<int>(1, 1), false), (new Vector2<int>(2, 2), true)), 
            }
        ];
        
        yield return
        [
            "N134", // Scenario
            new PathfindingSize(3),
            new (Vector2<int>, Vector2<int>, Area[], Area[])[]
            {
                (
                    new Vector2<int>(2, 2),
                    new Vector2<int>(3, 4),
                    [
                        new Area(new Vector2<int>(2, 2), new Vector2<int>(3, 1))
                    ],
                    [
                        new Area(new Vector2<int>(2, 2), new Vector2<int>(3, 4))
                    ]
                ),
                (
                    new Vector2<int>(3, 4),
                    new Vector2<int>(3, 3),
                    [],
                    [
                        new Area(new Vector2<int>(3, 4), new Vector2<int>(3, 3))
                    ]
                ),
                (
                    new Vector2<int>(5, 6),
                    new Vector2<int>(2, 2),
                    [],
                    [
                        new Area(new Vector2<int>(5, 6), new Vector2<int>(2, 2))
                    ]
                ),
                (
                    new Vector2<int>(4, 8),
                    new Vector2<int>(1, 1),
                    [],
                    [
                        new Area(new Vector2<int>(4, 8), new Vector2<int>(1, 1))
                    ]
                ),
            },
            // Expected:
            new[] // Non connected diagonal pairs
            {
                ((new Vector2<int>(2, 3), true), (new Vector2<int>(3, 4), true)), 
            },
            new[] // Connected diagonal pairs
            {
                ((new Vector2<int>(2, 2), true), (new Vector2<int>(2, 3), true)), 
                ((new Vector2<int>(1, 0), false), (new Vector2<int>(2, 1), false)), 
            }
        ];
        
        yield return
        [
            "Z110", // Scenario
            new PathfindingSize(1),
            new (Vector2<int>, Vector2<int>, Area[], Area[])[]
            {
                (
                    new Vector2<int>(1, 1),
                    new Vector2<int>(1, 1),
                    [
                        new Area(new Vector2<int>(1, 1), new Vector2<int>(1, 1))
                    ],
                    []
                ),
                (
                    new Vector2<int>(2, 2),
                    new Vector2<int>(1, 1),
                    [
                        new Area(new Vector2<int>(2, 2), new Vector2<int>(1, 1))
                    ],
                    []
                ),
                
                (
                    new Vector2<int>(5, 1),
                    new Vector2<int>(1, 1),
                    [
                        new Area(new Vector2<int>(5, 1), new Vector2<int>(1, 1))
                    ],
                    []
                ),
                (
                    new Vector2<int>(4, 2),
                    new Vector2<int>(1, 1),
                    [
                        new Area(new Vector2<int>(4, 2), new Vector2<int>(1, 1))
                    ],
                    []
                ),
                
                (
                    new Vector2<int>(1, 4),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(2, 5),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(2, 4),
                    new Vector2<int>(1, 1),
                    [
                        new Area(new Vector2<int>(2, 4), new Vector2<int>(1, 1))
                    ],
                    []
                ),
                
                (
                    new Vector2<int>(4, 4),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(5, 5),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(4, 5),
                    new Vector2<int>(1, 1),
                    [
                        new Area(new Vector2<int>(4, 5), new Vector2<int>(1, 1))
                    ],
                    []
                ),
                
                (
                    new Vector2<int>(7, 4),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(8, 5),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(8, 4),
                    new Vector2<int>(1, 1),
                    [
                        new Area(new Vector2<int>(8, 4), new Vector2<int>(1, 1))
                    ],
                    []
                ),
                (
                    new Vector2<int>(7, 5),
                    new Vector2<int>(1, 1),
                    [
                        new Area(new Vector2<int>(7, 5), new Vector2<int>(1, 1))
                    ],
                    []
                ),
                
                (
                    new Vector2<int>(2, 7),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(1, 8),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(1, 7),
                    new Vector2<int>(1, 1),
                    [
                        new Area(new Vector2<int>(1, 7), new Vector2<int>(1, 1))
                    ],
                    []
                ),
                
                (
                    new Vector2<int>(5, 7),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(4, 8),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(5, 8),
                    new Vector2<int>(1, 1),
                    [
                        new Area(new Vector2<int>(5, 8), new Vector2<int>(1, 1))
                    ],
                    []
                ),
                
                (
                    new Vector2<int>(8, 7),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(7, 8),
                    new Vector2<int>(1, 1), [], []
                ),
                (
                    new Vector2<int>(7, 7),
                    new Vector2<int>(1, 1),
                    [
                        new Area(new Vector2<int>(7, 7), new Vector2<int>(1, 1))
                    ],
                    []
                ),
                (
                    new Vector2<int>(8, 8),
                    new Vector2<int>(1, 1),
                    [
                        new Area(new Vector2<int>(8, 8), new Vector2<int>(1, 1))
                    ],
                    []
                ),
            },
            // Expected:
            new[] // Non connected diagonal pairs
            {
                ((new Vector2<int>(2, 4), true), (new Vector2<int>(1, 5), false)), 
                ((new Vector2<int>(5, 4), false), (new Vector2<int>(4, 5), true)), 
                ((new Vector2<int>(8, 4), true), (new Vector2<int>(7, 5), true)), 
                ((new Vector2<int>(1, 7), true), (new Vector2<int>(2, 8), false)), 
                ((new Vector2<int>(4, 7), false), (new Vector2<int>(5, 8), true)), 
                ((new Vector2<int>(7, 7), true), (new Vector2<int>(8, 8), true)), 
            },
            new[] // Connected diagonal pairs
            {
                ((new Vector2<int>(1, 1), true), (new Vector2<int>(2, 2), true)), 
                ((new Vector2<int>(5, 1), true), (new Vector2<int>(4, 2), true)), 
                ((new Vector2<int>(2, 1), false), (new Vector2<int>(1, 2), false)), 
                ((new Vector2<int>(4, 1), false), (new Vector2<int>(5, 2), false)), 
            }
        ];
    }

    [Theory]
    [MemberData(nameof(GetExpectedConnectionsForDiagonals))]
    public void Pathfinding_ShouldCalculateConnections_ForDiagonals(
        string scenario,
        PathfindingSize pathfindingSize,
        (Vector2<int>, Vector2<int>, Area[], Area[])[] entities,
        ((Vector2<int>, bool), (Vector2<int>, bool))[] expectedNonConnectedDiagonalPairs,
        ((Vector2<int>, bool), (Vector2<int>, bool))[] expectedConnectedDiagonalPairs)
    {
        foreach (var (pos, size, ascendables, highGrounds) in entities)
        {
            SetupAddingEntityInstance(pos, size, Team.Default, false, null,
                ascendables.Select(x => x.ToVectors()).ToList(),
                highGrounds.SelectMany(x => x.ToVectors()).ToHashSet());
        }

        GetAvailablePointsFor(pathfindingSize);
        
        ExpectedPositionsShouldBeConnected(pathfindingSize, expectedNonConnectedDiagonalPairs, 
            expectedConnectedDiagonalPairs, scenario);
    }

    #endregion Complex Scenarios: 4. Diagonals

    #region Complex Scenarios: 5. Complex Stairs
    
    public static IEnumerable<object[]> GetExpectedOccupationAndHighGroundForComplexStairs()
    {
        yield return
        [
            "B146", // Scenario
            new PathfindingSize(1),
            new (Vector2<int>, Vector2<int>, Team, Area[], Area[])[] // Position, size, team, ascendables, high grounds
            {
                (
                    new Vector2<int>(5, 2),
                    new Vector2<int>(1, 7),
                    Team.Default, 
                    [
                        new Area(new Vector2<int>(5, 3), new Vector2<int>(1, 5)), 
                        new Area(new Vector2<int>(5, 2), new Vector2<int>(1, 7)), 
                    ],
                    [
                        new Area(new Vector2<int>(5, 5), new Vector2<int>(1, 1)), 
                    ]
                ),
                (
                    new Vector2<int>(4, 3),
                    new Vector2<int>(3, 5),
                    Team.Default, 
                    [
                        new Area(new Vector2<int>(4, 4), new Vector2<int>(3, 3)), 
                        new Area(new Vector2<int>(4, 3), new Vector2<int>(3, 5)), 
                    ], []
                ),
                (
                    new Vector2<int>(3, 4),
                    new Vector2<int>(5, 3),
                    Team.Default, 
                    [
                        new Area(new Vector2<int>(4, 4), new Vector2<int>(3, 3)), 
                        new Area(new Vector2<int>(3, 4), new Vector2<int>(5, 3)), 
                    ], []
                ),
                (
                    new Vector2<int>(2, 5),
                    new Vector2<int>(7, 1),
                    Team.Default, 
                    [
                        new Area(new Vector2<int>(3, 5), new Vector2<int>(5, 1)), 
                        new Area(new Vector2<int>(2, 5), new Vector2<int>(7, 1)), 
                    ], []
                ),
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(5, 2), 
                new(4, 3), new(5, 3), new(6, 3), 
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4), 
                new(2, 5), new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5), new(8, 5), 
                new(3, 6), new(4, 6), new(5, 6), new(6, 6), new(7, 6), 
                new(4, 7), new(5, 7), new(6, 7), 
                new(5, 8), 
            },
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new Vector2<int>[] // Free high ground
            {
                new(5, 2), 
                new(4, 3), new(5, 3), new(6, 3), 
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4), 
                new(2, 5), new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5), new(8, 5), 
                new(3, 6), new(4, 6), new(5, 6), new(6, 6), new(7, 6), 
                new(4, 7), new(5, 7), new(6, 7), 
                new(5, 8), 
            },
            Array.Empty<((Vector2<int>, bool), (Vector2<int>, bool))>(), // Non connected position pairs,
            new[] // Connected position pairs
            {
                ((new Vector2<int>(1, 4), false), (new Vector2<int>(2, 5), true)), 
            },
        ];

        yield return
        [
            "B158", // Scenario
            new PathfindingSize(2),
            new (Vector2<int>, Vector2<int>, Team, Area[], Area[])[] // Position, size, team, ascendables, high grounds
            {
                (
                    new Vector2<int>(5, 2),
                    new Vector2<int>(1, 7),
                    Team.Default, 
                    [
                        new Area(new Vector2<int>(5, 3), new Vector2<int>(1, 5)), 
                        new Area(new Vector2<int>(5, 2), new Vector2<int>(1, 7)), 
                    ],
                    [
                        new Area(new Vector2<int>(5, 5), new Vector2<int>(1, 1)), 
                    ]
                ),
                (
                    new Vector2<int>(4, 3),
                    new Vector2<int>(3, 5),
                    Team.Default, 
                    [
                        new Area(new Vector2<int>(4, 4), new Vector2<int>(3, 3)), 
                        new Area(new Vector2<int>(4, 3), new Vector2<int>(3, 5)), 
                    ], []
                ),
                (
                    new Vector2<int>(3, 4),
                    new Vector2<int>(5, 3),
                    Team.Default, 
                    [
                        new Area(new Vector2<int>(4, 4), new Vector2<int>(3, 3)), 
                        new Area(new Vector2<int>(3, 4), new Vector2<int>(5, 3)), 
                    ], []
                ),
                (
                    new Vector2<int>(2, 5),
                    new Vector2<int>(7, 1),
                    Team.Default, 
                    [
                        new Area(new Vector2<int>(3, 5), new Vector2<int>(5, 1)), 
                        new Area(new Vector2<int>(2, 5), new Vector2<int>(7, 1)), 
                    ], []
                ),
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(5, 2), 
                new(4, 3), new(5, 3), new(6, 3), 
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4), 
                new(2, 5), new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5), new(8, 5), 
                new(3, 6), new(4, 6), new(5, 6), new(6, 6), new(7, 6), 
                new(4, 7), new(5, 7), new(6, 7), 
                new(5, 8), 
            }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new Vector2<int>[] // Free high ground
            {
                new(5, 2), 
                new(4, 3), new(5, 3), new(6, 3), 
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4), 
                new(2, 5), new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5), new(8, 5), 
                new(3, 6), new(4, 6), new(5, 6), new(6, 6), new(7, 6), 
                new(4, 7), new(5, 7), new(6, 7), 
                new(5, 8), 
            },
            Array.Empty<((Vector2<int>, bool), (Vector2<int>, bool))>(), // Non connected position pairs,
            new[] // Connected position pairs
            {
                ((new Vector2<int>(1, 4), false), (new Vector2<int>(2, 5), true)), 
            },
        ];
        
        yield return
        [
            "B170", // Scenario
            new PathfindingSize(3),
            new (Vector2<int>, Vector2<int>, Team, Area[], Area[])[] // Position, size, team, ascendables, high grounds
            {
                (
                    new Vector2<int>(5, 2),
                    new Vector2<int>(1, 7),
                    Team.Default, 
                    [
                        new Area(new Vector2<int>(5, 3), new Vector2<int>(1, 5)), 
                        new Area(new Vector2<int>(5, 2), new Vector2<int>(1, 7)), 
                    ],
                    [
                        new Area(new Vector2<int>(5, 5), new Vector2<int>(1, 1)), 
                    ]
                ),
                (
                    new Vector2<int>(4, 3),
                    new Vector2<int>(3, 5),
                    Team.Default, 
                    [
                        new Area(new Vector2<int>(4, 4), new Vector2<int>(3, 3)), 
                        new Area(new Vector2<int>(4, 3), new Vector2<int>(3, 5)), 
                    ], []
                ),
                (
                    new Vector2<int>(3, 4),
                    new Vector2<int>(5, 3),
                    Team.Default, 
                    [
                        new Area(new Vector2<int>(4, 4), new Vector2<int>(3, 3)), 
                        new Area(new Vector2<int>(3, 4), new Vector2<int>(5, 3)), 
                    ], []
                ),
                (
                    new Vector2<int>(2, 5),
                    new Vector2<int>(7, 1),
                    Team.Default, 
                    [
                        new Area(new Vector2<int>(3, 5), new Vector2<int>(5, 1)), 
                        new Area(new Vector2<int>(2, 5), new Vector2<int>(7, 1)), 
                    ], []
                ),
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(5, 2), 
                new(4, 3), new(5, 3), new(6, 3), 
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4), 
                new(2, 5), new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5), 
                new(3, 6), new(4, 6), new(5, 6), new(6, 6), new(7, 6), 
                new(4, 7), new(5, 7), new(6, 7), 
            }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
            new Vector2<int>[] // Occupation: high ground
            {
                new(8, 5), new(5, 8), 
            },
            new Vector2<int>[] // Free high ground
            {
                new(5, 2), 
                new(4, 3), new(5, 3), new(6, 3), 
                new(3, 4), new(4, 4), new(5, 4), new(6, 4), new(7, 4), 
                new(2, 5), new(3, 5), new(4, 5), new(5, 5), new(6, 5), new(7, 5), 
                new(3, 6), new(4, 6), new(5, 6), new(6, 6), new(7, 6), 
                new(4, 7), new(5, 7), new(6, 7), 
            },
            Array.Empty<((Vector2<int>, bool), (Vector2<int>, bool))>(), // Non connected position pairs,
            new[] // Connected position pairs
            {
                ((new Vector2<int>(1, 4), false), (new Vector2<int>(2, 5), true)), 
            },
        ];
        
        yield return
        [
            "N146", // Scenario
            new PathfindingSize(1),
            new (Vector2<int>, Vector2<int>, Team, Area[], Area[])[] // Position, size, team, ascendables, high grounds
            {
                (
                    new Vector2<int>(4, 2),
                    new Vector2<int>(4, 1),
                    Team.Default,
                    [
                        new Area(new Vector2<int>(5, 2), new Vector2<int>(1, 1)), 
                        new Area(new Vector2<int>(4, 2), new Vector2<int>(1, 1)), 
                    ],
                    [
                        new Area(new Vector2<int>(6, 2), new Vector2<int>(2, 1)), 
                    ]
                ),
                (
                    new Vector2<int>(7, 2),
                    new Vector2<int>(1, 5),
                    Team.Default,
                    [],
                    [
                        new Area(new Vector2<int>(7, 2), new Vector2<int>(1, 5)), 
                    ]
                ),
                (
                    new Vector2<int>(4, 6),
                    new Vector2<int>(4, 1),
                    Team.Default,
                    [
                        new Area(new Vector2<int>(5, 6), new Vector2<int>(1, 1)), 
                        new Area(new Vector2<int>(4, 6), new Vector2<int>(1, 1)), 
                    ],
                    [
                        new Area(new Vector2<int>(6, 6), new Vector2<int>(2, 1)), 
                    ]
                ),
                (
                    new Vector2<int>(2, 4),
                    new Vector2<int>(5, 1),
                    Team.Default, [], []
                ),
                (
                    new Vector2<int>(2, 4),
                    new Vector2<int>(1, 5),
                    Team.Default, [], []
                ),
                (
                    new Vector2<int>(2, 8),
                    new Vector2<int>(6, 1),
                    Team.Default, [], []
                ),
                (
                    new Vector2<int>(7, 7),
                    new Vector2<int>(1, 2),
                    Team.Default, [], []
                ),
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 2), new(5, 2), new(6, 2), new(7, 2), 
                new(7, 3), 
                new(7, 4), 
                new(7, 5), 
                new(4, 6), new(5, 6), new(6, 6), new(7, 6), 
                
                new(2, 4), new(3, 4), new(4, 4), new(5, 4), new(6, 4), 
                new(2, 5), 
                new(2, 6), 
                new(2, 7), new(7, 7), 
                new(2, 8), new(3, 8), new(4, 8), new(5, 8), new(6, 8), new(7, 8), 
            },
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new Vector2<int>[] // Free high ground
            {
                new(4, 2), new(5, 2), new(6, 2), new(7, 2), 
                new(7, 3), 
                new(7, 4), 
                new(7, 5), 
                new(4, 6), new(5, 6), new(6, 6), new(7, 6), 
            },
            new[] // Non connected position pairs
            {
                ((new Vector2<int>(4, 5), false), (new Vector2<int>(5, 6), true)), 
                ((new Vector2<int>(5, 5), false), (new Vector2<int>(5, 6), true)), 
                ((new Vector2<int>(6, 5), false), (new Vector2<int>(5, 6), true)), 
                ((new Vector2<int>(4, 7), false), (new Vector2<int>(5, 6), true)), 
                ((new Vector2<int>(5, 7), false), (new Vector2<int>(5, 6), true)), 
                ((new Vector2<int>(6, 7), false), (new Vector2<int>(5, 6), true)), 
            },
            new[] // Connected position pairs
            {
                ((new Vector2<int>(3, 5), false), (new Vector2<int>(4, 6), true)), 
                ((new Vector2<int>(4, 5), false), (new Vector2<int>(4, 6), true)), 
                ((new Vector2<int>(5, 5), false), (new Vector2<int>(4, 6), true)), 
                ((new Vector2<int>(3, 6), false), (new Vector2<int>(4, 6), true)), 
                ((new Vector2<int>(3, 7), false), (new Vector2<int>(4, 6), true)), 
                ((new Vector2<int>(4, 7), false), (new Vector2<int>(4, 6), true)), 
                ((new Vector2<int>(5, 7), false), (new Vector2<int>(4, 6), true)), 
            },
        ];
        
        yield return
        [
            "N158", // Scenario
            new PathfindingSize(1),
            new (Vector2<int>, Vector2<int>, Team, Area[], Area[])[] // Position, size, team, ascendables, high grounds
            {
                (
                    new Vector2<int>(4, 2),
                    new Vector2<int>(4, 1),
                    Team.Default, 
                    [
                        new Area(new Vector2<int>(5, 2), new Vector2<int>(1, 1)), 
                        new Area(new Vector2<int>(4, 2), new Vector2<int>(1, 1)), 
                    ],
                    [
                        new Area(new Vector2<int>(6, 2), new Vector2<int>(2, 1)), 
                    ]
                ),
                (
                    new Vector2<int>(7, 2),
                    new Vector2<int>(1, 5),
                    Team.Default, 
                    [],
                    [
                        new Area(new Vector2<int>(7, 2), new Vector2<int>(1, 5)), 
                    ]
                ),
                (
                    new Vector2<int>(4, 6),
                    new Vector2<int>(4, 1),
                    new Team(2),
                    [
                        new Area(new Vector2<int>(5, 6), new Vector2<int>(1, 1)), 
                        new Area(new Vector2<int>(4, 6), new Vector2<int>(1, 1)), 
                    ],
                    [
                        new Area(new Vector2<int>(6, 6), new Vector2<int>(2, 1)), 
                    ]
                ),
                (
                    new Vector2<int>(2, 4),
                    new Vector2<int>(5, 1),
                    Team.Default, [], []
                ),
                (
                    new Vector2<int>(2, 4),
                    new Vector2<int>(1, 5),
                    Team.Default, [], []
                ),
                (
                    new Vector2<int>(2, 8),
                    new Vector2<int>(6, 1),
                    Team.Default, [], []
                ),
                (
                    new Vector2<int>(7, 7),
                    new Vector2<int>(1, 2),
                    Team.Default, [], []
                ),
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(4, 2), new(5, 2), new(6, 2), new(7, 2), 
                new(7, 3), 
                new(7, 4), 
                new(7, 5), 
                new(4, 6), new(5, 6), new(6, 6), new(7, 6), 
                
                new(2, 4), new(3, 4), new(4, 4), new(5, 4), new(6, 4), 
                new(2, 5), 
                new(2, 6), 
                new(2, 7), new(7, 7), 
                new(2, 8), new(3, 8), new(4, 8), new(5, 8), new(6, 8), new(7, 8), 
                
                new(3, 5), new(4, 5), new(5, 5), new(6, 5), 
                new(3, 6), 
                new(3, 7), new(4, 7), new(5, 7), new(6, 7), 
            },
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new Vector2<int>[] // Free high ground
            {
                new(4, 2), new(5, 2), new(6, 2), new(7, 2), 
                new(7, 3), 
                new(7, 4), 
                new(7, 5), 
                new(4, 6), new(5, 6), new(6, 6), new(7, 6), 
            },
            new[] // Non connected position pairs
            {
                ((new Vector2<int>(3, 5), false), (new Vector2<int>(4, 6), true)), 
                ((new Vector2<int>(4, 5), false), (new Vector2<int>(4, 6), true)), 
                ((new Vector2<int>(5, 5), false), (new Vector2<int>(4, 6), true)), 
                ((new Vector2<int>(3, 6), false), (new Vector2<int>(4, 6), true)), 
                ((new Vector2<int>(3, 7), false), (new Vector2<int>(4, 6), true)), 
                ((new Vector2<int>(4, 7), false), (new Vector2<int>(4, 6), true)), 
                ((new Vector2<int>(5, 7), false), (new Vector2<int>(4, 6), true)), 
                
                ((new Vector2<int>(4, 5), false), (new Vector2<int>(5, 6), true)), 
                ((new Vector2<int>(5, 5), false), (new Vector2<int>(5, 6), true)), 
                ((new Vector2<int>(6, 5), false), (new Vector2<int>(5, 6), true)), 
                ((new Vector2<int>(4, 7), false), (new Vector2<int>(5, 6), true)), 
                ((new Vector2<int>(5, 7), false), (new Vector2<int>(5, 6), true)), 
                ((new Vector2<int>(6, 7), false), (new Vector2<int>(5, 6), true)), 
            },
            Array.Empty<((Vector2<int>, bool), (Vector2<int>, bool))>(), // Connected position pairs
        ];
        
        yield return
        [
            "Z146", // Scenario
            new PathfindingSize(1),
            new (Vector2<int>, Vector2<int>, Team, Area[], Area[])[] // Position, size, team, ascendables, high grounds
            {
                (
                    new Vector2<int>(5, 3),
                    new Vector2<int>(1, 4),
                    Team.Default, 
                    [
                        new Area(new Vector2<int>(5, 4), new Vector2<int>(1, 2)), 
                    ],
                    [
                        new Area(new Vector2<int>(5, 3), new Vector2<int>(1, 4)), 
                    ]
                ),
                (
                    new Vector2<int>(6, 3),
                    new Vector2<int>(2, 1),
                    Team.Default, [], []
                ),
                (
                    new Vector2<int>(7, 3),
                    new Vector2<int>(1, 4),
                    Team.Default, [], []
                ),
                (
                    new Vector2<int>(6, 6),
                    new Vector2<int>(2, 1),
                    Team.Default, [], []
                ),
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(5, 3), new(6, 3), new(7, 3), 
                new(5, 4), new(7, 4), 
                new(5, 5), new(7, 5), 
                new(5, 6), new(6, 6), new(7, 6), 
            },
            Array.Empty<Vector2<int>>(), // Occupation: high ground
            new Vector2<int>[] // Free high ground
            {
                new(5, 3), 
                new(5, 4), 
                new(5, 5), 
                new(5, 6), 
            },
            new[] // Non connected position pairs
            {
                ((new Vector2<int>(4, 2), false), (new Vector2<int>(5, 3), true)), 
                ((new Vector2<int>(5, 2), false), (new Vector2<int>(5, 3), true)), 
                ((new Vector2<int>(6, 2), false), (new Vector2<int>(5, 3), true)), 
                ((new Vector2<int>(4, 3), false), (new Vector2<int>(5, 3), true)), 
                ((new Vector2<int>(4, 4), false), (new Vector2<int>(5, 3), true)), 
                ((new Vector2<int>(6, 4), false), (new Vector2<int>(5, 3), true)), 
            },
            new[] // Connected position pairs
            {
                ((new Vector2<int>(4, 3), false), (new Vector2<int>(5, 4), true)), 
                ((new Vector2<int>(4, 4), false), (new Vector2<int>(5, 4), true)), 
                ((new Vector2<int>(4, 5), false), (new Vector2<int>(5, 4), true)), 
                ((new Vector2<int>(6, 4), false), (new Vector2<int>(5, 4), true)), 
                ((new Vector2<int>(6, 5), false), (new Vector2<int>(5, 4), true)), 

                ((new Vector2<int>(4, 4), false), (new Vector2<int>(5, 5), true)), 
                ((new Vector2<int>(4, 5), false), (new Vector2<int>(5, 5), true)), 
                ((new Vector2<int>(4, 6), false), (new Vector2<int>(5, 5), true)), 
                ((new Vector2<int>(6, 4), false), (new Vector2<int>(5, 5), true)), 
                ((new Vector2<int>(6, 5), false), (new Vector2<int>(5, 5), true)), 
            },
        ];
        
        yield return
        [
            "Z158", // Scenario
            new PathfindingSize(1),
            new (Vector2<int>, Vector2<int>, Team, Area[], Area[])[] // Position, size, team, ascendables, high grounds
            {
                (
                    new Vector2<int>(5, 3),
                    new Vector2<int>(1, 4),
                    new Team(2), 
                    [
                        new Area(new Vector2<int>(5, 4), new Vector2<int>(1, 2)), 
                    ],
                    [
                        new Area(new Vector2<int>(5, 3), new Vector2<int>(1, 4)), 
                    ]
                ),
                (
                    new Vector2<int>(5, 3),
                    new Vector2<int>(3, 1),
                    Team.Default, [], []
                ),
                (
                    new Vector2<int>(7, 3),
                    new Vector2<int>(1, 4),
                    Team.Default, [], []
                ),
                (
                    new Vector2<int>(5, 6),
                    new Vector2<int>(3, 1),
                    Team.Default, [], []
                ),
            },

            // Expected:
            new Vector2<int>[] // Occupation: low ground
            {
                new(5, 3), new(6, 3), new(7, 3), 
                new(5, 4), new(7, 4), 
                new(5, 5), new(7, 5), 
                new(5, 6), new(6, 6), new(7, 6), 
                
                new(6, 4),
                new(6, 5),
            },
            new Vector2<int>[] // Occupation: high ground
            {
                // Not actually occupied, just inaccessible.
                new(5, 3), 
                new(5, 4), 
                new(5, 5), 
                new(5, 6), 
            },
            Array.Empty<Vector2<int>>(), // Free high ground
            new[] // Non connected position pairs
            {
                ((new Vector2<int>(4, 2), false), (new Vector2<int>(5, 3), true)), 
                ((new Vector2<int>(5, 2), false), (new Vector2<int>(5, 3), true)), 
                ((new Vector2<int>(6, 2), false), (new Vector2<int>(5, 3), true)), 
                ((new Vector2<int>(4, 3), false), (new Vector2<int>(5, 3), true)), 
                ((new Vector2<int>(4, 4), false), (new Vector2<int>(5, 3), true)), 
                ((new Vector2<int>(6, 4), false), (new Vector2<int>(5, 3), true)), 
                
                ((new Vector2<int>(4, 3), false), (new Vector2<int>(5, 4), true)), 
                ((new Vector2<int>(4, 4), false), (new Vector2<int>(5, 4), true)), 
                ((new Vector2<int>(4, 5), false), (new Vector2<int>(5, 4), true)), 
                ((new Vector2<int>(6, 4), false), (new Vector2<int>(5, 4), true)), 
                ((new Vector2<int>(6, 5), false), (new Vector2<int>(5, 4), true)), 

                ((new Vector2<int>(4, 4), false), (new Vector2<int>(5, 5), true)), 
                ((new Vector2<int>(4, 5), false), (new Vector2<int>(5, 5), true)), 
                ((new Vector2<int>(4, 6), false), (new Vector2<int>(5, 5), true)), 
                ((new Vector2<int>(6, 4), false), (new Vector2<int>(5, 5), true)), 
                ((new Vector2<int>(6, 5), false), (new Vector2<int>(5, 5), true)), 
            },
            Array.Empty<((Vector2<int>, bool), (Vector2<int>, bool))>(), // Connected position pairs
        ];
    }

    [Theory]
    [MemberData(nameof(GetExpectedOccupationAndHighGroundForComplexStairs))]
    public void Pathfinding_ShouldCalculateHighGroundAndOccupation_ForComplexStairs(
        string scenario,
        PathfindingSize pathfindingSize,
        (Vector2<int>, Vector2<int>, Team, Area[], Area[])[] entities, 
        Vector2<int>[] expectedOccupiedLowGroundPositions,
        Vector2<int>[] expectedOccupiedHighGroundPositions,
        Vector2<int>[] expectedFreeHighGroundPositions,
        ((Vector2<int>, bool), (Vector2<int>, bool))[] expectedNonConnectedPositionPairs,
        ((Vector2<int>, bool), (Vector2<int>, bool))[] expectedConnectedPositionPairs)
    {
        foreach (var (pos, size, team, ascendables, highGrounds) in entities)
        {
            SetupAddingEntityInstance(pos, size, team, false, null,
                ascendables.Select(x => x.ToVectors()).ToList(),
                highGrounds.SelectMany(x => x.ToVectors()).ToHashSet());
        }

        var availablePoints = GetAvailablePointsFor(pathfindingSize);

        ExpectedPositionsShouldNotBeContainedIn(availablePoints,
            expectedOccupiedLowGroundPositions, CheckElevation.LowGround,
            scenario);

        ExpectedPositionsShouldNotBeContainedIn(availablePoints,
            expectedOccupiedHighGroundPositions, CheckElevation.HighGround,
            scenario);
        
        ExpectedHighGroundPositionsShouldBeAccessibleIn(availablePoints, 
            expectedFreeHighGroundPositions, scenario);

        ExpectedPositionsShouldBeConnected(pathfindingSize, expectedNonConnectedPositionPairs, 
            expectedConnectedPositionPairs, scenario);
    }

    #endregion Complex Scenarios: 5. Complex Stairs

    #region Complex Scenarios: 6. Changes Over Time

    public class GivenEntity
    {
        public required int Id { get; init; }
        public required Vector2<int> PrimaryPosition { get; init; }
        public required Vector2<int> Size { get; init; }
        public required Vector2<int>[][] Ascendables { get; init; } = [];
        public required Area[] HighGrounds { get; init; } = [];
        public required Area[] Walkables { get; init; } = [];
    }
    
    public class ChangeStep
    {
        public required string Scenario { get; init; }
        public required int[] AddedEntities { get; init; } = [];
        public required int[] RemovedEntities { get; init; } = [];
        public required Vector2<int>[] ExpectedOccupiedLowGroundPositions { get; init; } = [];
        public required Vector2<int>[] ExpectedOccupiedHighGroundPositions { get; init; } = [];
        public required Vector2<int>[] ExpectedFreeHighGroundPositions { get; init; } = [];
        public required ((Vector2<int>, bool), (Vector2<int>, bool))[] ExpectedNonConnectedPositionPairs { get; init; } = [];
    }
    
    public static IEnumerable<object[]> GetExpectedOccupationAndHighGroundForChangesOverTime()
    {
        yield return
        [
            "Forwards", 
            new PathfindingSize(1),
            new GivenEntity[]
            {
                new()
                {
                    Id = 32,
                    PrimaryPosition = new Vector2<int>(3, 2),
                    Size = new Vector2<int>(3, 3),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(3, 3), new(4, 3), new(3, 4), 
                        }, 
                    ],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(4, 4), new Vector2<int>(2, 1)), 
                    ],
                    Walkables =
                    [
                        new Area(new Vector2<int>(3, 2), new Vector2<int>(3, 1)), 
                        new Area(new Vector2<int>(5, 3), new Vector2<int>(1, 1)), 
                    ]
                }, 
                new()
                {
                    Id = 62,
                    PrimaryPosition = new Vector2<int>(6, 2),
                    Size = new Vector2<int>(1, 2),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(6, 3),  
                        }, 
                        new Vector2<int>[]
                        {
                            new(6, 2),  
                        }, 
                    ],
                    HighGrounds = [],
                    Walkables = []
                }, 
                new()
                {
                    Id = 64,
                    PrimaryPosition = new Vector2<int>(6, 4),
                    Size = new Vector2<int>(1, 3),
                    Ascendables = [],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(6, 4), new Vector2<int>(1, 3)), 
                    ],
                    Walkables = []
                }, 
                new()
                {
                    Id = 78,
                    PrimaryPosition = new Vector2<int>(7, 8),
                    Size = new Vector2<int>(3, 2),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(8, 8), new(8, 9), 
                        }, 
                        new Vector2<int>[]
                        {
                            new(7, 8), new(7, 9), 
                        }, 
                    ],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(9, 8), new Vector2<int>(1, 2)), 
                    ],
                    Walkables = []
                }, 
                new()
                {
                    Id = 93,
                    PrimaryPosition = new Vector2<int>(9, 3),
                    Size = new Vector2<int>(1, 2),
                    Ascendables = [],
                    HighGrounds = [],
                    Walkables = []
                }, 
                new()
                {
                    Id = 28,
                    PrimaryPosition = new Vector2<int>(2, 8),
                    Size = new Vector2<int>(1, 2),
                    Ascendables = [],
                    HighGrounds = [],
                    Walkables = []
                }, 
            },
            new ChangeStep[]
            {
                new()
                {
                    Scenario = "B182",
                    AddedEntities = [32, 62, 64, 78, 93, 28],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(6, 2), new(6, 3),
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(9, 3), new(9, 4),
                        
                        new(2, 8), new(2, 9), 
                    },
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[] { },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(6, 2), new(6, 3),
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
                new()
                {
                    Scenario = "N182",
                    AddedEntities = [],
                    RemovedEntities = [62],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(9, 3), new(9, 4),
                        
                        new(2, 8), new(2, 9), 
                    },
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[] { },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
                new()
                {
                    Scenario = "Z182",
                    AddedEntities = [],
                    RemovedEntities = [32],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(9, 3), new(9, 4),
                        
                        new(2, 8), new(2, 9), 
                    },
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[]
                    {
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
                new()
                {
                    Scenario = "AL182",
                    AddedEntities = [],
                    RemovedEntities = [78],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(9, 3), new(9, 4),
                        
                        new(2, 8), new(2, 9), 
                    },
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[] { },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
            }
        ];
        
        yield return
        [
            "Forwards", 
            new PathfindingSize(2),
            new GivenEntity[]
            {
                new()
                {
                    Id = 32,
                    PrimaryPosition = new Vector2<int>(3, 2),
                    Size = new Vector2<int>(3, 3),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(3, 3), new(4, 3), new(3, 4), 
                        }, 
                    ],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(4, 4), new Vector2<int>(2, 1)), 
                    ],
                    Walkables =
                    [
                        new Area(new Vector2<int>(3, 2), new Vector2<int>(3, 1)), 
                        new Area(new Vector2<int>(5, 3), new Vector2<int>(1, 1)), 
                    ]
                }, 
                new()
                {
                    Id = 62,
                    PrimaryPosition = new Vector2<int>(6, 2),
                    Size = new Vector2<int>(1, 2),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(6, 3),  
                        }, 
                        new Vector2<int>[]
                        {
                            new(6, 2),  
                        }, 
                    ],
                    HighGrounds = [],
                    Walkables = []
                }, 
                new()
                {
                    Id = 64,
                    PrimaryPosition = new Vector2<int>(6, 4),
                    Size = new Vector2<int>(1, 3),
                    Ascendables = [],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(6, 4), new Vector2<int>(1, 3)), 
                    ],
                    Walkables = []
                }, 
                new()
                {
                    Id = 78,
                    PrimaryPosition = new Vector2<int>(7, 8),
                    Size = new Vector2<int>(3, 2),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(8, 8), new(8, 9), 
                        }, 
                        new Vector2<int>[]
                        {
                            new(7, 8), new(7, 9), 
                        }, 
                    ],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(9, 8), new Vector2<int>(1, 2)), 
                    ],
                    Walkables = []
                }, 
                new()
                {
                    Id = 93,
                    PrimaryPosition = new Vector2<int>(9, 3),
                    Size = new Vector2<int>(1, 2),
                    Ascendables = [],
                    HighGrounds = [],
                    Walkables = []
                }, 
                new()
                {
                    Id = 28,
                    PrimaryPosition = new Vector2<int>(2, 8),
                    Size = new Vector2<int>(1, 2),
                    Ascendables = [],
                    HighGrounds = [],
                    Walkables = []
                }, 
            },
            new ChangeStep[]
            {
                new()
                {
                    Scenario = "B194",
                    AddedEntities = [32, 62, 64, 78, 93, 28],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(5, 2), new(5, 3), 
                        new(6, 2), new(6, 3),
                        
                        new(5, 3), new(6, 3), new(5, 4), new(5, 5), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 7), new(8, 7), new(9, 7), 
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(8, 2), new(8, 3), new(8, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(1, 7), new(2, 7), new(1, 8), 
                        new(2, 8), new(2, 9), 
                    }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4), 
                        
                        new(6, 2), new(6, 3),
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), 
                        
                        new(7, 8), new(8, 8), 
                    },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[]
                    {
                        new ((new Vector2<int>(7, 6), false), (new Vector2<int>(6, 7), false)), 
                    }
                }, 
                new()
                {
                    Scenario = "N194",
                    AddedEntities = [],
                    RemovedEntities = [62],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(5, 3), new(6, 3), new(5, 4), new(5, 5), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 7), new(8, 7), new(9, 7), 
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(8, 2), new(8, 3), new(8, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(1, 7), new(2, 7), new(1, 8), 
                        new(2, 8), new(2, 9), 
                    }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4), 
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), 
                        
                        new(7, 8), new(8, 8), 
                    },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[]
                    {
                        new ((new Vector2<int>(7, 6), false), (new Vector2<int>(6, 7), false)), 
                    }
                }, 
                new()
                {
                    Scenario = "Z194",
                    AddedEntities = [],
                    RemovedEntities = [32],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(5, 3), new(6, 3), new(5, 4), new(5, 5), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 7), new(8, 7), new(9, 7), 
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(8, 2), new(8, 3), new(8, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(1, 7), new(2, 7), new(1, 8), 
                        new(2, 8), new(2, 9), 
                    }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[]
                    {
                        new(7, 8), new(8, 8), 
                    },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[]
                    {
                        new ((new Vector2<int>(7, 6), false), (new Vector2<int>(6, 7), false)), 
                    }
                }, 
                new()
                {
                    Scenario = "AL194",
                    AddedEntities = [],
                    RemovedEntities = [78],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(5, 3), new(6, 3), new(5, 4), new(5, 5), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(8, 2), new(8, 3), new(8, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(1, 7), new(2, 7), new(1, 8), 
                        new(2, 8), new(2, 9), 
                    }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[] { },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
            }
        ];
        
        yield return
        [
            "Forwards",
            new PathfindingSize(3),
            new GivenEntity[]
            {
                new()
                {
                    Id = 32,
                    PrimaryPosition = new Vector2<int>(3, 2),
                    Size = new Vector2<int>(3, 3),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(3, 3), new(4, 3), new(3, 4), 
                        }, 
                    ],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(4, 4), new Vector2<int>(2, 1)), 
                    ],
                    Walkables =
                    [
                        new Area(new Vector2<int>(3, 2), new Vector2<int>(3, 1)), 
                        new Area(new Vector2<int>(5, 3), new Vector2<int>(1, 1)), 
                    ]
                }, 
                new()
                {
                    Id = 62,
                    PrimaryPosition = new Vector2<int>(6, 2),
                    Size = new Vector2<int>(1, 2),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(6, 3),  
                        }, 
                        new Vector2<int>[]
                        {
                            new(6, 2),  
                        }, 
                    ],
                    HighGrounds = [],
                    Walkables = []
                }, 
                new()
                {
                    Id = 64,
                    PrimaryPosition = new Vector2<int>(6, 4),
                    Size = new Vector2<int>(1, 3),
                    Ascendables = [],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(6, 4), new Vector2<int>(1, 3)), 
                    ],
                    Walkables = []
                }, 
                new()
                {
                    Id = 78,
                    PrimaryPosition = new Vector2<int>(7, 8),
                    Size = new Vector2<int>(3, 2),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(8, 8), new(8, 9), 
                        }, 
                        new Vector2<int>[]
                        {
                            new(7, 8), new(7, 9), 
                        }, 
                    ],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(9, 8), new Vector2<int>(1, 2)), 
                    ],
                    Walkables = []
                }, 
                new()
                {
                    Id = 93,
                    PrimaryPosition = new Vector2<int>(9, 3),
                    Size = new Vector2<int>(1, 2),
                    Ascendables = [],
                    HighGrounds = [],
                    Walkables = []
                }, 
                new()
                {
                    Id = 28,
                    PrimaryPosition = new Vector2<int>(2, 8),
                    Size = new Vector2<int>(1, 2),
                    Ascendables = [],
                    HighGrounds = [],
                    Walkables = []
                }, 
            },
            new ChangeStep[]
            {
                new()
                {
                    Scenario = "B206",
                    AddedEntities = [32, 62, 64, 78, 93, 28],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(3, 2), new(4, 2), new(5, 2), new(5, 3), new(2, 3), new(2, 4), 
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(4, 1), new(5, 1), new(6, 1), 
                        new(5, 2), new(5, 3), 
                        new(6, 2), new(6, 3),
                        
                        new(4, 2), new(5, 2), new(6, 2), 
                        new(4, 3), new(5, 3), new(6, 3), 
                        new(4, 4), new(5, 4), 
                        new(4, 5), new(5, 5), 
                        new(4, 6), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 6), new(6, 7), new(7, 7), 
                        new(7, 7), new(8, 7), new(9, 7), 
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(7, 1), new(7, 2), new(7, 3), new(7, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(0, 6), new(1, 6), new(2, 6), 
                        new(0, 7), new(1, 7), new(2, 7), 
                        new(2, 8), new(2, 9), 
                        
                        new(7, 5), 
                    }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4), 
                        
                        new(6, 2), new(6, 3),
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[] { },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
                new()
                {
                    Scenario = "N206",
                    AddedEntities = [],
                    RemovedEntities = [62],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(3, 2), new(4, 2), new(5, 2), new(5, 3), new(2, 3), new(2, 4), 
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(4, 2), new(5, 2), new(6, 2), 
                        new(4, 3), new(5, 3), new(6, 3), 
                        new(4, 4), new(5, 4), 
                        new(4, 5), new(5, 5), 
                        new(4, 6), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 6), new(6, 7), new(7, 7), 
                        new(7, 7), new(8, 7), new(9, 7), 
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(7, 1), new(7, 2), new(7, 3), new(7, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(0, 6), new(1, 6), new(2, 6), 
                        new(0, 7), new(1, 7), new(2, 7), 
                        new(2, 8), new(2, 9), 
                        
                        new(7, 5), 
                    }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4), 
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[] { },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
                new()
                {
                    Scenario = "Z206",
                    AddedEntities = [],
                    RemovedEntities = [32],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(4, 2), new(5, 2), new(6, 2), 
                        new(4, 3), new(5, 3), new(6, 3), 
                        new(4, 4), new(5, 4), 
                        new(4, 5), new(5, 5), 
                        new(4, 6), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 6), new(6, 7), new(7, 7), 
                        new(7, 7), new(8, 7), new(9, 7), 
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(7, 1), new(7, 2), new(7, 3), new(7, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(0, 6), new(1, 6), new(2, 6), 
                        new(0, 7), new(1, 7), new(2, 7), 
                        new(2, 8), new(2, 9), 
                        
                        new(7, 5), 
                    }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[] { },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
                new()
                {
                    Scenario = "AL206",
                    AddedEntities = [],
                    RemovedEntities = [78],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(4, 2), new(5, 2), new(6, 2), 
                        new(4, 3), new(5, 3), new(6, 3), 
                        new(4, 4), new(5, 4), 
                        new(4, 5), new(5, 5), 
                        new(4, 6), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 1), new(7, 2), new(7, 3), new(7, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(0, 6), new(1, 6), new(2, 6), 
                        new(0, 7), new(1, 7), new(2, 7), 
                        new(2, 8), new(2, 9), 
                    }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[] { },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
            }
        ];
        
        yield return
        [
            "Backwards", 
            new PathfindingSize(1),
            new GivenEntity[]
            {
                new()
                {
                    Id = 32,
                    PrimaryPosition = new Vector2<int>(3, 2),
                    Size = new Vector2<int>(3, 3),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(3, 3), new(4, 3), new(3, 4), 
                        }, 
                    ],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(4, 4), new Vector2<int>(2, 1)), 
                    ],
                    Walkables =
                    [
                        new Area(new Vector2<int>(3, 2), new Vector2<int>(3, 1)), 
                        new Area(new Vector2<int>(5, 3), new Vector2<int>(1, 1)), 
                    ]
                }, 
                new()
                {
                    Id = 62,
                    PrimaryPosition = new Vector2<int>(6, 2),
                    Size = new Vector2<int>(1, 2),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(6, 3),  
                        }, 
                        new Vector2<int>[]
                        {
                            new(6, 2),  
                        }, 
                    ],
                    HighGrounds = [],
                    Walkables = []
                }, 
                new()
                {
                    Id = 64,
                    PrimaryPosition = new Vector2<int>(6, 4),
                    Size = new Vector2<int>(1, 3),
                    Ascendables = [],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(6, 4), new Vector2<int>(1, 3)), 
                    ],
                    Walkables = []
                }, 
                new()
                {
                    Id = 78,
                    PrimaryPosition = new Vector2<int>(7, 8),
                    Size = new Vector2<int>(3, 2),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(8, 8), new(8, 9), 
                        }, 
                        new Vector2<int>[]
                        {
                            new(7, 8), new(7, 9), 
                        }, 
                    ],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(9, 8), new Vector2<int>(1, 2)), 
                    ],
                    Walkables = []
                }, 
                new()
                {
                    Id = 93,
                    PrimaryPosition = new Vector2<int>(9, 3),
                    Size = new Vector2<int>(1, 2),
                    Ascendables = [],
                    HighGrounds = [],
                    Walkables = []
                }, 
                new()
                {
                    Id = 28,
                    PrimaryPosition = new Vector2<int>(2, 8),
                    Size = new Vector2<int>(1, 2),
                    Ascendables = [],
                    HighGrounds = [],
                    Walkables = []
                }, 
            },
            new ChangeStep[]
            {
                new()
                {
                    Scenario = "AL182",
                    AddedEntities = [93, 28, 64],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(9, 3), new(9, 4),
                        
                        new(2, 8), new(2, 9), 
                    },
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[] { },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
                new()
                {
                    Scenario = "Z182",
                    AddedEntities = [78],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(9, 3), new(9, 4),
                        
                        new(2, 8), new(2, 9), 
                    },
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[]
                    {
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
                new()
                {
                    Scenario = "N182",
                    AddedEntities = [32],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(9, 3), new(9, 4),
                        
                        new(2, 8), new(2, 9), 
                    },
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[] { },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
                new()
                {
                    Scenario = "B182",
                    AddedEntities = [62],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(6, 2), new(6, 3),
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(9, 3), new(9, 4),
                        
                        new(2, 8), new(2, 9), 
                    },
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[] { },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(6, 2), new(6, 3),
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
            }
        ];
        
        yield return
        [
            "Backwards", 
            new PathfindingSize(2),
            new GivenEntity[]
            {
                new()
                {
                    Id = 32,
                    PrimaryPosition = new Vector2<int>(3, 2),
                    Size = new Vector2<int>(3, 3),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(3, 3), new(4, 3), new(3, 4), 
                        }, 
                    ],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(4, 4), new Vector2<int>(2, 1)), 
                    ],
                    Walkables =
                    [
                        new Area(new Vector2<int>(3, 2), new Vector2<int>(3, 1)), 
                        new Area(new Vector2<int>(5, 3), new Vector2<int>(1, 1)), 
                    ]
                }, 
                new()
                {
                    Id = 62,
                    PrimaryPosition = new Vector2<int>(6, 2),
                    Size = new Vector2<int>(1, 2),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(6, 3),  
                        }, 
                        new Vector2<int>[]
                        {
                            new(6, 2),  
                        }, 
                    ],
                    HighGrounds = [],
                    Walkables = []
                }, 
                new()
                {
                    Id = 64,
                    PrimaryPosition = new Vector2<int>(6, 4),
                    Size = new Vector2<int>(1, 3),
                    Ascendables = [],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(6, 4), new Vector2<int>(1, 3)), 
                    ],
                    Walkables = []
                }, 
                new()
                {
                    Id = 78,
                    PrimaryPosition = new Vector2<int>(7, 8),
                    Size = new Vector2<int>(3, 2),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(8, 8), new(8, 9), 
                        }, 
                        new Vector2<int>[]
                        {
                            new(7, 8), new(7, 9), 
                        }, 
                    ],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(9, 8), new Vector2<int>(1, 2)), 
                    ],
                    Walkables = []
                }, 
                new()
                {
                    Id = 93,
                    PrimaryPosition = new Vector2<int>(9, 3),
                    Size = new Vector2<int>(1, 2),
                    Ascendables = [],
                    HighGrounds = [],
                    Walkables = []
                }, 
                new()
                {
                    Id = 28,
                    PrimaryPosition = new Vector2<int>(2, 8),
                    Size = new Vector2<int>(1, 2),
                    Ascendables = [],
                    HighGrounds = [],
                    Walkables = []
                }, 
            },
            new ChangeStep[]
            {
                new()
                {
                    Scenario = "AL194",
                    AddedEntities = [93, 28, 64],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(5, 3), new(6, 3), new(5, 4), new(5, 5), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(8, 2), new(8, 3), new(8, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(1, 7), new(2, 7), new(1, 8), 
                        new(2, 8), new(2, 9), 
                    }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[] { },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
                new()
                {
                    Scenario = "Z194",
                    AddedEntities = [78],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(5, 3), new(6, 3), new(5, 4), new(5, 5), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 7), new(8, 7), new(9, 7), 
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(8, 2), new(8, 3), new(8, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(1, 7), new(2, 7), new(1, 8), 
                        new(2, 8), new(2, 9), 
                    }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[]
                    {
                        new(7, 8), new(8, 8), 
                    },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[]
                    {
                        new ((new Vector2<int>(7, 6), false), (new Vector2<int>(6, 7), false)), 
                    }
                }, 
                new()
                {
                    Scenario = "N194",
                    AddedEntities = [32],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(5, 3), new(6, 3), new(5, 4), new(5, 5), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 7), new(8, 7), new(9, 7), 
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(8, 2), new(8, 3), new(8, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(1, 7), new(2, 7), new(1, 8), 
                        new(2, 8), new(2, 9), 
                    }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4), 
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), 
                        
                        new(7, 8), new(8, 8), 
                    },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[]
                    {
                        new ((new Vector2<int>(7, 6), false), (new Vector2<int>(6, 7), false)), 
                    }
                }, 
                new()
                {
                    Scenario = "B194",
                    AddedEntities = [62],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(5, 2), new(5, 3), 
                        new(6, 2), new(6, 3),
                        
                        new(5, 3), new(6, 3), new(5, 4), new(5, 5), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 7), new(8, 7), new(9, 7), 
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(8, 2), new(8, 3), new(8, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(1, 7), new(2, 7), new(1, 8), 
                        new(2, 8), new(2, 9), 
                    }.Concat(Vector2Collections.Size2BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4), 
                        
                        new(6, 2), new(6, 3),
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), 
                        
                        new(7, 8), new(8, 8), 
                    },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[]
                    {
                        new ((new Vector2<int>(7, 6), false), (new Vector2<int>(6, 7), false)), 
                    }
                }, 
            }
        ];
    
        
        yield return
        [
            "Backwards",
            new PathfindingSize(3),
            new GivenEntity[]
            {
                new()
                {
                    Id = 32,
                    PrimaryPosition = new Vector2<int>(3, 2),
                    Size = new Vector2<int>(3, 3),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(3, 3), new(4, 3), new(3, 4), 
                        }, 
                    ],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(4, 4), new Vector2<int>(2, 1)), 
                    ],
                    Walkables =
                    [
                        new Area(new Vector2<int>(3, 2), new Vector2<int>(3, 1)), 
                        new Area(new Vector2<int>(5, 3), new Vector2<int>(1, 1)), 
                    ]
                }, 
                new()
                {
                    Id = 62,
                    PrimaryPosition = new Vector2<int>(6, 2),
                    Size = new Vector2<int>(1, 2),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(6, 3),  
                        }, 
                        new Vector2<int>[]
                        {
                            new(6, 2),  
                        }, 
                    ],
                    HighGrounds = [],
                    Walkables = []
                }, 
                new()
                {
                    Id = 64,
                    PrimaryPosition = new Vector2<int>(6, 4),
                    Size = new Vector2<int>(1, 3),
                    Ascendables = [],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(6, 4), new Vector2<int>(1, 3)), 
                    ],
                    Walkables = []
                }, 
                new()
                {
                    Id = 78,
                    PrimaryPosition = new Vector2<int>(7, 8),
                    Size = new Vector2<int>(3, 2),
                    Ascendables =
                    [
                        new Vector2<int>[]
                        {
                            new(8, 8), new(8, 9), 
                        }, 
                        new Vector2<int>[]
                        {
                            new(7, 8), new(7, 9), 
                        }, 
                    ],
                    HighGrounds =
                    [
                        new Area(new Vector2<int>(9, 8), new Vector2<int>(1, 2)), 
                    ],
                    Walkables = []
                }, 
                new()
                {
                    Id = 93,
                    PrimaryPosition = new Vector2<int>(9, 3),
                    Size = new Vector2<int>(1, 2),
                    Ascendables = [],
                    HighGrounds = [],
                    Walkables = []
                }, 
                new()
                {
                    Id = 28,
                    PrimaryPosition = new Vector2<int>(2, 8),
                    Size = new Vector2<int>(1, 2),
                    Ascendables = [],
                    HighGrounds = [],
                    Walkables = []
                }, 
            },
            new ChangeStep[]
            {
                new()
                {
                    Scenario = "AL206",
                    AddedEntities = [93, 28, 64],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(4, 2), new(5, 2), new(6, 2), 
                        new(4, 3), new(5, 3), new(6, 3), 
                        new(4, 4), new(5, 4), 
                        new(4, 5), new(5, 5), 
                        new(4, 6), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 1), new(7, 2), new(7, 3), new(7, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(0, 6), new(1, 6), new(2, 6), 
                        new(0, 7), new(1, 7), new(2, 7), 
                        new(2, 8), new(2, 9), 
                    }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[] { },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
                new()
                {
                    Scenario = "Z206",
                    AddedEntities = [78],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(4, 2), new(5, 2), new(6, 2), 
                        new(4, 3), new(5, 3), new(6, 3), 
                        new(4, 4), new(5, 4), 
                        new(4, 5), new(5, 5), 
                        new(4, 6), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 6), new(6, 7), new(7, 7), 
                        new(7, 7), new(8, 7), new(9, 7), 
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(7, 1), new(7, 2), new(7, 3), new(7, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(0, 6), new(1, 6), new(2, 6), 
                        new(0, 7), new(1, 7), new(2, 7), 
                        new(2, 8), new(2, 9), 
                        
                        new(7, 5), 
                    }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[] { },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
                new()
                {
                    Scenario = "N206",
                    AddedEntities = [32],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(3, 2), new(4, 2), new(5, 2), new(5, 3), new(2, 3), new(2, 4), 
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(4, 2), new(5, 2), new(6, 2), 
                        new(4, 3), new(5, 3), new(6, 3), 
                        new(4, 4), new(5, 4), 
                        new(4, 5), new(5, 5), 
                        new(4, 6), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 6), new(6, 7), new(7, 7), 
                        new(7, 7), new(8, 7), new(9, 7), 
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(7, 1), new(7, 2), new(7, 3), new(7, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(0, 6), new(1, 6), new(2, 6), 
                        new(0, 7), new(1, 7), new(2, 7), 
                        new(2, 8), new(2, 9), 
                        
                        new(7, 5), 
                    }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4), 
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[] { },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
                new()
                {
                    Scenario = "B206",
                    AddedEntities = [62],
                    RemovedEntities = [],
                    ExpectedOccupiedLowGroundPositions = new Vector2<int>[]
                    {
                        new(3, 2), new(4, 2), new(5, 2), new(5, 3), new(2, 3), new(2, 4), 
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4),
                        
                        new(4, 1), new(5, 1), new(6, 1), 
                        new(5, 2), new(5, 3), 
                        new(6, 2), new(6, 3),
                        
                        new(4, 2), new(5, 2), new(6, 2), 
                        new(4, 3), new(5, 3), new(6, 3), 
                        new(4, 4), new(5, 4), 
                        new(4, 5), new(5, 5), 
                        new(4, 6), new(5, 6), 
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 6), new(6, 7), new(7, 7), 
                        new(7, 7), new(8, 7), new(9, 7), 
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                        
                        new(7, 1), new(7, 2), new(7, 3), new(7, 4), 
                        new(9, 3), new(9, 4),
                        
                        new(0, 6), new(1, 6), new(2, 6), 
                        new(0, 7), new(1, 7), new(2, 7), 
                        new(2, 8), new(2, 9), 
                        
                        new(7, 5), 
                    }.Concat(Vector2Collections.Size3BoundariesFor10X10).ToArray(),
                    ExpectedOccupiedHighGroundPositions = new Vector2<int>[]
                    {
                        new(3, 3), new(4, 3), 
                        new(3, 4), new(4, 4), new(5, 4), 
                        
                        new(6, 2), new(6, 3),
                        
                        new(6, 4), new(6, 5), new(6, 6), 
                        
                        new(7, 8), new(8, 8), new(9, 8), 
                        new(7, 9), new(8, 9), new(9, 9), 
                    },
                    ExpectedFreeHighGroundPositions = new Vector2<int>[] { },
                    ExpectedNonConnectedPositionPairs = new ((Vector2<int>, bool), (Vector2<int>, bool))[] { }
                }, 
            }
        ];
    }

    [Theory]
    [MemberData(nameof(GetExpectedOccupationAndHighGroundForChangesOverTime))]
    public void Pathfinding_ShouldCalculateHighGroundAndOccupation_ForChangesOverTime(
        string scenario,
        PathfindingSize pathfindingSize,
        GivenEntity[] entities,
        ChangeStep[] changeSteps)
    {
        var entitiesInScene = new Dictionary<int, Guid>();
        foreach (var changeStep in changeSteps)
        {
            entitiesInScene = UpdateEntitiesInScene(changeStep, entitiesInScene, entities);

            var rng = new Random();
            var randomizedIds = entitiesInScene.Keys.ToList().OrderBy(_ => rng.Next()).ToList();
            /*if (changeStep.Scenario.Equals("Z206")) 
                randomizedIds = [78, 93, 64, 28];*/
            foreach (var id in randomizedIds)
            {
                var guid = entitiesInScene[id];
                _pathfinding.RemoveEntity(guid);
                var entity = entities.Single(x => x.Id == id);
                var newGuid = SetupAddingEntityInstance(entity.PrimaryPosition, entity.Size, Team.Default, false, 
                    entity.Walkables.SelectMany(x => x.ToVectors()).ToHashSet(),
                    entity.Ascendables.Select(x => x.AsEnumerable()).ToList(),
                    entity.HighGrounds.SelectMany(x => x.ToVectors()).ToHashSet());
                entitiesInScene[id] = newGuid;
                
                var availablePoints = GetAvailablePointsFor(pathfindingSize);

                ExpectedPositionsShouldNotBeContainedIn(availablePoints,
                    changeStep.ExpectedOccupiedLowGroundPositions, CheckElevation.LowGround, 
                    $"{scenario}-{changeStep.Scenario}");

                ExpectedPositionsShouldNotBeContainedIn(availablePoints,
                    changeStep.ExpectedOccupiedHighGroundPositions, CheckElevation.HighGround,
                    $"{scenario}-{changeStep.Scenario}");
                
                ExpectedHighGroundPositionsShouldBeAccessibleIn(availablePoints, 
                    changeStep.ExpectedFreeHighGroundPositions, $"{scenario}-{changeStep.Scenario}");

                ExpectedPositionsShouldBeConnected(pathfindingSize, changeStep.ExpectedNonConnectedPositionPairs, 
                    [], $"{scenario}-{changeStep.Scenario}");
            }
        }
    }

    private Dictionary<int, Guid> UpdateEntitiesInScene(ChangeStep changeStep, 
        Dictionary<int, Guid> entitiesInScene, GivenEntity[] entities)
    {
        foreach (var addedEntityId in changeStep.AddedEntities)
        {
            if (entitiesInScene.ContainsKey(addedEntityId))
                continue;
                
            var entity = entities.Single(x => x.Id == addedEntityId);
            var guid = SetupAddingEntityInstance(entity.PrimaryPosition, entity.Size, Team.Default, false, 
                entity.Walkables.SelectMany(x => x.ToVectors()).ToHashSet(),
                entity.Ascendables.Select(x => x.AsEnumerable()).ToList(),
                entity.HighGrounds.SelectMany(x => x.ToVectors()).ToHashSet());
                
            entitiesInScene[addedEntityId] = guid;
        }
            
        foreach (var removedEntityId in changeStep.RemovedEntities)
        {
            _pathfinding.RemoveEntity(entitiesInScene[removedEntityId]);
            entitiesInScene.Remove(removedEntityId);
        }

        return entitiesInScene;
    }
    
    #endregion Complex Scenarios: 6. Changes Over Time

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
        CheckElevation checkElevation,
        string scenario)
    {
        var foundUnfulfilledExpectedPositions = new HashSet<Vector2<int>>();

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
                        foundUnfulfilledExpectedPositions.Add(pos);
                    }

                    continue;
                }

                if (checkElevation is CheckElevation.LowGround)
                {
                    var foundPositions = availablePoints
                        .Where(x => x.Position.Equals(position) && x.IsLowGround)
                        .Select(x => x.Position);
                    foreach (var pos in foundPositions)
                    {
                        foundUnfulfilledExpectedPositions.Add(pos);
                    }

                    continue;
                }

                var foundPos = availablePoints
                    .Where(x => x.Position.Equals(position) && x.IsHighGround)
                    .Select(x => x.Position);
                foreach (var pos in foundPos)
                {
                    foundUnfulfilledExpectedPositions.Add(pos);
                }
            }
            else if (checkElevation is CheckElevation.LowGround)
            {
                availablePoints
                    .Where(x => x.IsLowGround && x.Position == position)
                    .Should()
                    .NotBeEmpty($"Remaining low ground at {position} has to be unoccupied at {checkElevation} " +
                                $"for scenario {scenario}");
            }
        }

        foundUnfulfilledExpectedPositions.Should().BeSubsetOf(Array.Empty<Vector2<int>>(), 
            $"Found positions should be occupied or inaccessible at {checkElevation} for scenario {scenario}");
    }

    private static void ExpectedHighGroundPositionsShouldBeAccessibleIn(IEnumerable<Point> availablePoints, 
        Vector2<int>[] expectedFreeHighGroundPositions, string scenario)
    {
        expectedFreeHighGroundPositions.Should().BeSubsetOf(
            availablePoints.Where(x => x.IsHighGround).Select(x => x.Position), 
            $"Found high ground positions should be accessible for scenario {scenario}");
    }
    
    private void ExpectedPositionsShouldBeConnected(PathfindingSize pathfindingSize, 
        ((Vector2<int>, bool), (Vector2<int>, bool))[] expectedNonConnectedPositionPairs, 
        ((Vector2<int>, bool), (Vector2<int>, bool))[] expectedConnectedPositionPairs, 
        string scenario)
    {
        var foundIncorrectlyConnectedPositionPairs = 
            GetConnectedPairs(pathfindingSize, expectedNonConnectedPositionPairs, true);
        var foundIncorrectlyNonConnectedPositionPairs = 
            GetConnectedPairs(pathfindingSize, expectedConnectedPositionPairs, false);

        foundIncorrectlyConnectedPositionPairs.Should().BeSubsetOf([],
            $"Found position pairs should be non-connected for scenario {scenario}");
        foundIncorrectlyNonConnectedPositionPairs.Should().BeSubsetOf([],
            $"Found position pairs should be connected for scenario {scenario}");
    }

    private Guid SetupAddingEntityInstance(Vector2<int> entityPrimaryPosition, Vector2<int> entitySize,
        Team? team = null, bool? isOnHighGround = null, IEnumerable<Vector2<int>>? walkablePositions = null,
        IList<IEnumerable<Vector2<int>>>? ascendablePositions = null,
        IEnumerable<Vector2<int>>? highGroundPositions = null)
    {
        var entityId = Guid.NewGuid();
        var entity = new PathfindingEntity(entityId, entityPrimaryPosition, entitySize, team ?? Team.Default,
            isOnHighGround ?? false, (p, t) => CanBeMovedThroughAt(p, t, entityId),
            (fp, tp, t) => AllowsConnectionBetweenPoints(fp, tp, t, entityId));

        _entities[entityId] = entity;
        _walkablePositionsByEntityId[entityId] = walkablePositions ?? new List<Vector2<int>>();
        _ascendablesByEntityId[entityId] = ascendablePositions
                                           ?? new List<IEnumerable<Vector2<int>>> { new List<Vector2<int>>() };
        _highGroundsByEntityId[entityId] = highGroundPositions ?? new List<Vector2<int>>();

        _pathfinding.AddOrUpdateEntity(entity);
        if (ascendablePositions != null)
            _pathfinding.AddAscendableHighGround(entityId, ascendablePositions);
        if (highGroundPositions != null)
            _pathfinding.AddHighGround(entityId, highGroundPositions);
        _pathfinding.UpdateAround(entityId);

        return entityId;
    }

    private bool AllowsConnectionBetweenPoints(Point fromPoint, Point toPoint, Team forTeam, Guid entityId)
    {
        var isSameTeam = _entities[entityId].Team.Equals(forTeam);

        var ascendablePositions = _ascendablesByEntityId[entityId]
            .SelectMany(inner => inner)
            .ToHashSet();
        
        return ascendablePositions.Contains(toPoint.Position) 
               && ((isSameTeam && fromPoint.IsLowGround) || fromPoint.IsHighGround);
    }

    private bool CanBeMovedThroughAt(Point point, Team forTeam, Guid entityId)
    {
        if (_walkablePositionsByEntityId[entityId].Any(point.Position.Equals))
            return true;

        if (HasHighGroundAt(point, forTeam, entityId))
            return true;

        if (point.Position.IsInBoundsOf(
                _entities[entityId].Position,
                _entities[entityId].UpperBounds))
            return false;

        return true;
    }

    private bool HasHighGroundAt(Point point, Team forTeam, Guid entityId)
    {
        if (point.IsLowGround)
            return false;

        var position = point.Position;

        if (position.IsInBoundsOf(
                _entities[entityId].Position,
                _entities[entityId].UpperBounds)
            is false)
            return false;

        if (CanBeMovedOnAtAscendable(position, entityId)
            || CanBeMovedOnAtHighGround(position, entityId))
            return true;

        return false;
    }

    private bool CanBeMovedOnAtAscendable(Vector2<int> position, Guid entityId)
    {
        var ascendablePositions = _ascendablesByEntityId[entityId]
            .SelectMany(inner => inner)
            .ToHashSet();

        return ascendablePositions.Contains(position);
    }

    private bool CanBeMovedOnAtHighGround(Vector2<int> position, Guid entityId)
        => _highGroundsByEntityId[entityId].Contains(position);
    
    private List<((Vector2<int>, bool), (Vector2<int>, bool))> GetConnectedPairs(
        PathfindingSize pathfindingSize, 
        IEnumerable<((Vector2<int>, bool), (Vector2<int>, bool))> positionPairs, 
        bool shouldBeConnected)
    {
        var result = new List<((Vector2<int>, bool), (Vector2<int>, bool))>();
        
        foreach (var (pointAEntry, pointBEntry) in positionPairs)
        {
            var (pointAPosition, pointAOnHighGround) = pointAEntry;
            var (pointBPosition, pointBOnLowGround) = pointBEntry;
            var pointA = _pathfinding.GetPointAt(pointAPosition, pointAOnHighGround, pathfindingSize);
            var pointB = _pathfinding.GetPointAt(pointBPosition, pointBOnLowGround, pathfindingSize);
            
            if (_pathfinding.HasConnection(pointA, pointB, Team.Default, pathfindingSize) == shouldBeConnected)
                result.Add((pointAEntry, pointBEntry));
        }

        return result;
    }

    #endregion Helpers
}