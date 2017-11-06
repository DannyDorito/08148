using System;

namespace logic {

  public class Store : Node {

    public readonly String typ;

    public int amount;

    public readonly int capacity = -1;

    public Store(String id, String typ, int amount)
      : base(id) {
      this.typ = typ;
      this.amount = amount;
      this.capacity = -1;
    }

    public Store(String id, String typ, int amount, int capacity)
      : base(id) {
      this.typ = typ;
      this.amount = amount;
      this.capacity = capacity;
    }

    public bool Limited() {
      return capacity != -1;
    }

    public override String ToString() {
      return id + " " + amount;
    }
  }

}
