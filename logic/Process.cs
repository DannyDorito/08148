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

        //will be usefull in limiting execution, todo
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
        /// Finds the total resource cost of a given process (input resources + output resources)
        /// </summary>
        /// <returns>the total resouce cost of a given process</returns>
        public int ProcessCost()
        {
            int linkInDifference = 0;
            int linksOutDifference = 0;
            foreach (LinkIn link in linksIn)
            {
                linkInDifference = FindDifference(link.source.amount, link.amount);
            }
            foreach (LinkOut link in linksOut)
            {
                linksOutDifference = FindDifference(link.target.amount, link.amount);
            }
            return linkInDifference + linksOutDifference;
        }

        /// <summary>
        /// Finds the difference between two given ints
        /// </summary>
        /// <param name="num1">input one</param>
        /// <param name="num2">input 2</param>
        /// <returns></returns>
        public int FindDifference(int num1, int num2)
        {
            return Math.Abs(num1 - num2);
        }

        public void Execute()
        {
            ProcessCost();
            //removes resources
            foreach (LinkIn link in linksIn)
            {
                link.source.amount -= link.amount;
            }
            //adds resources
            foreach (LinkOut link in linksOut)
            {
                link.target.amount += link.amount;
            }
        }
    }
}
