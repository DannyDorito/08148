using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using processFlow;
using xmlIO;

namespace processFlowTest
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void BasicOutputTest()
        {
            Program.Main(new string[] { "flow1.xml" });  // this file resides in processFlow\processFlowTest\bin\Debug
            int[] required = new int[] { 1, 3 };
            List<int> amounts = ProcessFlowFactory.LoadOutput("out.xml");  // this file resides in processFlow\processFlowTest\bin\Debug
            Assert.AreEqual(required.Length, amounts.Count);
            for (int i = 0; i < amounts.Count; i++)
            {
                Assert.AreEqual(required[i], amounts[i]);
            }
        }
    }
}
