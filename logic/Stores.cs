using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using logic;

namespace logic {
  public class Stores {

    public List<Store> stores;

    public Stores(List<Store> stores) {
      this.stores = stores;
    }

    public Store Find(String id) {
      foreach (Store s in stores) {
        if (s.id == id) {
          return s;
        }
      }
      return null;
    }
  }
}
