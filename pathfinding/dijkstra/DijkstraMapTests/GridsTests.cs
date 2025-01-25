using FluentAssertions;
using DijkstraMap;
using DijkstraMap.Methods;
using LowAgeCommon;

namespace DijkstraMapTests;

public class GridsTests
{
    private readonly Dijkstra _dijkstraMap = new();

    [Theory]
    [InlineData(5, 5, 3, 2, null)]
    [InlineData(50, 60, 1, -30, 1f)]
    public void AddSquareGrid_ShouldCreateGrid_WithGivenInput(int width, int height, int xOffset, int yOffset, float? diagonalCost)
    {
        var pointsByPosition = _dijkstraMap.AddSquareGrid(width, height, 
            new Vector2<int>(xOffset, yOffset), Terrain.Default, 1f, diagonalCost);
        
        for (var x = xOffset; x < width + xOffset; x++)
        {
            for (var y = yOffset; y < height + yOffset; y++)
            {
                var position = new Vector2<int>(x, y);
                pointsByPosition.Should().ContainKey(position);
                var pointId = pointsByPosition[position];

                var topPosition = position + new Vector2<int>(0, 1);
                if (pointsByPosition.TryGetValue(topPosition, out var topPointId)) 
                    _dijkstraMap.HasConnection(pointId, topPointId).Should().BeTrue();
                
                var bottomPosition = position + new Vector2<int>(0, -1);
                if (pointsByPosition.TryGetValue(bottomPosition, out var bottomPointId)) 
                    _dijkstraMap.HasConnection(pointId, bottomPointId).Should().BeTrue();
                
                var rightPosition = position + new Vector2<int>(1, 0);
                if (pointsByPosition.TryGetValue(rightPosition, out var rightPointId)) 
                    _dijkstraMap.HasConnection(pointId, rightPointId).Should().BeTrue();
                
                var leftPosition = position + new Vector2<int>(-1, 0);
                if (pointsByPosition.TryGetValue(leftPosition, out var leftPointId)) 
                    _dijkstraMap.HasConnection(pointId, leftPointId).Should().BeTrue();
                
                var topRightPosition = position + new Vector2<int>(1, 1);
                if (pointsByPosition.TryGetValue(topRightPosition, out var topRightPointId)
                    && diagonalCost != null) 
                    _dijkstraMap.HasConnection(pointId, topRightPointId).Should().BeTrue();
                
                var topLeftPosition = position + new Vector2<int>(-1, 1);
                if (pointsByPosition.TryGetValue(topLeftPosition, out var topLeftPointId)
                    && diagonalCost != null) 
                    _dijkstraMap.HasConnection(pointId, topLeftPointId).Should().BeTrue();
                
                var bottomRightPosition = position + new Vector2<int>(1, -1);
                if (pointsByPosition.TryGetValue(bottomRightPosition, out var bottomRightPointId)
                    && diagonalCost != null) 
                    _dijkstraMap.HasConnection(pointId, bottomRightPointId).Should().BeTrue();
                
                var bottomLeftPosition = position + new Vector2<int>(-1, -1);
                if (pointsByPosition.TryGetValue(bottomLeftPosition, out var bottomLeftPointId)
                    && diagonalCost != null) 
                    _dijkstraMap.HasConnection(pointId, bottomLeftPointId).Should().BeTrue();
            }
        }
    }
}