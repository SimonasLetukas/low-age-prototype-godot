using System;
using System.Collections.Generic;
using FluentAssertions;
using Godot;
using Xunit;

namespace low_age_tests
{
    public class Vector2ExtensionsTests
    {
        public static IEnumerable<object[]> GetExpectedRectsByListOfVector2()
        {
            yield return new object[]
            {
                Array.Empty<Vector2>(),
                // Expected:
                Array.Empty<Rect2>()
            };
            
            yield return new object[]
            {
                new[]
                {
                    new Vector2(1, 1),
                    new Vector2(11, 11), new Vector2(12, 11),
                    new Vector2(11, 12), new Vector2(12, 12),
                    new Vector2(30, 30), new Vector2(31, 30),
                    new Vector2(30, 31), new Vector2(31, 31),
                },
                // Expected:
                new[]
                {
                    new Rect2(new Vector2(1, 1), new Vector2(1, 1)),
                    new Rect2(new Vector2(11, 11), new Vector2(2, 2)),
                    new Rect2(new Vector2(30, 30), new Vector2(2, 2)),
                }
            };
            
            yield return new object[]
            {
                new[]
                {
                    new Vector2(17, 8),
                    new Vector2(16, 9), new Vector2(17, 9),
                    new Vector2(16, 10), new Vector2(17, 10),
                    new Vector2(16, 11), new Vector2(17, 11),
                    new Vector2(16, 12), new Vector2(17, 12), new Vector2(18, 12),
                    new Vector2(16, 13), new Vector2(17, 13), new Vector2(18, 13),
                    new Vector2(16, 14), new Vector2(17, 14), new Vector2(18, 14),
                    new Vector2(16, 15), new Vector2(17, 15), new Vector2(18, 15),
                },
                // Expected:
                new[]
                {
                    new Rect2(new Vector2(17, 8), new Vector2(1, 1)),
                    new Rect2(new Vector2(16, 9), new Vector2(2, 2)),
                    new Rect2(new Vector2(16, 11), new Vector2(2, 2)),
                    new Rect2(new Vector2(18, 12), new Vector2(1, 1)),
                    new Rect2(new Vector2(16, 13), new Vector2(3, 3)),
                }
            };
            
            yield return new object[]
            {
                new[]
                {
                    new Vector2(12, 18), new Vector2(13, 18),
                    new Vector2(12, 19), new Vector2(13, 19), new Vector2(14, 19), new Vector2(15, 19),
                    new Vector2(13, 20), new Vector2(14, 20), new Vector2(15, 20),
                    new Vector2(13, 21), new Vector2(14, 21), new Vector2(15, 21),
                    new Vector2(14, 22), new Vector2(15, 22),
                },
                // Expected:
                new[]
                {
                    new Rect2(new Vector2(12, 18), new Vector2(2, 2)),
                    new Rect2(new Vector2(14, 19), new Vector2(2, 2)),
                    new Rect2(new Vector2(13, 20), new Vector2(1, 1)),
                    new Rect2(new Vector2(13, 21), new Vector2(1, 1)),
                    new Rect2(new Vector2(14, 21), new Vector2(2, 2)),
                }
            };
        }
        
        [Theory]
        [MemberData(nameof(GetExpectedRectsByListOfVector2))]
        public void ToSquareRects_ShouldCorrectlyConvertToRects_GivenListOfVector2(IList<Vector2> listOfVector2, 
            IList<Rect2> expected)
        {
            listOfVector2.ToSquareRects().Should().BeEquivalentTo(expected);
        }
    }
}