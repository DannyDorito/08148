using System;
using System.Collections.Generic;
using System.Collections;

namespace logic
{
    public class Links : IEnumerable
    {

        public List<Link> links;

        public Links()
        {
            links = new List<Link>();
        }

        public Links(List<Link> links)
        {
            this.links = links;
        }

        public IEnumerator GetEnumerator()
        {
            return links.GetEnumerator();
        }

        public Link Find(String id)
        {
            foreach (Link link in this)
            {
                if (link.id == id)
                {
                    return link;
                }
            }
            return null;
        }
    }
}
