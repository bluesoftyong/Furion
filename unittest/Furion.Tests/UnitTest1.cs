using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace Furion.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [ContractTestCase]
        public void TestMethod1()
        {
            "���� Furion ���ʾ��".Test(() =>
            {
                Assert.AreEqual("Fur", "Furion");
            });
        }
    }
}