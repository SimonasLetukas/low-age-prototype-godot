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
                ActorRotation.BottomRight,
                ActorRotation.BottomLeft,
                // Expected:
                1
            };
            
            yield return new object[]
            {
                ActorRotation.BottomRight,
                ActorRotation.TopLeft,
                // Expected:
                2
            };
            
            yield return new object[]
            {
                ActorRotation.BottomRight,
                ActorRotation.TopRight,
                // Expected:
                3
            };
            
            yield return new object[]
            {
                ActorRotation.BottomRight,
                ActorRotation.BottomRight,
                // Expected:
                0
            };
            
            yield return new object[]
            {
                ActorRotation.TopRight,
                ActorRotation.BottomRight,
                // Expected:
                1
            };
            
            yield return new object[]
            {
                ActorRotation.TopLeft,
                ActorRotation.BottomRight,
                // Expected:
                2
            };
            
            yield return new object[]
            {
                ActorRotation.BottomLeft,
                ActorRotation.BottomRight,
                // Expected:
                3
            };
            
            yield return new object[]
            {
                ActorRotation.TopRight,
                ActorRotation.TopLeft,
                // Expected:
                3
            };
        }

        [Theory]
        [MemberData(nameof(GetExpectedCountsByGivenRotations))]
        public void CountTo_ShouldReturnCorrectResult_ForGivenRotations(ActorRotation rotation, 
            ActorRotation targetRotation, int expectedCount)
        {
            rotation.CountTo(targetRotation).Should().Be(expectedCount);
        }
    }
}