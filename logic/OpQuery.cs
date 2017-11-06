using System;
using System.Collections.Generic;

namespace logic {
  public class OpQuery : Operation {

    public List<String> Ids;

    public OpQuery(List<String> Ids) {
      this.Ids = Ids;
    }

    public override Object Do(ProcessFlow flow) {
      List<int> result = new List<int>();
      foreach (String id in Ids) {
        Store store = flow.stores.Find(id);
        result.Add(store.amount);
      }
      return result;
    }

    public override Object Output(Object result) {
      return new OpQueryOutput((List<int>)result);
    }

    public override string ToString() {
      String result = "query";
      foreach (String s in Ids) {
        result += " " + s;
      }
      return result;
    }
  }

  public class OpQueryOutput {

    public List<int> amounts;

    public OpQueryOutput() {
    }

    public OpQueryOutput(List<int> amounts) {
      this.amounts = amounts;
    }
  }
}
