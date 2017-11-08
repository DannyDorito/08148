using System;
using System.Collections.Generic;
using System.Collections;

namespace logic
{
    public class Processes : IEnumerable
    {
        public List<Process> processes;

        public Processes(List<Process> processes)
        {
            this.processes = processes;
        }

        public IEnumerator GetEnumerator()
        {
            return processes.GetEnumerator();
        }

        public Process Find(String id)
        {
            foreach (Process p in processes)
            {
                if (p.id == id)
                {
                    return p;
                }
            }
            return null;
        }
    }
}
