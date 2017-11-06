using System;
using System.Collections.Generic;

namespace logic {

  public struct StoreIDAmount {

    public String storeId;

    public int amount;

    public StoreIDAmount(String storeId, int amount) {
      this.storeId = storeId;
      this.amount = amount;
    }
  }

  public class OpLoad : Operation {

    public List<StoreIDAmount> amounts;

    public OpLoad(List<StoreIDAmount> amounts) {
      this.amounts = amounts;
    }

    public override Object Do(ProcessFlow flow) {
      foreach (StoreIDAmount sa in this.amounts) {
        String storeId = sa.storeId;
        Store store = flow.stores.Find(storeId);
        int amount = sa.amount;
        int storeAmount = store.amount;
        int newAmount = storeAmount + amount;
        store.amount = newAmount;
      }
      return null;
    }

    public override Object Output(Object result) {
      return result;
    }

    public override string ToString() {
      String result = "load";
      foreach (StoreIDAmount sa in this.amounts) {
        result += " " + sa.storeId + " " + sa.amount;
      }
      return result;
    }
  }
}
