﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using processFlow;
using xmlIO;

namespace processFlowTest
{
    /// <summary>
    /// The test suite for the processFlow solution
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Test checks if the program's output is correct
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "name of exception")] //set name of exception
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
        /// Test checks if the program can handle errors in the Flow portion of the input XML doc
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "name of exception")] //set name of exception
        public void IncorrectFlowInputTest()
        {
            Program.Main(new string[] { "incorrectflow.xml" });  // this file resides in processFlow\processFlowTest\bin\Debug
            //todo
        }

        /// <summary>
        /// Test checks if the program can handle errors in the Load portion of the input XML doc
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "name of exception")] //set name of exception
        public void IncorectLoadInputTest()
        {
            Program.Main(new string[] { "incorrectload.xml" });  // this file resides in processFlow\processFlowTest\bin\Debug
            //todo
        }

        /// <summary>
        /// Test checks if the program can handle errors in the Input portion of the input XML doc
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "name of exception")] //set name of exception
        public void IncorrectExecuteInputTest()
        {
            Program.Main(new string[] { "incorrectexecute.xml" });  // this file resides in processFlow\processFlowTest\bin\Debug
            //todo
        }

        /// <summary>
        /// Test checks if the program can handle errors in the Query portion of the input XML doc
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "name of exception")] //set name of exception
        public void IncorrectQueryInputTest()
        {
            Program.Main(new string[] { "incorrectquery.xml" });  // this file resides in processFlow\processFlowTest\bin\Debug
            //todo
        }

        /// <summary>
        /// Test checks if the program can handle errors in the program start arguments
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException),"name of exception")] //set name of exception
        public void IncorrectStartArgumentsTest()
        {
            Program.Main(new string[] { "abc.xml" }); //the file does not exist
            //todo
        }
    }
}
