using System;
using ClassLibrary1;
using Xunit;

namespace XUnitTestProject1
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
