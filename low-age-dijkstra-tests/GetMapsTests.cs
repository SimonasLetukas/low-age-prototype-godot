using FluentAssertions;
using low_age_dijkstra;
using low_age_dijkstra.Methods;

namespace low_age_dijkstra_tests;

public class GetMapsTests
{
    private readonly DijkstraMap _dijkstraMap = new();

    private static PointId Id0 => new(0);
    private static PointId Id1 => new(1);
    private static PointId Id2 => new(2);

    // 0 -> 1 -> 2
    private void SetupConnections(Terrain? useTerrain = null)
    {
        useTerrain ??= Terrain.Default;
        _dijkstraMap.AddPoint(Id0, useTerrain.Value);
        _dijkstraMap.AddPoint(Id1, useTerrain.Value);
        _dijkstraMap.AddPoint(Id2, useTerrain.Value);
        _dijkstraMap.ConnectPoints(Id0, Id1, null, false);
        _dijkstraMap.ConnectPoints(Id1, Id2, null, false);
    }

    [Fact]
    public void GetDirectionAtPoint_ShouldNotGoFrom2To0_When0IsDestination()
    {
        SetupConnections();
        _dijkstraMap.Recalculate(Id0, inputDirection: InputDirection.InputIsDestination);

        _dijkstraMap.GetDirectionAtPoint(Id0).Should().BeEquivalentTo(Id0);
        _dijkstraMap.GetDirectionAtPoint(Id1).Should().BeNull();
        _dijkstraMap.GetDirectionAtPoint(Id2).Should().BeNull();
    }

    [Fact]
    public void GetDirectionAtPoint_ShouldGoFrom0To2_When2IsDestination()
    {
        SetupConnections();
        _dijkstraMap.Recalculate(Id2, inputDirection: InputDirection.InputIsDestination);

        _dijkstraMap.GetDirectionAtPoint(Id0).Should().BeEquivalentTo(Id1);
        _dijkstraMap.GetDirectionAtPoint(Id1).Should().BeEquivalentTo(Id2);
        _dijkstraMap.GetDirectionAtPoint(Id2).Should().BeEquivalentTo(Id2);
    }

    [Fact]
    public void GetDirectionAtPoint_ShouldGoFrom2To0_When0IsOrigin()
    {
        SetupConnections();
        _dijkstraMap.Recalculate(Id0, inputDirection: InputDirection.InputIsOrigin);

        _dijkstraMap.GetDirectionAtPoint(Id0).Should().BeEquivalentTo(Id0);
        _dijkstraMap.GetDirectionAtPoint(Id1).Should().BeEquivalentTo(Id0);
        _dijkstraMap.GetDirectionAtPoint(Id2).Should().BeEquivalentTo(Id1);
    }

    [Fact]
    public void GetDirectionAtPoint_ShouldNotGoFrom0To2_When2IsOrigin()
    {
        SetupConnections();
        _dijkstraMap.Recalculate(Id2, inputDirection: InputDirection.InputIsOrigin);

        _dijkstraMap.GetDirectionAtPoint(Id0).Should().BeNull();
        _dijkstraMap.GetDirectionAtPoint(Id1).Should().BeNull();
        _dijkstraMap.GetDirectionAtPoint(Id2).Should().BeEquivalentTo(Id2);
    }

    [Fact]
    public void GetCostAtPoint_ShouldHaveCorrectCostFrom0To2_When2IsDestination()
    {
        SetupConnections();
        _dijkstraMap.Recalculate(Id2, inputDirection: InputDirection.InputIsDestination);

        _dijkstraMap.GetCostAtPoint(Id0).Should().BeEquivalentTo(new Cost(2f));
        _dijkstraMap.GetCostAtPoint(Id1).Should().BeEquivalentTo(new Cost(1f));
        _dijkstraMap.GetCostAtPoint(Id2).Should().BeEquivalentTo(new Cost(0f));
    }

    [Fact]
    public void GetCostAtPoint_ShouldHaveInfiniteCostFrom2To0_When0IsDestination()
    {
        SetupConnections();
        _dijkstraMap.Recalculate(Id0, inputDirection: InputDirection.InputIsDestination);

        _dijkstraMap.GetCostAtPoint(Id0).Should().BeEquivalentTo(new Cost(0f));
        _dijkstraMap.GetCostAtPoint(Id1).Should().BeEquivalentTo(Cost.Infinity);
        _dijkstraMap.GetCostAtPoint(Id2).Should().BeEquivalentTo(Cost.Infinity);
    }

    [Fact]
    public void GetCostAtPoint_ShouldAccountForTerrain_WhenTerrainWeightsAreAdded()
    {
        SetupConnections(new Terrain(1));
        var terrainWeights = new Dictionary<Terrain, Weight>
        {
            { new Terrain(1), new Weight(2f) }
        };
        _dijkstraMap.Recalculate(Id2, terrainWeights: terrainWeights);

        _dijkstraMap.GetCostAtPoint(Id0).Should().BeEquivalentTo(new Cost(4f));
        _dijkstraMap.GetCostAtPoint(Id1).Should().BeEquivalentTo(new Cost(2f));
        _dijkstraMap.GetCostAtPoint(Id2).Should().BeEquivalentTo(new Cost(0f));
    }

    public static IEnumerable<object[]> GetExpectedPointsWithCostBetween()
    {
        yield return
        [
            new Cost(float.NegativeInfinity), new Cost(float.PositiveInfinity),
            // Expected:
            new List<PointId> { new(2), new(1), new(0) }
        ];
        
        yield return
        [
            new Cost(0.5f), new Cost(1.5f),
            // Expected:
            new List<PointId> { new(1) }
        ];
        
        yield return
        [
            new Cost(1f), new Cost(1f),
            // Expected:
            new List<PointId> { new(1) }
        ];
    }

    [Theory]
    [MemberData(nameof(GetExpectedPointsWithCostBetween))]
    public void GetAllPointsWithCostBetween_ShouldReturnCorrectPoints_WithGivenInput(Cost costFrom, Cost costTo, 
        IEnumerable<PointId> expected)
    {
        SetupConnections();
        _dijkstraMap.Recalculate(Id2);

        _dijkstraMap.GetAllPointsWithCostBetween(costFrom.Value, costTo.Value).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetDirectionAndCostMap_ShouldNotReturnUnreachablePoints_WhenConnectionIsNotAdded()
    {
        SetupConnections();
        var id3 = new PointId(3);
        _dijkstraMap.AddPoint(id3, Terrain.Default);
        _dijkstraMap.Recalculate(Id0);

        _dijkstraMap.GetDirectionAndCostMap().Should().NotContain(x => 
            x.Key.Equals(id3));
    }
}