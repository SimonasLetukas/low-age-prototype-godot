using FluentAssertions;
using low_age_dijkstra;
using low_age_dijkstra.Methods;

namespace low_age_dijkstra_tests;

public class DijkstraMapTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Recalculate_ShouldBeDeterministic_WithTheSamePointsAndConnections(bool reverseOrder)
    {
        var dijkstraMap = CreateMap(reverseOrder);

        dijkstraMap.Recalculate(0);

        var directionsAndCosts = dijkstraMap.GetDirectionAndCostMap();

        for (var i = 0; i < 100; i++)
        {
            // Mess up the order of creation
            var newMap = CreateMap(reverseOrder);
            newMap.Recalculate(new PointId(0));

            directionsAndCosts.Should().BeEquivalentTo(newMap.GetDirectionAndCostMap());
        }
    }

    [Fact]
    public void Recalculate_ShouldCalculateThePathCorrectly_WhenTerrainsAndWeightsMutate()
    {
        // Initial setup:
        // 0(t:1) -[w:1]- 1(t:2) -[w:10]- 2(t:1)
        //                \[w:1]- 3(t:2)
        var dijkstraMap = new DijkstraMap();
        dijkstraMap.AddPoint(0, new Terrain(1));
        dijkstraMap.AddPoint(1, new Terrain(2));
        dijkstraMap.AddPoint(2, new Terrain(1));
        dijkstraMap.AddPoint(3, new Terrain(2));
        dijkstraMap.ConnectPoints(0, 1);
        dijkstraMap.ConnectPoints(1, 2, 10f);
        dijkstraMap.ConnectPoints(1, 3);
        
        dijkstraMap.Recalculate(
            origin: 0, 
            terrainWeights: new Dictionary<Terrain, Weight>
            {
                {new Terrain(1), 1f},
                {new Terrain(2), 2f},
            }, 
            maximumCost: 2f);

        dijkstraMap.GetDirectionAtPoint(0).Should().Be(0);
        dijkstraMap.GetDirectionAtPoint(1).Should().Be(0);
        dijkstraMap.GetDirectionAtPoint(2).Should().Be(null);
        dijkstraMap.GetDirectionAtPoint(3).Should().Be(null);
        
        // Change terrain of 1 & 3 to become accessible:
        // 0(t:1) -[w:1]- 1(t:1) -[w:10]- 2(t:1)
        //                \[w:1]- 3(t:1)

        dijkstraMap.SetTerrainForPoint(1, new Terrain(1));
        dijkstraMap.SetTerrainForPoint(3, new Terrain(1));
        
        dijkstraMap.Recalculate(
            origin: 0, 
            terrainWeights: new Dictionary<Terrain, Weight>
            {
                {new Terrain(1), 1f},
                {new Terrain(2), 2f},
            }, 
            maximumCost: 2f);
        
        dijkstraMap.GetDirectionAtPoint(3).Should().Be(1);
        
        // Increase general cost of Terrain(1), change connection weight and
        // terrain for Point(2) so it's accessible, increase maximumCost:
        // 0(t:2) -[w:1]- 1(t:2) -[w:0.5]- 2(t:1)
        //                \[w:1]- 3(t:2)
        
        dijkstraMap.SetTerrainForPoint(2, new Terrain(2));
        dijkstraMap.ConnectPoints(1, 2, 0.5f);
        
        dijkstraMap.Recalculate(
            origin: 0, 
            terrainWeights: new Dictionary<Terrain, Weight>
            {
                {new Terrain(1), 2f},
                {new Terrain(2), 1f},
            }, 
            maximumCost: 3f);
        
        dijkstraMap.GetDirectionAtPoint(1).Should().Be(0);
        dijkstraMap.GetDirectionAtPoint(2).Should().Be(1);
        dijkstraMap.GetDirectionAtPoint(3).Should().Be(null);
    }

    [Fact]
    public void DuplicateGraph_ShouldWorkCorrectly()
    {
        var originalMap = CreateMap(false);
        originalMap.AddPoint(4);
        var duplicateMap = originalMap.DuplicateGraph();
        duplicateMap.ConnectPoints(3, 4, 1);
        duplicateMap.AddPoint(5);

        duplicateMap.HasPoint(0).Should().BeTrue();
        duplicateMap.HasPoint(1).Should().BeTrue();
        duplicateMap.HasPoint(2).Should().BeTrue();
        duplicateMap.HasPoint(3).Should().BeTrue();
        duplicateMap.HasConnection(0, 1).Should().BeTrue();
        duplicateMap.HasConnection(0, 2).Should().BeTrue();
        duplicateMap.HasConnection(1, 3).Should().BeTrue();
        duplicateMap.HasConnection(2, 3).Should().BeTrue();
        duplicateMap.HasConnection(1, 2).Should().BeFalse();
        duplicateMap.HasConnection(0, 3).Should().BeFalse();
        
        duplicateMap.HasPoint(4).Should().BeTrue();
        duplicateMap.HasConnection(3, 4).Should().BeTrue();
        duplicateMap.HasPoint(5).Should().BeTrue();
        duplicateMap.HasConnection(4, 5).Should().BeFalse();

        originalMap.HasConnection(3, 4).Should().BeFalse();
        originalMap.HasPoint(5).Should().BeFalse();
        originalMap.HasConnection(4, 5).Should().BeFalse();
    }

    [Fact]
    public void DuplicateGraphFrom_ShouldWorkCorrectly()
    {
        var originalMap = CreateMap(false);
        originalMap.AddPoint(4);
        var duplicateMap = new DijkstraMap();
        duplicateMap.DuplicateGraphFrom(originalMap);
        duplicateMap.ConnectPoints(3, 4, 1);
        duplicateMap.AddPoint(5);

        duplicateMap.HasPoint(0).Should().BeTrue();
        duplicateMap.HasPoint(1).Should().BeTrue();
        duplicateMap.HasPoint(2).Should().BeTrue();
        duplicateMap.HasPoint(3).Should().BeTrue();
        duplicateMap.HasConnection(0, 1).Should().BeTrue();
        duplicateMap.HasConnection(0, 2).Should().BeTrue();
        duplicateMap.HasConnection(1, 3).Should().BeTrue();
        duplicateMap.HasConnection(2, 3).Should().BeTrue();
        duplicateMap.HasConnection(1, 2).Should().BeFalse();
        duplicateMap.HasConnection(0, 3).Should().BeFalse();
        
        duplicateMap.HasPoint(4).Should().BeTrue();
        duplicateMap.HasConnection(3, 4).Should().BeTrue();
        duplicateMap.HasPoint(5).Should().BeTrue();
        duplicateMap.HasConnection(4, 5).Should().BeFalse();

        originalMap.HasConnection(3, 4).Should().BeFalse();
        originalMap.HasPoint(5).Should().BeFalse();
        originalMap.HasConnection(4, 5).Should().BeFalse();
    }
    
    private static DijkstraMap CreateMap(bool reverseOrder)
    {
        // graph :
        //     3
        //    / \
        //   1   2
        //    \ /
        //     0 <- origin
        
        var dijkstraMap = new DijkstraMap();
        dijkstraMap.AddPoint(new PointId(0), Terrain.Default);
        dijkstraMap.AddPoint(new PointId(3), Terrain.Default);

        if (reverseOrder)
        {
            for (var i = 2; i >= 1; i--)
            {
                dijkstraMap.AddPoint(new PointId(i), Terrain.Default);
                dijkstraMap.ConnectPoints(new PointId(0), new PointId(i), null, null);
                dijkstraMap.ConnectPoints(new PointId(3), new PointId(i), null, null);
            }
        }
        else
        {
            for (var i = 1; i <= 2; i++)
            {
                dijkstraMap.AddPoint(new PointId(i), Terrain.Default);
                dijkstraMap.ConnectPoints(new PointId(3), new PointId(i), null, null);
                dijkstraMap.ConnectPoints(new PointId(0), new PointId(i), null, null);
            }
        }

        return dijkstraMap;
    }
}