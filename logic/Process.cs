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

        public List<Process> sortedprocess = new List<Process>();
        public void SortProcesses(List<Process> unsortedlist)
        {
            foreach (Process p in unsortedlist)
            {
                TotalProcessCost(p.id);
                unsortedlist.Sort();
            }

        }

        /// <summary>
        /// Finds the difference between two given integers
        /// </summary>
        /// <param name="num1">input 1</param>
        /// <param name="num2">input 2</param>
        /// <returns></returns>
        public int FindDifference(int num1, int num2)
        {
            return Math.Abs(num1 - num2);
        }

        public void Execute()
        {
            //removes resources
            foreach (LinkIn link in linksIn)
            {
                if ((link.source.amount -= link.amount) >= 0)
                {
                    link.source.amount -= link.amount;
                }
            }
            //adds resources
            foreach (LinkOut link in linksOut)
            {
                if ((link.target.amount += link.amount) <= 0)
                {
                    link.target.amount += link.amount;
                }
            }
        }
    }
}
