using System;
using Godot;
using Xunit;
using Xunit.Abstractions;

namespace TestProject1
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
            var main = (LocalStartup)ResourceLoader.Load<PackedScene>(LocalStartup.ScenePath).Instance();
            GD.Print(System.Threading.Thread.CurrentThread.IsBackground);
            //System.Environment.CurrentManagedThreadId
            Assert.Equal("LocalStartup", main.Name);
            //Assert.Equal("LocalStartup1", main.Name);
        }
    }
}