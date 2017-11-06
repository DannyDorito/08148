using System;

namespace logic {

  public abstract class Operation : IFlowOperation {

    public abstract Object Do(ProcessFlow flow);

    public abstract Object Output(Object result);

  }
}
