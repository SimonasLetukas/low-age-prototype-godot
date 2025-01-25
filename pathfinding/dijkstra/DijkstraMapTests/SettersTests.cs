using FluentAssertions;
using DijkstraMap;
using DijkstraMap.Methods;

namespace DijkstraMapTests;

public class SettersTests
{
    private readonly Dijkstra _dijkstraMap = new();

    [Fact]
    public void AddPoint_ShouldAddPoint_WhenItDoesNotExist()
    {
        _dijkstraMap.AddPoint(0).Should().BeTrue();
    }
    
    [Fact]
    public void AddPoint_ShouldAddPointAndSpecifyTerrain_WhenItDoesNotExist()
    {
        _dijkstraMap.AddPoint(0);
        _dijkstraMap.AddPoint(1, new Terrain(1)).Should().BeTrue();
    }
    
    [Fact]
    public void AddPoint_ShouldNotAddPoint_WhenItExists()
    {
        _dijkstraMap.AddPoint(0, new Terrain(1));
        _dijkstraMap.AddPoint(0, new Terrain(1)).Should().BeFalse();
    }
    
    [Fact]
    public void AddPoint_ShouldNotChangeTerrain_WhenThePointExists()
    {
        _dijkstraMap.AddPoint(0, new Terrain(1));
        _dijkstraMap.AddPoint(0, new Terrain(2)).Should().BeFalse();
        _dijkstraMap.GetTerrainForPoint(0).Should().Be(new Terrain(1));
    }

    [Fact]
    public void SetTerrainForPoint_ShouldChangeTerrain_WhenThePointExists()
    {
        _dijkstraMap.AddPoint(0);
        _dijkstraMap.SetTerrainForPoint(0, new Terrain(1)).Should().BeTrue();
        _dijkstraMap.GetTerrainForPoint(0).Should().Be(new Terrain(1));
    }
    
    [Fact]
    public void SetTerrainForPoint_ShouldNotChangeTerrain_WhenThePointDoesNotExist()
    {
        _dijkstraMap.SetTerrainForPoint(0, new Terrain(1)).Should().BeFalse();
        _dijkstraMap.GetTerrainForPoint(0).Should().Be(Terrain.Default);
    }
    
    [Fact]
    public void SetTerrainForPoint_ShouldChangeTerrain_WhenDoneMultipleTimes()
    {
        _dijkstraMap.AddPoint(0);
        
        _dijkstraMap.SetTerrainForPoint(0, new Terrain(1)).Should().BeTrue();
        _dijkstraMap.GetTerrainForPoint(0).Should().Be(new Terrain(1));
        
        _dijkstraMap.SetTerrainForPoint(0, new Terrain(2)).Should().BeTrue();
        _dijkstraMap.GetTerrainForPoint(0).Should().Be(new Terrain(2));
    }

    [Fact]
    public void RemovePoint_ShouldReturnTrue_WhenPointExists()
    {
        _dijkstraMap.AddPoint(0);
        
        _dijkstraMap.RemovePoint(0).Should().BeTrue();
        _dijkstraMap.HasPoint(0).Should().BeFalse();
    }
    
    [Fact]
    public void RemovePoint_ShouldReturnFalse_WhenPointDoesNotExist()
    {
        _dijkstraMap.RemovePoint(0).Should().BeFalse();
        _dijkstraMap.HasPoint(0).Should().BeFalse();
    }
    
    [Fact]
    public void RemovePoint_ShouldReturnFalse_WhenPointDidExistButWasRemoved()
    {
        _dijkstraMap.AddPoint(0);
        
        _dijkstraMap.RemovePoint(0).Should().BeTrue();
        _dijkstraMap.HasPoint(0).Should().BeFalse();
        
        _dijkstraMap.RemovePoint(0).Should().BeFalse();
        _dijkstraMap.HasPoint(0).Should().BeFalse();
    }

    [Fact]
    public void DisablePoint_ShouldReturnFalse_WhenPointWasDisabledBefore()
    {
        _dijkstraMap.AddPoint(0);
        
        _dijkstraMap.DisablePoint(0).Should().BeTrue();
        _dijkstraMap.IsPointDisabled(0).Should().BeTrue();
        
        _dijkstraMap.DisablePoint(0).Should().BeFalse();
        _dijkstraMap.IsPointDisabled(0).Should().BeTrue();
    }
    
    [Fact]
    public void DisablePoint_ShouldReturnFalse_WhenPointDoesNotExist()
    {
        _dijkstraMap.DisablePoint(0).Should().BeFalse();
        _dijkstraMap.IsPointDisabled(0).Should().BeFalse();
    }
    
    [Fact]
    public void EnablePoint_ShouldReturnFalse_WhenPointWasEnabledBefore()
    {
        _dijkstraMap.AddPoint(0);
        _dijkstraMap.DisablePoint(0);
        
        _dijkstraMap.EnablePoint(0).Should().BeTrue();
        _dijkstraMap.IsPointDisabled(0).Should().BeFalse();
        
        _dijkstraMap.EnablePoint(0).Should().BeFalse();
        _dijkstraMap.IsPointDisabled(0).Should().BeFalse();
    }
    
    [Fact]
    public void EnablePoint_ShouldReturnFalse_WhenPointDoesNotExist()
    {
        _dijkstraMap.EnablePoint(0).Should().BeFalse();
        _dijkstraMap.IsPointDisabled(0).Should().BeFalse();
    }

    [Fact]
    public void ConnectPoints_ShouldBeBidirectional_ByDefault()
    {
        _dijkstraMap.AddPoint(0);
        _dijkstraMap.AddPoint(1);
        _dijkstraMap.ConnectPoints(0, 1).Should().BeTrue();
        
        _dijkstraMap.HasConnection(0, 1).Should().BeTrue();
        _dijkstraMap.HasConnection(1, 0).Should().BeTrue();
    }

    [Fact]
    public void ConnectPoints_ReturnsFalse_WhenEitherPointDoesNotExist()
    {
        _dijkstraMap.AddPoint(0);
        _dijkstraMap.ConnectPoints(0, 1).Should().BeFalse();
        _dijkstraMap.ConnectPoints(1, 0).Should().BeFalse();
    }
    
    [Fact]
    public void RemoveConnection_ReturnsTrue_WhenConnectionIsRemoved()
    {
        _dijkstraMap.AddPoint(0);
        _dijkstraMap.AddPoint(1);
        _dijkstraMap.ConnectPoints(0, 1).Should().BeTrue();
        
        _dijkstraMap.RemoveConnection(0, 1).Should().BeTrue();
        _dijkstraMap.HasConnection(0, 1).Should().BeFalse();
        _dijkstraMap.HasConnection(1, 0).Should().BeFalse();
    }
    
    [Fact]
    public void RemoveConnection_ReturnsFalse_WhenEitherPointDoesNotExist()
    {
        _dijkstraMap.AddPoint(0);

        _dijkstraMap.RemoveConnection(0, 1).Should().BeFalse();
        _dijkstraMap.RemoveConnection(1, 0).Should().BeFalse();
        _dijkstraMap.HasConnection(0, 1).Should().BeFalse();
        _dijkstraMap.HasConnection(1, 0).Should().BeFalse();
    }
    
    [Fact]
    public void RemoveConnection_OnlyRemovesOneDirection_WhenBidirectionalIsFalse()
    {
        _dijkstraMap.AddPoint(0);
        _dijkstraMap.AddPoint(1);
        _dijkstraMap.ConnectPoints(0, 1).Should().BeTrue();
        
        _dijkstraMap.RemoveConnection(1, 0, false).Should().BeTrue();
        _dijkstraMap.HasConnection(1, 0).Should().BeFalse();
        _dijkstraMap.HasConnection(0, 1).Should().BeTrue();
    }
}