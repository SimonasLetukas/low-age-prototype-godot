using System;
using Godot;
using Xunit;

namespace TestProject1
{
    public class MyTest
    {
        [Fact]
        public void Test1()
        {
            int i = 0;
            Console.WriteLine("Testsss");
            GD.Print("XUnitTest");
            var main = (LocalStartup)ResourceLoader.Load<PackedScene>(LocalStartup.ScenePath).Instance();
            GD.Print(System.Threading.Thread.CurrentThread.IsBackground);
            //System.Environment.CurrentManagedThreadId
            Assert.Equal("LocalStartup", main.Name);
        }
    }
}