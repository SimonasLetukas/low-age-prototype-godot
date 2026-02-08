using FluentAssertions;
using LowAgeCommon.Extensions;

namespace LowAgeTests
{
    public class EnumExtensionsTests
    {
        public static IEnumerable<object[]> GetExpectedCountsByGivenRotations()
        {
            yield return new object[]
            {
                IsometricRotation.BottomRight,
                IsometricRotation.BottomLeft,
                // Expected:
                1
            };
            
            yield return new object[]
            {
                IsometricRotation.BottomRight,
                IsometricRotation.TopLeft,
                // Expected:
                2
            };
            
            yield return new object[]
            {
                IsometricRotation.BottomRight,
                IsometricRotation.TopRight,
                // Expected:
                3
            };
            
            yield return new object[]
            {
                IsometricRotation.BottomRight,
                IsometricRotation.BottomRight,
                // Expected:
                0
            };
            
            yield return new object[]
            {
                IsometricRotation.TopRight,
                IsometricRotation.BottomRight,
                // Expected:
                1
            };
            
            yield return new object[]
            {
                IsometricRotation.TopLeft,
                IsometricRotation.BottomRight,
                // Expected:
                2
            };
            
            yield return new object[]
            {
                IsometricRotation.BottomLeft,
                IsometricRotation.BottomRight,
                // Expected:
                3
            };
            
            yield return new object[]
            {
                IsometricRotation.TopRight,
                IsometricRotation.TopLeft,
                // Expected:
                3
            };
        }

        [Theory]
        [MemberData(nameof(GetExpectedCountsByGivenRotations))]
        public void CountTo_ShouldReturnCorrectResult_ForGivenRotations(IsometricRotation rotation, 
            IsometricRotation targetRotation, int expectedCount)
        {
            rotation.CountTo(targetRotation).Should().Be(expectedCount);
        }
    }
}