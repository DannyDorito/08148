using System;
using System.Collections.Generic;

namespace logic
{
    public class Process : Node
    {

        public List<LinkIn> linksIn;

        public List<LinkOut> linksOut;

        public List<String> typsIn;

        public List<String> typsOut;

        public Process(String id) : base(id)
        {
            linksIn = new List<LinkIn>();
            linksOut = new List<LinkOut>();
        }

        public Process(String id, List<String> typsIn, List<String> typsOut) : base(id)
        {
            linksIn = new List<LinkIn>();
            linksOut = new List<LinkOut>();
            this.typsIn = typsIn;
            this.typsOut = typsOut;
        }

        public bool Enabled()
        {
            foreach (LinkIn link in linksIn)
            {
                if (link.source.amount < link.amount)
                {
                    return false;
                }
                if (link.source.amount <= 0)
                {
                    return false;
                }
            }
            foreach (LinkOut link in linksOut)
            {
                if (link.target.Limited() && link.target.capacity < link.amount)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Finds the total process cost of a given process ID
        /// </summary>
        /// <param name="processID">The input id identifier of a process</param>
        /// <returns>int total cost of a given process</returns>    
        public int TotalProcessCost(string processID)
        {
            int processCost = LinkInCost(processID) + LinkOutCost(processID);

            return processCost;
        }

        /// <summary>
        /// Finds the link in cost
        /// </summary>
        /// <param name="processID">string ID of the given process</param>
        /// <returns>the total link in cost of a given process</returns>
        public int LinkInCost(string processID)
        {
            int linkInDifference = 0;
            foreach (LinkIn link in linksIn)
            {
                if (link.id == processID)
                {
                    linkInDifference = FindDifference(link.source.amount, link.amount);
                }
            }
            return linkInDifference;
        }

        /// <summary>
        /// Finds the link out cost
        /// </summary>
        /// <param name="processID">string ID of the given process</param>
        /// <returns>the total link out cost of a given process</returns>
        public int LinkOutCost(string processID)
        {
            int linksOutDifference = 0;
            foreach (LinkOut link in linksOut)
            {
                if (link.id == processID)
                {
                    linksOutDifference = FindDifference(link.target.amount, link.amount);
                }
            }
            return linksOutDifference;
        }

        /// <summary>
        /// List of processes that have been sorted from lowest cost to highest cost
        /// </summary>
        public List<Process> SortedProcessList = new List<Process>();

        /// <summary>
        /// List of processes to be sorted from lowest cost to highest cost
        /// </summary>
        /// <param name="UnsortedProcessList">list of processes</param>
        public List<Process> SortProcesses(List<Process> UnsortedProcessList)
        {
            foreach (Process p in UnsortedProcessList)
            {
                TotalProcessCost(p.id);
                SortedProcessList.Add(p);
            }
            SortedProcessList.Sort(); // does not sort correctly, todo

            return SortedProcessList;
        }

        /// <summary>
        /// Finds the difference between two given integers
        /// </summary>
        /// <param name="num1">input 1</param>
        /// <param name="num2">input 2</param>
        /// <returns>difference between two given integers</returns>
        public int FindDifference(int num1, int num2)
        {
            return Math.Abs(num1 - num2);
        }

        public void Execute()
        {
            //removes resources from link, is capacity check necessary? todo
            foreach (LinkIn link in linksIn)
            {
                //Prevents negative amounts from being executed
                if ((link.source.amount - link.amount) >= 0)
                {
                    link.source.amount -= link.amount;
                }
            }
            //adds resources from link
            foreach (LinkOut link in linksOut)
            {
                //Prevents execution if it goes over capacity
                if (link.target.capacity >= 1)
                {
                    //Prevents execution if the amount is too large
                    if ((link.target.amount + link.amount) > link.target.capacity)
                    {
                        link.target.amount += link.amount;
                    }
                }
                if (link.target.capacity == -1)
                {
                    //Prevents execution if the amount is too large
                    if ((link.target.amount + link.amount) >= 0)
                    {
                        link.target.amount += link.amount;
                    }
                }
            }
        }
    }
}
