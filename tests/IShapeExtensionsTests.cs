using FluentAssertions;
using LowAgeData.Domain.Common.Shape;
using LowAgeCommon;

namespace LowAgeTests
{
    public partial class IShapeExtensionsTests
    {
        public static IEnumerable<object[]> GetExpectedPositionsByMapShape()
        {
            yield return new object[]
            {
                new LowAgeData.Domain.Common.Shape.Map(), 
                new Vector2Int(0, 0),
                new Vector2Int(1, 1),
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0)
                }
            };
            
            yield return new object[]
            {
                new LowAgeData.Domain.Common.Shape.Map(), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 2),
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0),
                    new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(3, 1)
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetExpectedPositionsByMapShape))]
        public void ToPositions_ShouldReturnExpectedPositions_WhenShapeIsMap(LowAgeData.Domain.Common.Shape.Map map,
            Vector2Int centerPoint, Vector2Int mapSize, Vector2Int[] expectedPositions)
        {
            map.ToPositions(centerPoint, mapSize).Should().BeEquivalentTo(expectedPositions);
        }
        
        /*
        m centerPoint-included
        q centerPoint-excluded
        w size-included
        o size-excluded
        x included
        . excluded
         */
        public static IEnumerable<object[]> GetExpectedPositionsByLineShape()
        {
            /*
            mxx
             */
            yield return new object[]
            {
                new Line(2), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),
                ActorRotation.BottomRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), 
                }
            };
            
            /*
            m
            x
            x
             */
            yield return new object[]
            {
                new Line(2), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),
                ActorRotation.BottomLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), 
                    new Vector2Int(0, 1), 
                    new Vector2Int(0, 2), 
                }
            };
            
            /*
            xxm
             */
            yield return new object[]
            {
                new Line(2), 
                new Vector2Int(2, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),
                ActorRotation.TopLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), 
                }
            };
            
            /*
            x
            x
            m
             */
            yield return new object[]
            {
                new Line(2), 
                new Vector2Int(0, 2),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),
                ActorRotation.TopRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), 
                    new Vector2Int(0, 1), 
                    new Vector2Int(0, 2), 
                }
            };
            
            /*
            qxx
             */
            yield return new object[]
            {
                new Line(2, 0), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),
                ActorRotation.BottomRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(1, 0), new Vector2Int(2, 0), 
                }
            };
            
            /*
            q
            x
            x
             */
            yield return new object[]
            {
                new Line(2, 0), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),
                ActorRotation.BottomLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 1), 
                    new Vector2Int(0, 2), 
                }
            };
            
            /*
            xxq
             */
            yield return new object[]
            {
                new Line(2, 0), 
                new Vector2Int(2, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),
                ActorRotation.TopLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0),
                }
            };
            
            /*
            x
            x
            q
             */
            yield return new object[]
            {
                new Line(2, 0), 
                new Vector2Int(0, 2),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),
                ActorRotation.TopRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), 
                    new Vector2Int(0, 1), 
                }
            };
            
            /*
            q.x
             */
            yield return new object[]
            {
                new Line(2, 1), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),
                ActorRotation.BottomRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(2, 0), 
                }
            };
            
            /*
            q
            .
            x
             */
            yield return new object[]
            {
                new Line(2, 1), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),
                ActorRotation.BottomLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 2), 
                }
            };
            
            /*
            x.q
             */
            yield return new object[]
            {
                new Line(2, 1), 
                new Vector2Int(2, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),
                ActorRotation.TopLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0),
                }
            };
            
            /*
            x
            .
            q
             */
            yield return new object[]
            {
                new Line(2, 1), 
                new Vector2Int(0, 2),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),
                ActorRotation.TopRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), 
                }
            };
            
            /*
            mwx
             */
            yield return new object[]
            {
                new Line(1), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(2, 1),
                ActorRotation.BottomRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), 
                }
            };
            
            /*
            m
            w
            x
             */
            yield return new object[]
            {
                new Line(1), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 2),
                ActorRotation.BottomLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), 
                    new Vector2Int(0, 1), 
                    new Vector2Int(0, 2), 
                }
            };
            
            /*
            xmw
             */
            yield return new object[]
            {
                new Line(1), 
                new Vector2Int(1, 0),
                new Vector2Int(4, 5),
                new Vector2Int(2, 1),
                ActorRotation.TopLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), 
                }
            };
            
            /*
            x
            m
            w
             */
            yield return new object[]
            {
                new Line(1), 
                new Vector2Int(0, 1),
                new Vector2Int(4, 5),
                new Vector2Int(1, 2),
                ActorRotation.TopRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), 
                    new Vector2Int(0, 1), 
                    new Vector2Int(0, 2), 
                }
            };
            
            /*
            qox
             */
            yield return new object[]
            {
                new Line(1, 0), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(2, 1),
                ActorRotation.BottomRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(2, 0), 
                }
            };
            
            /*
            q
            o
            x
             */
            yield return new object[]
            {
                new Line(1, 0), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 2),
                ActorRotation.BottomLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 2), 
                }
            };
            
            /*
            xqo
             */
            yield return new object[]
            {
                new Line(1, 0), 
                new Vector2Int(1, 0),
                new Vector2Int(4, 5),
                new Vector2Int(2, 1),
                ActorRotation.TopLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0),
                }
            };
            
            /*
            x
            q
            o
             */
            yield return new object[]
            {
                new Line(1, 0), 
                new Vector2Int(0, 1),
                new Vector2Int(4, 5),
                new Vector2Int(1, 2),
                ActorRotation.TopRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), 
                }
            };
            
            /*
            qo.x
             */
            yield return new object[]
            {
                new Line(2, 1), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(2, 1),
                ActorRotation.BottomRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(3, 0), 
                }
            };
            
            /*
            q
            o
            .
            x
             */
            yield return new object[]
            {
                new Line(2, 1), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 2),
                ActorRotation.BottomLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 3), 
                }
            };
            
            /*
            x.qo
             */
            yield return new object[]
            {
                new Line(2, 1), 
                new Vector2Int(2, 0),
                new Vector2Int(4, 5),
                new Vector2Int(2, 1),
                ActorRotation.TopLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0),
                }
            };
            
            /*
            x
            .
            q
            o
             */
            yield return new object[]
            {
                new Line(2, 1), 
                new Vector2Int(0, 2),
                new Vector2Int(4, 5),
                new Vector2Int(1, 2),
                ActorRotation.TopRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), 
                }
            };
            
            /*
            mxx
            wxx
             */
            yield return new object[]
            {
                new Line(2), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 2),
                ActorRotation.BottomRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), 
                    new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), 
                }
            };
            
            /*
            mw
            xx
            xx
             */
            yield return new object[]
            {
                new Line(2), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(2, 1),
                ActorRotation.BottomLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), 
                    new Vector2Int(0, 1), new Vector2Int(1, 1), 
                    new Vector2Int(0, 2), new Vector2Int(1, 2), 
                }
            };
            
            /*
            xxm
            xxw
             */
            yield return new object[]
            {
                new Line(2), 
                new Vector2Int(2, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 2),
                ActorRotation.TopLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), 
                    new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), 
                }
            };
            
            /*
            xx
            xx
            mw
             */
            yield return new object[]
            {
                new Line(2), 
                new Vector2Int(0, 2),
                new Vector2Int(4, 5),
                new Vector2Int(2, 1),
                ActorRotation.TopRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), 
                    new Vector2Int(0, 1), new Vector2Int(1, 1), 
                    new Vector2Int(0, 2), new Vector2Int(1, 2), 
                }
            };
            
            /*
            qxx
            oxx
             */
            yield return new object[]
            {
                new Line(2, 0), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 2),
                ActorRotation.BottomRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(1, 0), new Vector2Int(2, 0), 
                    new Vector2Int(1, 1), new Vector2Int(2, 1), 
                }
            };
            
            /*
            qo
            xx
            xx
             */
            yield return new object[]
            {
                new Line(2, 0), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(2, 1),
                ActorRotation.BottomLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 1), new Vector2Int(1, 1), 
                    new Vector2Int(0, 2), new Vector2Int(1, 2), 
                }
            };
            
            /*
            xxq
            xxo
             */
            yield return new object[]
            {
                new Line(2, 0), 
                new Vector2Int(2, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 2),
                ActorRotation.TopLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0),
                    new Vector2Int(0, 1), new Vector2Int(1, 1),
                }
            };
            
            /*
            xx
            xx
            qo
             */
            yield return new object[]
            {
                new Line(2, 0), 
                new Vector2Int(0, 2),
                new Vector2Int(4, 5),
                new Vector2Int(2, 1),
                ActorRotation.TopRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), 
                    new Vector2Int(0, 1), new Vector2Int(1, 1), 
                }
            };
            
            /*
            q.x
            o.x
             */
            yield return new object[]
            {
                new Line(2, 1), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 2),
                ActorRotation.BottomRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(2, 0), 
                    new Vector2Int(2, 1), 
                }
            };
            
            /*
            qo
            ..
            xx
             */
            yield return new object[]
            {
                new Line(2, 1), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(2, 1),
                ActorRotation.BottomLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 2), new Vector2Int(1, 2), 
                }
            };
            
            /*
            x.q
            x.o
             */
            yield return new object[]
            {
                new Line(2, 1), 
                new Vector2Int(2, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 2),
                ActorRotation.TopLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                }
            };
            
            /*
            xx
            ..
            qo
             */
            yield return new object[]
            {
                new Line(2, 1), 
                new Vector2Int(0, 2),
                new Vector2Int(4, 5),
                new Vector2Int(2, 1),
                ActorRotation.TopRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0),
                }
            };
            
            /*
            mwx
            wwx
            wwx
             */
            yield return new object[]
            {
                new Line(1), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(2, 3),
                ActorRotation.BottomRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), 
                    new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), 
                    new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2), 
                }
            };
            
            /*
            mww
            www
            xxx
             */
            yield return new object[]
            {
                new Line(1), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(3, 2),
                ActorRotation.BottomLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), 
                    new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), 
                    new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2), 
                }
            };
            
            /*
            xmw
            xww
            xww
             */
            yield return new object[]
            {
                new Line(1), 
                new Vector2Int(1, 0),
                new Vector2Int(4, 5),
                new Vector2Int(2, 3),
                ActorRotation.TopLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), 
                    new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), 
                    new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2), 
                }
            };
            
            /*
            xxx
            mww
            www
             */
            yield return new object[]
            {
                new Line(1), 
                new Vector2Int(0, 1),
                new Vector2Int(4, 5),
                new Vector2Int(3, 2),
                ActorRotation.TopRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), 
                    new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), 
                    new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2), 
                }
            };
            
            /*
            qox
            oox
            oox
             */
            yield return new object[]
            {
                new Line(1, 0), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(2, 3),
                ActorRotation.BottomRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(2, 0), 
                    new Vector2Int(2, 1), 
                    new Vector2Int(2, 2), 
                }
            };
            
            /*
            qoo
            ooo
            xxx
             */
            yield return new object[]
            {
                new Line(1, 0), 
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(3, 2),
                ActorRotation.BottomLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2), 
                }
            };
            
            /*
            xqo
            xoo
            xoo
             */
            yield return new object[]
            {
                new Line(1, 0), 
                new Vector2Int(1, 0),
                new Vector2Int(4, 5),
                new Vector2Int(2, 3),
                ActorRotation.TopLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1), 
                    new Vector2Int(0, 2), 
                }
            };
            
            /*
            xxx
            qoo
            ooo
             */
            yield return new object[]
            {
                new Line(1, 0), 
                new Vector2Int(0, 1),
                new Vector2Int(4, 5),
                new Vector2Int(3, 2),
                ActorRotation.TopRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), 
                }
            };
            
            /*
            qo.xx
            oo.xx
            oo.xx
             */
            yield return new object[]
            {
                new Line(3, 1), 
                new Vector2Int(0, 0),
                new Vector2Int(5, 5),
                new Vector2Int(2, 3),
                ActorRotation.BottomRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(3, 0), new Vector2Int(4, 0), 
                    new Vector2Int(3, 1), new Vector2Int(4, 1), 
                    new Vector2Int(3, 2), new Vector2Int(4, 2), 
                }
            };
            
            /*
            qoo
            ooo
            ...
            xxx
            xxx
             */
            yield return new object[]
            {
                new Line(3, 1), 
                new Vector2Int(0, 0),
                new Vector2Int(5, 5),
                new Vector2Int(3, 2),
                ActorRotation.BottomLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 3), new Vector2Int(1, 3), new Vector2Int(2, 3), 
                    new Vector2Int(0, 4), new Vector2Int(1, 4), new Vector2Int(2, 4), 
                }
            };
            
            /*
            xx.qo
            xx.oo
            xx.oo
             */
            yield return new object[]
            {
                new Line(3, 1), 
                new Vector2Int(3, 0),
                new Vector2Int(5, 5),
                new Vector2Int(2, 3),
                ActorRotation.TopLeft,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), 
                    new Vector2Int(0, 1), new Vector2Int(1, 1), 
                    new Vector2Int(0, 2), new Vector2Int(1, 2), 
                }
            };
            
            /*
            xxx
            xxx
            ...
            qoo
            ooo
             */
            yield return new object[]
            {
                new Line(3, 1), 
                new Vector2Int(0, 3),
                new Vector2Int(5, 5),
                new Vector2Int(3, 2),
                ActorRotation.TopRight,
                
                // Expected:
                new []
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), 
                    new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), 
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetExpectedPositionsByLineShape))]
        public void ToPositions_ShouldReturnExpectedPositions_WhenShapeIsLine(Line line, Vector2Int centerPoint, 
            Vector2Int mapSize, Vector2Int actorSize, ActorRotation actorRotation, 
            Vector2Int[] expectedPositions)
        {
            line.ToPositions(centerPoint, mapSize, actorSize, actorRotation).Should().BeEquivalentTo(expectedPositions);
        }

        /*
        m centerPoint-included
        q centerPoint-excluded
        w size-included
        o size-excluded
        x included
        . excluded
         */
        public static IEnumerable<object[]> GetExpectedPositionsByCircleShape()
        {
            /*
            m
             */
            yield return new object[]
            {
                new Circle(0, -1),
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),

                // Expected:
                new[]
                {
                    new Vector2Int(0, 0)
                }
            };
            
            /*
            
             */
            yield return new object[]
            {
                new Circle(0, 0),
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),

                // Expected:
                new Vector2Int[]{}
            };

            #region BubblyCircle

            /*
            q.x
            ..x
            xx
             */
            yield return new object[]
            {
                new Circle(2, 1),
                new Vector2Int(0, 0),
                new Vector2Int(4, 5),
                new Vector2Int(1, 1),

                // Expected:
                new[]
                {
                    new Vector2Int(2, 0), 
                    new Vector2Int(2, 1), 
                    new Vector2Int(0, 2), new Vector2Int(1, 2),
                }
            };
            
            /*
              xx
             x..
            x...
            x..q
            x...
             */
            yield return new object[]
            {
                new Circle(3, 2),
                new Vector2Int(4, 4),
                new Vector2Int(5, 6),
                new Vector2Int(1, 1),

                // Expected:
                new[]
                {
                    new Vector2Int(3, 1), new Vector2Int(4, 1), 
                    new Vector2Int(2, 2),
                    new Vector2Int(1, 3),
                    new Vector2Int(1, 4),
                    new Vector2Int(1, 5),
                }
            };
            
            /*
            xxxx
            xxxxx
            ...xxx
            ....xxx
            .....xx
            qo...xx
            oo...xx
            oo...xx
             */
            yield return new object[]
            {
                new Circle(5, 3),
                new Vector2Int(0, 5),
                new Vector2Int(7, 8),
                new Vector2Int(2, 3),

                // Expected:
                new[]
                {
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), 
                    new Vector2Int(0, 1), new Vector2Int(1, 1),  new Vector2Int(2, 1),  new Vector2Int(3, 1),  new Vector2Int(4, 1), 
                    new Vector2Int(3, 2), new Vector2Int(4, 2), new Vector2Int(5, 2), 
                    new Vector2Int(4, 3), new Vector2Int(5, 3), new Vector2Int(6, 3), 
                    new Vector2Int(5, 4), new Vector2Int(6, 4), 
                    new Vector2Int(5, 5), new Vector2Int(6, 5), 
                    new Vector2Int(5, 6), new Vector2Int(6, 6), 
                    new Vector2Int(5, 7), new Vector2Int(6, 7), 
                }
            };

            #endregion SharpCircle
            
            #region SharpCircle

            /*
            /*
            q.x
            .x
            x
             #1#
            yield return new object[]
            {
                new Circle(2, 1),
                new Vector2<int>(0, 0),
                new Vector2<int>(4, 5),
                new Vector2<int>(1, 1),

                // Expected:
                new[]
                {
                    new Vector2<int>(2, 0), 
                    new Vector2<int>(1, 1), 
                    new Vector2<int>(0, 2),
                }
            };
            
            /*
               x
             xx.
             x..
            x..q
             x..
             #1#
            yield return new object[]
            {
                new Circle(3, 2),
                new Vector2<int>(4, 4),
                new Vector2<int>(5, 6),
                new Vector2<int>(1, 1),

                // Expected:
                new[]
                {
                    new Vector2<int>(4, 1), 
                    new Vector2<int>(2, 2), new Vector2<int>(3, 2), 
                    new Vector2<int>(2, 3),
                    new Vector2<int>(1, 4),
                    new Vector2<int>(2, 5),
                }
            };
            
            /*
            xx
            xxxxx
            ..xxxx
            ....xx
            ....xx
            qo...xx
            oo...xx
            oo...xx
             #1#
            yield return new object[]
            {
                new Circle(5, 3),
                new Vector2<int>(0, 5),
                new Vector2<int>(7, 8),
                new Vector2<int>(2, 3),

                // Expected:
                new[]
                {
                    new Vector2<int>(0, 0), new Vector2<int>(1, 0), 
                    new Vector2<int>(0, 1), new Vector2<int>(1, 1),  new Vector2<int>(2, 1),  new Vector2<int>(3, 1),  new Vector2<int>(4, 1), 
                    new Vector2<int>(2, 2),  new Vector2<int>(3, 2),  new Vector2<int>(4, 2), new Vector2<int>(5, 2), 
                    new Vector2<int>(4, 3), new Vector2<int>(5, 3), 
                    new Vector2<int>(4, 4), new Vector2<int>(5, 4), 
                    new Vector2<int>(5, 5), new Vector2<int>(6, 5), 
                    new Vector2<int>(5, 6), new Vector2<int>(6, 6), 
                    new Vector2<int>(5, 7), new Vector2<int>(6, 7), 
                }
            };
            */

            #endregion SharpCircle
        }
        
        [Theory]
        [MemberData(nameof(GetExpectedPositionsByCircleShape))]
        public void ToPositions_ShouldReturnExpectedPositions_WhenShapeIsCircle(Circle circle, 
            Vector2Int centerPoint, Vector2Int mapSize, Vector2Int actorSize, 
            Vector2Int[] expectedPositions)
        {
            circle.ToPositions(centerPoint, mapSize, actorSize).Should().BeEquivalentTo(expectedPositions);
        }
    }
}