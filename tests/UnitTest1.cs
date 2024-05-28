using Godot;
using low_age_data.Domain.Common;
using Xunit;
using Xunit.Abstractions;

namespace low_age_tests
{
    public class MyTest
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
            var main = (ClientStartup)ResourceLoader.Load<PackedScene>(ClientStartup.ScenePath).Instance();
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
    }
}