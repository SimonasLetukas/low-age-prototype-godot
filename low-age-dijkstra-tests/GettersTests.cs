using FluentAssertions;
using low_age_dijkstra;
using low_age_dijkstra.Methods;

namespace low_age_dijkstra_tests;

public class GettersTests
{
    private readonly DijkstraMap _dijkstraMap = new();

    [Fact]
    public void GetAvailableId_ShouldGetTheSmallestUnusedPointId()
    {
        var id = _dijkstraMap.GetAvailableId();
        id.Should().BeEquivalentTo(new PointId(0));
        
        id = _dijkstraMap.GetAvailableId();
        id.Should().BeEquivalentTo(new PointId(0));
        
        for (var i = 0; i < 100; i++)
        {
            id = _dijkstraMap.GetAvailableId();
            id.Should().BeEquivalentTo(new PointId(i));
            _dijkstraMap.AddPoint(id, Terrain.Default);
        }
    }
    
    [Fact]
    public void GetAvailableId_ShouldGetTheSmallestUnusedPointIdAboveThreshold()
    {
        var id = _dijkstraMap.GetAvailableId(new PointId(4));
        id.Should().BeEquivalentTo(new PointId(4));
    }
    
    [Fact]
    public void GetAvailableId_ShouldGetTheSmallestUnusedPointIdAboveThreshold_WhenPointAlreadyExists()
    {
        _dijkstraMap.AddPoint(new PointId(4), Terrain.Default);
        var id = _dijkstraMap.GetAvailableId(new PointId(4));
        id.Should().BeEquivalentTo(new PointId(5));
    }

    [Fact]
    public void GetShortestPathFromPoint()
    {
        for (var i = 0; i < 5; i++)
        {
            _dijkstraMap.AddPoint(new PointId(i), Terrain.Default);
        }

        for (var i = 0; i < 4; i++)
        {
            _dijkstraMap.ConnectPoints(new PointId(i + 1), new PointId(i), null, false);
        }
        
        _dijkstraMap.Recalculate(0);
        using var pathIterator = _dijkstraMap.GetShortestPathFromPoint(new PointId(3)).GetEnumerator();
        
        pathIterator.MoveNext().Should().BeTrue();
        pathIterator.Current.Should().BeEquivalentTo(new PointId(2));
        
        pathIterator.MoveNext().Should().BeTrue();
        pathIterator.Current.Should().BeEquivalentTo(new PointId(1));
        
        pathIterator.MoveNext().Should().BeTrue();
        pathIterator.Current.Should().BeEquivalentTo(new PointId(0));
        
        pathIterator.MoveNext().Should().BeFalse();
    }
}