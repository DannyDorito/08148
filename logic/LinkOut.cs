using System;

namespace logic
{
    public class LinkOut : Link
    {
        internal String sourceId;
        public Process source;
        internal String targetId;
        public Store target;

        public LinkOut(String id, String sourceId, String targetId, int amount) : base(id, amount)
        {
            this.sourceId = sourceId;
            this.targetId = targetId;
        }

        public override void Connect(Stores stores, Processes processes)
        {
            Process p = processes.Find(sourceId);
            source = p;
            Store s = stores.Find(targetId);
            target = s;
            p.linksOut.Add(this);
        }

        public override String ToString()
        {
            return id + " " + sourceId + " " + targetId + " " + amount;
        }

    }
}
