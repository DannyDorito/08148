using System;

namespace logic
{
    public class OpExecute : Operation
    {
        public int count;

        public OpExecute(int count)
        {
            this.count = count;
        }

        public override Object Do(ProcessFlow flow)
        {
            flow.Execute(count);
            return null;
        }

        public override Object Output(Object result)
        {
            return null;//somthing should be here, todo
        }

        public override string ToString()
        {
            return "execute " + count;
        }

    }
}
