using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Godot;
using low_age_data.Domain.Common;
using low_age_data.Domain.Entities.Actors.Structures;
using TestProject1.Helpers;
using Xunit;
using Area = low_age_data.Domain.Common.Area;
using Array = System.Array;

namespace low_age_tests
{
    public class StructureNodeTests
    {
        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization
            {
                ConfigureMembers = true
            });

        private readonly FixtureCustomization<Structure> _blueprint;
        private readonly StructureNode _structure;

        public StructureNodeTests()
        {
            Data.Instance.ReadBlueprint();
            _blueprint = _fixture
                .For<Structure>()
                .With(x => x.Sprite, "res://assets/sprites/structures/revs/boss post front indexed 2x3.png");
            _structure = StructureNode.Instance();
            _structure._Ready();
        }
        
        public static IEnumerable<object[]> GetExpectedRotationResultsByInitialStructureConfiguration()
        { 
/*           x -1 0 1 2  x -1 0 1 2  x -1 0 1 2  x -1 0 1 2
            y           y           y           y
            -1          -1          -1          -1
            0     o     0     o     0     o     0     o
            1           1           1           1
            2           2           2           2
            |			|           |           |
  rotation: | bot-right | bot-left  | top-left  | top-right
x size:		| 1,1	    | 1,1	    | 1,1       | 1,1
o center:   | 0,0		| 0,0       | 0,0       | 0,0 */
            yield return new object[]
            {
                // Initial values from blueprint:
                (new Vector2<int>(1, 1), new Vector2<int>(0, 0), Array.Empty<Area>()),
                
                // Expected after 1 rotation:
                (new Vector2(1, 1), new Vector2(0, 0), Array.Empty<Rect2>()),
                // Expected after 2 rotations:
                (new Vector2(1, 1), new Vector2(0, 0), Array.Empty<Rect2>()),
                // Expected after 3 rotations:
                (new Vector2(1, 1), new Vector2(0, 0), Array.Empty<Rect2>()),
            };
            
/*			 x -1 0 1 2  x -1 0 1 2  x -1 0 1 2  x -1 0 1 2 
			y           y           y           y
			-1          -1          -1          -1         
			0     o     0     x x o 0     x     0     o x x    
			1     x     1           1     x     1        
			2     x     2           2     o     2          
			|			|           |           |
  rotation: | bot-right | bot-left  | top-left  | top-right
x size:		| 1,3		| 3,1		| 1,3	    | 3,1
o center:   | 0,0		| 2,0       | 0,2       | 0,0 */
            yield return new object[]
            {
                // Initial values from blueprint:
                (new Vector2<int>(1, 3), new Vector2<int>(0, 0), Array.Empty<Area>()),
                
                // Expected after 1 rotation:
                (new Vector2(3, 1), new Vector2(2, 0), Array.Empty<Rect2>()),
                // Expected after 2 rotations:
                (new Vector2(1, 3), new Vector2(0, 2), Array.Empty<Rect2>()),
                // Expected after 3 rotations:
                (new Vector2(3, 1), new Vector2(0, 0), Array.Empty<Rect2>()),
            };
            
/*			 x -1 0 1 2  x -1 0 1 2  x -1 0 1 2  x -1 0 1 2 	Calculate walkable with negative
			y           y           y           y				coordinates, but trim them down
			-1          -1          -1      .   -1         		in the final coordinates.
			0     . x   0   . . .   0     o .   0     x o    	
			1     . o   1     o x   1     x .   1     . . .  	Unsure if this changes anything --
			2     .     2           2           2          		probably not.
			|			|           |           |
  rotation: | bot-right | bot-left  | top-left  | top-right
x size:		| 2,2		| 2,2		| 2,2	    | 2,2
o center:   | 1,1		| 0,1       | 0,0       | 1,0
. walkable: | 0,0;1,2  	| 0,0;2,1   | 1,0;1,2   | 0,1;2,1 */
            yield return new object[]
            {
                // Initial values from blueprint:
                (new Vector2<int>(2, 2), new Vector2<int>(1, 1), new[]
                {
                    new Area(new Vector2<int>(0, 0), new Vector2<int>(1, 3))
                }),

                // Expected after 1 rotation:
                (new Vector2(2, 2), new Vector2(0, 1), new[]
                {
                    new Rect2(new Vector2(0, 0), new Vector2(2, 1))
                }),
                // Expected after 2 rotations:
                (new Vector2(2, 2), new Vector2(0, 0), new[]
                {
                    new Rect2(new Vector2(1, 0), new Vector2(1, 2))
                }),
                // Expected after 3 rotations:
                (new Vector2(2, 2), new Vector2(1, 0), new[]
                {
                    new Rect2(new Vector2(0, 1), new Vector2(2, 1))
                }),
            };
            
/*			 x -1 0 1 2  x -1 0 1 2  x -1 0 1 2  x -1 0 1 2 
			y           y           y           y
			-1          -1          -1          -1         
			0     . o x 0     . .   0     x x . 0     x x  
			1     . x x 1     x o   1     x o . 1     o x  
			2           2     x x   2           2     . .  
			|			|           |           |
  rotation: | bot-right | bot-left  | top-left  | top-right
x size:		| 3,2		| 2,3		| 3,2	    | 2,3
o center:   | 1,0		| 1,1       | 1,1       | 0,1
. walkable: | 0,0;1,2  	| 0,0;2,1   | 2,0;1,2   | 0,2;2,1 */
            yield return new object[]
            {
                // Initial values from blueprint:
                (new Vector2<int>(3, 2), new Vector2<int>(1, 0), new[]
                {
                    new Area(new Vector2<int>(0, 0), new Vector2<int>(1, 2))
                }),

                // Expected after 1 rotation:
                (new Vector2(2, 3), new Vector2(1, 1), new[]
                {
                    new Rect2(new Vector2(0, 0), new Vector2(2, 1))
                }),
                // Expected after 2 rotations:
                (new Vector2(3, 2), new Vector2(1, 1), new[]
                {
                    new Rect2(new Vector2(2, 0), new Vector2(1, 2))
                }),
                // Expected after 3 rotations:
                (new Vector2(2, 3), new Vector2(0, 1), new[]
                {
                    new Rect2(new Vector2(0, 2), new Vector2(2, 1))
                }),
            };
            
/*			 x -1 0 1 2  x -1 0 1 2  x -1 0 1 2  x -1 0 1 2 
			y           y           y           y
			-1          -1          -1          -1         
			0     . x x 0     x . . 0     . x x 0     x o .  
			1     . x o 1     x x x 1     o x . 1     x x x  
			2     x x . 2     . o x 2     x x . 2     . . x  
			|			|           |           |
  rotation: | bot-right | bot-left  | top-left  | top-right
x size:		| 3,3		| 3,3		| 3,3	    | 3,3
o center:   | 2,1		| 1,2       | 0,1       | 1,0
. walkable: | 0,0;1,2  	| 1,0;2,1   | 2,1;1,2   | 0,2;2,1
. walkable: | 2,2;1,1  	| 0,2;1,1   | 0,0;1,1   | 2,0;1,1 */
            yield return new object[]
            {
                // Initial values from blueprint:
                (new Vector2<int>(3, 3), new Vector2<int>(2, 1), new[]
                {
                    new Area(new Vector2<int>(0, 0), new Vector2<int>(1, 2)),
                    new Area(new Vector2<int>(2, 2), new Vector2<int>(1, 1)),
                }),
                
                // Expected after 1 rotation:
                (new Vector2(3, 3), new Vector2(1, 2), new[]
                {
                    new Rect2(new Vector2(1, 0), new Vector2(2, 1)),
                    new Rect2(new Vector2(0, 2), new Vector2(1, 1)),
                }),
                // Expected after 2 rotations:
                (new Vector2(3, 3), new Vector2(0, 1), new[]
                {
                    new Rect2(new Vector2(2, 1), new Vector2(1, 2)),
                    new Rect2(new Vector2(0, 0), new Vector2(1, 1)),
                }),
                // Expected after 3 rotations:
                (new Vector2(3, 3), new Vector2(1, 0), new[]
                {
                    new Rect2(new Vector2(0, 2), new Vector2(2, 1)),
                    new Rect2(new Vector2(2, 0), new Vector2(1, 1)),
                }),
            };
            
/*			 x -1 0 1 2  x -1 0 1 2  x -1 0 1 2  x -1 0 1 2 
			y           y           y           y
			-1          -1          -1          -1         
			0     . . . 0     x . . 0     x x x 0     . . x		
			1     . o . 1     x o . 1     . o . 1     . o x 	
			2     x x x 2     x . . 2     . . . 2     . . x 	
			|			|           |           |				
  rotation: | bot-right | bot-left  | top-left  | top-right			
x size:		| 3,3		| 3,3		| 3,3	    | 3,3				
o center:   | 1,1		| 1,1       | 1,1       | 1,1
. walkable: | 0,0;3,2  	| 1,0;2,3   | 0,1;3,2   | 0,0;2,3 */
            yield return new object[]
            {
                // Initial values from blueprint:
                (new Vector2<int>(3, 3), new Vector2<int>(1, 1), new[]
                {
                    new Area(new Vector2<int>(0, 0), new Vector2<int>(3, 2)),
                }),
                
                // Expected after 1 rotation:
                (new Vector2(3, 3), new Vector2(1, 1), new[]
                {
                    new Rect2(new Vector2(1, 0), new Vector2(2, 3)),
                }),
                // Expected after 2 rotations:
                (new Vector2(3, 3), new Vector2(1, 1), new[]
                {
                    new Rect2(new Vector2(0, 1), new Vector2(3, 2)),
                }),
                // Expected after 3 rotations:
                (new Vector2(3, 3), new Vector2(1, 1), new[]
                {
                    new Rect2(new Vector2(0, 0), new Vector2(2, 3)),
                }),
            };
        }
        
        [Theory]
        [MemberData(nameof(GetExpectedRotationResultsByInitialStructureConfiguration))]
        public void Rotate_ShouldHaveExpectedConfiguration_AfterEachRotation(
            (Vector2<int> Size, Vector2<int> CenterPoint, Area[] WalkableAreas) initialValues,
            (Vector2 Size, Vector2 CenterPoint, Rect2[] WalkableAreas) expectedAfter1Rotation,
            (Vector2 Size, Vector2 CenterPoint, Rect2[] WalkableAreas) expectedAfter2Rotations,
            (Vector2 Size, Vector2 CenterPoint, Rect2[] WalkableAreas) expectedAfter3Rotations)
        {
            var blueprint = _blueprint
                .With(x => x.Size, initialValues.Size)
                .With(x => x.CenterPoint, initialValues.CenterPoint)
                .With(x => x.WalkableAreas, initialValues.WalkableAreas.ToList())
                .Create();
            _structure.SetBlueprint(blueprint);
            (Vector2 Size, Vector2 CenterPoint, IList<Rect2> WalkableAreas) expectedAfter4Rotations = 
                (_structure.EntitySize, _structure.CenterPoint, _structure.WalkableAreasBlueprint);

            _structure.Rotate();

            _structure.EntitySize.Should().Be(expectedAfter1Rotation.Size);
            _structure.CenterPoint.Should().Be(expectedAfter1Rotation.CenterPoint);
            _structure.WalkableAreasBlueprint.Should().BeEquivalentTo(expectedAfter1Rotation.WalkableAreas);
            
            _structure.Rotate();
            
            _structure.EntitySize.Should().Be(expectedAfter2Rotations.Size);
            _structure.CenterPoint.Should().Be(expectedAfter2Rotations.CenterPoint);
            _structure.WalkableAreasBlueprint.Should().BeEquivalentTo(expectedAfter2Rotations.WalkableAreas);
            
            _structure.Rotate();
            
            _structure.EntitySize.Should().Be(expectedAfter3Rotations.Size);
            _structure.CenterPoint.Should().Be(expectedAfter3Rotations.CenterPoint);
            _structure.WalkableAreasBlueprint.Should().BeEquivalentTo(expectedAfter3Rotations.WalkableAreas);
            
            _structure.Rotate();
            
            _structure.EntitySize.Should().Be(expectedAfter4Rotations.Size);
            _structure.CenterPoint.Should().Be(expectedAfter4Rotations.CenterPoint);
            _structure.WalkableAreasBlueprint.Should().BeEquivalentTo(expectedAfter4Rotations.WalkableAreas);
        }

        [Fact]
        public void Rotate_ShouldHaveExpectedRotationEnum_AfterEachRotation()
        {
            _structure.SetBlueprint(_blueprint.Create());

            _structure.ActorRotation.Should().Be(ActorRotation.BottomRight);
            
            _structure.Rotate();
            _structure.ActorRotation.Should().Be(ActorRotation.BottomLeft);
            
            _structure.Rotate();
            _structure.ActorRotation.Should().Be(ActorRotation.TopLeft);
            
            _structure.Rotate();
            _structure.ActorRotation.Should().Be(ActorRotation.TopRight);
            
            _structure.Rotate();
            _structure.ActorRotation.Should().Be(ActorRotation.BottomRight);
        }
    }
}