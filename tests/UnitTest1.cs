using FluentAssertions;
using Godot;
using LowAgeData.Domain.Common;
using Xunit.Abstractions;

namespace LowAgeTests
{
    public partial class MyTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MyTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            int i = 0;
            _testOutputHelper.WriteLine("Testsss");
            GD.Print("XUnitTest");
            var main = (ClientStartup)ResourceLoader.Load<PackedScene>(ClientStartup.ScenePath).Instantiate();
            GD.Print(System.Threading.Thread.CurrentThread.IsBackground);
            //System.Environment.CurrentManagedThreadId
            Assert.Equal("ClientStartup", main.Name);
            //Assert.Equal("LocalStartup1", main.Name);
        }

        [Fact]
        public void ValueObjectTest()
        {
            var location = Location.Actor;

            switch (location)
            {
                case var _ when location.Equals(Location.Actor):
                    GD.Print(Location.Actor);
                    break;
                default:
                    GD.Print($"Not {Location.Actor}");
                    break;
            }
        }

        [Fact]
        public void CSharpDictionaryTupleTest()
        {
            var dictionary = new Dictionary<(Vector2, bool), int>();
            dictionary[(new Vector2(0, 0), true)] = 0;
            dictionary[(new Vector2(0, 0), false)] = 1;
            dictionary[(new Vector2(1, 1), false)] = 2;
            dictionary[(new Vector2(1, 1), false)] = 3;

            dictionary[(new Vector2(0, 0), true)].Should().Be(0);
            dictionary[(new Vector2(0, 0), false)].Should().Be(1);
            dictionary[(new Vector2(1, 1), false)].Should().Be(3);
        }

        [Fact]
        public void DijkstraLibraryRemovePointAlsoRemovesConnections()
        {
            /*
            Test cannot be instantiated but was tested in a game environment, so keeping this as a reference of 
            the library's behaviour:
             
            var pathfinding = new DijkstraMap();
            pathfinding.AddPoint(0);
            pathfinding.AddPoint(1);
            pathfinding.ConnectPoints(0, 1);

            pathfinding.HasConnection(0, 1).Should().Be(true);

            pathfinding.RemovePoint(1);

            pathfinding.HasConnection(0, 1).Should().Be(false);

            pathfinding.AddPoint(1);

            pathfinding.HasConnection(0, 1).Should().Be(false);
            */
        }
    }
}