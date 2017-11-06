using System.Collections.Generic;

namespace logic
{
    public class ProcessFlow : IFlowOperation {

    public Stores stores;

    public Processes processes;

    public Links links;

    public List<Process> enabled = new List<Process>();

    public ProcessFlow(Stores stores, Processes processes, Links links) {
      this.stores = stores;
      this.processes = processes;
      this.links = links;
    }

    public void Init() {
      foreach (Link link in links) {
        link.Connect(stores, processes);
      }
    }

    public void SetEnabled() {
      foreach (Process p in processes) {
        if (p.Enabled()) {
          enabled.Add(p);
        }
      }
    }

    public void Execute(int k) {
      SetEnabled();
      for (int i = 0; i < k; i++) {
        if (enabled.Count > 0) {
          Process p = enabled[0];
          p.Execute();
          SetEnabled();
        }
      }
    }
  }
}
