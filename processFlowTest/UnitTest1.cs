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
        /// <summary>
        /// Test checks if the program's output is correct
        /// </summary>
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

        /// <summary>
        /// Test checks if the program can handle errors in the Flow portion of the input xml doc
        /// </summary>
        [TestMethod]
        public void IncorrectFlowInputTest()
        {
            Program.Main(new string[] { "flow2.xml" });  // this file resides in processFlow\processFlowTest\bin\Debug
            //todo
        }

        /// <summary>
        /// Test checks if the program can handle errors in the Load portion of the input xml doc
        /// </summary>
        [TestMethod]
        public void IncorectLoadInputTest()
        {
            //make file
        }

        /// <summary>
        /// Test checks if the program can handle errors in the Input portion of the input xml doc
        /// </summary>
        [TestMethod]
        public void IncorrectExecuteInputTest()
        {
            //make file
        }

        /// <summary>
        /// Test checks if the program can handle errors in the Query portion of the input xml doc
        /// </summary>
        [TestMethod]
        public void IncorrectQueryInputTest()
        {
            //make file
        }

        /// <summary>
        /// Test checks if the program can handle errors in the program start arguments
        /// </summary>
        [TestMethod]
        public void IncorrectStartArgumentsTest()
        {
            Program.Main(new string[] { "abc.xml" });
        }
    }
}
