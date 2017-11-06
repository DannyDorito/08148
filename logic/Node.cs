using System;

namespace logic {

  public abstract class Node {

    public readonly String id;

    protected Node(String id) {
      this.id = id;
    }

    public override String ToString() {
      return id;
    }

  }
}
