using System.Collections.Generic;
using FluentAssertions;
using Godot;
using Xunit;

namespace low_age_tests
{
    public partial class Rect2ExtensionsTests
    {
        public static IEnumerable<object[]> GetExpectedRectsByRectsToTrimToVector2()
        {
            yield return new object[]
            {
                new Rect2(new Vector2(0, 0), new Vector2(1, 1)),
                new Vector2(1, 1),
                // Expected:
                new Rect2(new Vector2(0, 0), new Vector2(1, 1))
            };
            
            yield return new object[]
            {
                new Rect2(new Vector2(-1, -6), new Vector2(1, 1)),
                new Vector2(1, 1),
                // Expected:
                new Rect2(new Vector2(0, 0), new Vector2(1, 1))
            };
            
            yield return new object[]
            {
                new Rect2(new Vector2(0, 0), new Vector2(3, 2)),
                new Vector2(1, 1),
                // Expected:
                new Rect2(new Vector2(0, 0), new Vector2(1, 1))
            };
            
            yield return new object[]
            {
                new Rect2(new Vector2(0, 0), new Vector2(4, 2)),
                new Vector2(3, 2),
                // Expected:
                new Rect2(new Vector2(0, 0), new Vector2(3, 2))
            };
            
            yield return new object[]
            {
                new Rect2(new Vector2(0, 0), new Vector2(2, 2)),
                new Vector2(3, 2),
                // Expected:
                new Rect2(new Vector2(0, 0), new Vector2(2, 2))
            };
            
            yield return new object[]
            {
                new Rect2(new Vector2(0, 0), new Vector2(2, 3)),
                new Vector2(2, 3),
                // Expected:
                new Rect2(new Vector2(0, 0), new Vector2(2, 3))
            };
            
            yield return new object[]
            {
                new Rect2(new Vector2(3, 6), new Vector2(3, 2)),
                new Vector2(3, 2),
                // Expected:
                new Rect2(new Vector2(3, 6), new Vector2(3, 2))
            };
        }

        [Theory]
        [MemberData(nameof(GetExpectedRectsByRectsToTrimToVector2))]
        public void TrimTo_ShouldCorrectlyTrimRect_ToGivenBoundarySize(Rect2 rect2, 
            Vector2 size, Rect2 expected)
        {
            rect2.TrimTo(size).Should().Be(expected);
        }
    }
}