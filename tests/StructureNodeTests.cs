using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Godot;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Shared;
using Xunit;
using Area = low_age_data.Domain.Shared.Area;

namespace TestProject1
{
    public class StructureNodeTests
    {
        private readonly IFixture _fixture = new Fixture();

        public static IEnumerable<object[]> GetExpectedRotationResultsByInitialStructureConfiguration()
        {
            /*yield return new object[]
            {
                (new Vector2<int>(), new Vector2<int>(), new List<Area>()),
                
                // Expected after 1 rotation:
                (new Vector2(), new Vector2(), new List<Rect2>()),
                // Expected after 2 rotations:
                (new Vector2(), new Vector2(), new List<Rect2>()),
                // Expected after 3 rotations:
                (new Vector2(), new Vector2(), new List<Rect2>()),
            };*/
            
          /* x -1 0 1 2  x -1 0 1 2  x -1 0 1 2  x -1 0 1 2
            y           y           y           y
            -1          -1          -1          -1
            0     o     0     o     0     o     0     o
            1           1           1           1
            2           2           2           2
            |			|           |           |
  rotation: | bot-right | bot-left  | top-left  | top-right
o center:   | 0,0		| 0,0       | 0,0       | 0,0
x size:		| 1,1	    | 1,1	    | 1,1       | 1,1 */
            yield return new object[]
            {
                (new Vector2<int>(1, 1), new Vector2<int>(0, 0), new List<Area>()),
                
                // Expected after 1 rotation:
                (new Vector2(1, 1), new Vector2(0, 0), new List<Rect2>()),
                // Expected after 2 rotations:
                (new Vector2(1, 1), new Vector2(0, 0), new List<Rect2>()),
                // Expected after 3 rotations:
                (new Vector2(1, 1), new Vector2(0, 0), new List<Rect2>()),
            };
        }
        
        [Theory]
        [MemberData(nameof(GetExpectedRotationResultsByInitialStructureConfiguration))]
        public void Rotate_ShouldHaveExpectedConfiguration_AfterEachRotation(
            (Vector2<int> Size, Vector2<int> CenterPoint, List<Area> WalkableAreas) initialValues,
            (Vector2 Size, Vector2 CenterPoint, List<Rect2> WalkableAreas) expectedAfter1Rotation,
            (Vector2 Size, Vector2 CenterPoint, List<Rect2> WalkableAreas) expectedAfter2Rotations,
            (Vector2 Size, Vector2 CenterPoint, List<Rect2> WalkableAreas) expectedAfter3Rotations)
        {
            var structure = StructureNode.Instance();
            var blueprint = _fixture.Build<Structure>()
                .With(x => x.Size, initialValues.Size)
                .With(x => x.CenterPoint, initialValues.CenterPoint)
                .With(x => x.WalkableAreas, initialValues.WalkableAreas)
                .Create();
            structure.SetBlueprint(blueprint);
            (Vector2 Size, Vector2 CenterPoint, List<Rect2> WalkableAreas) expectedAfter4Rotations =
                (structure.StructureSize, structure.CenterPoint, structure.WalkableAreas);

            structure.Rotate();

            structure.StructureSize.Should().Be(expectedAfter1Rotation.Size);
            structure.CenterPoint.Should().Be(expectedAfter1Rotation.CenterPoint);
            structure.WalkableAreas.Should().BeEquivalentTo(expectedAfter1Rotation.WalkableAreas);
            
            structure.Rotate();
            
            structure.StructureSize.Should().Be(expectedAfter2Rotations.Size);
            structure.CenterPoint.Should().Be(expectedAfter2Rotations.CenterPoint);
            structure.WalkableAreas.Should().BeEquivalentTo(expectedAfter2Rotations.WalkableAreas);
            
            structure.Rotate();
            
            structure.StructureSize.Should().Be(expectedAfter3Rotations.Size);
            structure.CenterPoint.Should().Be(expectedAfter3Rotations.CenterPoint);
            structure.WalkableAreas.Should().BeEquivalentTo(expectedAfter3Rotations.WalkableAreas);
            
            structure.Rotate();
            
            structure.StructureSize.Should().Be(expectedAfter4Rotations.Size);
            structure.CenterPoint.Should().Be(expectedAfter4Rotations.CenterPoint);
            structure.WalkableAreas.Should().BeEquivalentTo(expectedAfter4Rotations.WalkableAreas);
        }
    }
}