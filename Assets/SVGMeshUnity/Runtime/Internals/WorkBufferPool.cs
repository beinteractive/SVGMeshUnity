using System;
using System.Collections.Generic;

namespace SVGMeshUnity.Internals
{
    public class WorkBufferPool
    {
        Dictionary<Type, List<object>> Pool = new Dictionary<Type, List<object>>();

        public WorkBuffer<T> Get<T>()
        {
            List<object> list;
            WorkBuffer<T> buf = null;

            if (Pool.TryGetValue(typeof(T), out list))
            {
                if (list.Count > 0)
                {
                    var n = list.Count - 1;
                    buf = (WorkBuffer<T>)list[n];
                    list.RemoveAt(n);
                }
            }
            
            return buf ?? new WorkBuffer<T>(32);
        }

        public void Get<T>(ref WorkBuffer<T> buf)
        {
            buf = Get<T>();
        }

        public void Release<T>(ref WorkBuffer<T> buf)
        {
            List<object> list;
            
            buf.Clear();

            if (Pool.TryGetValue(typeof(T), out list))
            {
                list.Add(buf);
            }
            else
            {
                Pool[typeof(T)] = new List<object> { buf };
            }

            buf = null;
        }
    }
}