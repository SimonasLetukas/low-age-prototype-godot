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
        
        public static IEnumerable<object[]> GetExpectedRectsForExceptMethod()
        {
            yield return new object[]
            {
                Vector2.Zero,
                Array.Empty<Vector2>(),
                // Expected:
                new Rect2()
            };
            
            yield return new object[]
            {
                // x.
                // x.
                new Vector2(2, 2),
                new[]
                {
                    new Vector2(1, 1),
                    new Vector2(1, 0)
                },
                // Expected:
                new Rect2(0, 0, 1, 2)
            };
            
            yield return new object[]
            {
                // xxx
                // ...
                new Vector2(3, 2),
                new[]
                {
                    new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1),
                },
                // Expected:
                new Rect2(0, 0, 3, 1)
            };
            
            yield return new object[]
            {
                // ..x
                // ..x
                new Vector2(3, 2),
                new[]
                {
                    new Vector2(0, 0), new Vector2(1, 0),
                    new Vector2(0, 1), new Vector2(1, 1),
                },
                // Expected:
                new Rect2(2, 0, 1, 2)
            };
            
            yield return new object[]
            {
                // ....
                // xxxx
                new Vector2(4, 2),
                new[]
                {
                    new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(3, 0), 
                },
                // Expected:
                new Rect2(0, 1, 4, 1)
            };
            
            yield return new object[]
            {
                // xxx
                // xxx
                // xx.
                new Vector2(3, 3),
                new[]
                {
                    new Vector2(2, 2),
                },
                // Expected:
                new Rect2(0, 0, 3, 3)
            };
            
            yield return new object[]
            {
                // ..x
                // x.x
                // .x.
                new Vector2(3, 3),
                new[]
                {
                    new Vector2(0, 0), new Vector2(1, 0), 
                    new Vector2(1, 1), 
                    new Vector2(0, 2), new Vector2(2, 2), 
                },
                // Expected:
                new Rect2(0, 0, 3, 3)
            };
            
            yield return new object[]
            {
                // ..x
                // ...
                // x..
                new Vector2(3, 3),
                new[]
                {
                    new Vector2(0, 0), new Vector2(1, 0), 
                    new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), 
                    new Vector2(1, 2), new Vector2(2, 2), 
                },
                // Expected:
                new Rect2(0, 0, 3, 3)
            };
        }
        
        [Theory]
        [MemberData(nameof(GetExpectedRectsForExceptMethod))]
        public void Except_ShouldCorrectlyConvertToRect_GivenSizeAndListOfVector2(Vector2 size, 
            IList<Vector2> listOfVector2, Rect2 expected)
        {
            size.Except(listOfVector2).Should().Be(expected);
        }
        
        public static IEnumerable<object[]> GetExpectedResultsForDiagonalToMethod()
        {
            yield return new object[]
            {
                new Vector2(0, 0),
                new Vector2(0, 0),
                // Expected:
                true
            };
            
            yield return new object[]
            {
                new Vector2(-1, 2),
                new Vector2(-2, 1),
                // Expected:
                true
            };
            
            yield return new object[]
            {
                new Vector2(3, -2),
                new Vector2(3, -3),
                // Expected:
                false
            };
            
            yield return new object[]
            {
                new Vector2(5, 0),
                new Vector2(6, -1),
                // Expected:
                true
            };
            
            yield return new object[]
            {
                new Vector2(5, 3),
                new Vector2(6, 3),
                // Expected:
                false
            };
            
            yield return new object[]
            {
                new Vector2(3, 6),
                new Vector2(4, 7),
                // Expected:
                true
            };
            
            yield return new object[]
            {
                new Vector2(-5, 4),
                new Vector2(-5, 5),
                // Expected:
                false
            };
            
            yield return new object[]
            {
                new Vector2(-6, -2),
                new Vector2(-7, -1),
                // Expected:
                true
            };
            
            yield return new object[]
            {
                new Vector2(-1, -5),
                new Vector2(-2, -5),
                // Expected:
                false
            };
            
            yield return new object[]
            {
                new Vector2(-6, -7),
                new Vector2(-4, -7),
                // Expected:
                false
            };
            
            yield return new object[]
            {
                new Vector2(3, -7),
                new Vector2(5, -5),
                // Expected:
                true
            };
            
            yield return new object[]
            {
                new Vector2(1, -3),
                new Vector2(1, 2),
                // Expected:
                false
            };
            
            yield return new object[]
            {
                new Vector2(-3, -2),
                new Vector2(-4, 0),
                // Expected:
                false
            };
            
            yield return new object[]
            {
                new Vector2(5, 4),
                new Vector2(10, 5),
                // Expected:
                false
            };
            
            yield return new object[]
            {
                new Vector2(10, -4),
                new Vector2(2, 4),
                // Expected:
                true
            };
            
            yield return new object[]
            {
                new Vector2(10, -3),
                new Vector2(2, 6),
                // Expected:
                false
            };
            
            yield return new object[]
            {
                new Vector2(0, 0),
                new Vector2(100, 100),
                // Expected:
                true
            };
        }
        
        [Theory]
        [MemberData(nameof(GetExpectedResultsForDiagonalToMethod))]
        public void IsDiagonalTo_ShouldReturnCorrectResponse_WithGivenInput(Vector2 source, Vector2 point, 
            bool expectedResult)
        {
            source.IsDiagonalTo(point).Should().Be(expectedResult);
        }
    }
}