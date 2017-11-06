using System;
using System.IO;
using System.Collections.Generic;

namespace logic {
  public class Process : Node {

    public List<LinkIn> linksIn;

    public List<LinkOut> linksOut;

    public List<String> typsIn;

    public List<String> typsOut;

    public Process(String id)
      : base(id) {
      this.linksIn = new List<LinkIn>();
      this.linksOut = new List<LinkOut>();
    }

    public Process(String id, List<String> typsIn, List<String> typsOut)
      : base(id) {
      this.linksIn = new List<LinkIn>();
      this.linksOut = new List<LinkOut>();
      this.typsIn = typsIn;
      this.typsOut = typsOut;
    }

    public bool Enabled() {
      foreach (LinkIn link in linksIn) {
        if (link.source.amount < link.amount) {
          return false;
        }
      }
      foreach (LinkOut link in linksOut) {
        if (link.target.Limited()
            && link.target.capacity < link.amount) {
          return false;
        }
      }
      return true;
    }

    public void Execute() {
      foreach (LinkIn link in linksIn) {
        link.source.amount -= link.amount;
      }
      foreach (LinkOut link in linksOut) {
        link.target.amount += link.amount;
      }
    }
  }
}
