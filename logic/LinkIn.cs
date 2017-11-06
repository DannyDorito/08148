using System;

namespace logic {
  public class LinkIn : Link {

    internal String sourceId;

    public Store source;

    internal String targetId;

    public Process target;

    public LinkIn(String id, String sourceId, String targetId, int amount)
      : base(id, amount) {
      this.sourceId = sourceId;
      this.targetId = targetId;
    }

    public override void Connect(Stores stores, Processes processes) {
      Store s = stores.Find(sourceId);
      source = s;
      Process p = processes.Find(targetId);
      target = p;
      p.linksIn.Add(this);
    }

    public override String ToString() {
      return id + " " + sourceId + " " + targetId + " " + amount;
    }

  }
}
