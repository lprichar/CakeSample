using ClassLibrary1;
using System;
using Xunit;

namespace CakeSample.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var actual = Class1.SayHello("Bob");
            Assert.Equal("Hello Bob", actual);
        }
    }
}
