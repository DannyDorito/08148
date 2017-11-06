using System;

namespace logic {

  public abstract class Link : Node {

    public readonly int amount = -1;

    protected Link(String id, int amount)
      : base(id) {
      this.amount = amount;
    }

    public abstract void Connect(Stores stores, Processes processes);
  }
}
