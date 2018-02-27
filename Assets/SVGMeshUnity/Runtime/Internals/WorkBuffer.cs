using System.Collections.Generic;
using System.Linq;

namespace SVGMeshUnity.Internals
{
    public class WorkBuffer<T>
    {
        public WorkBuffer(int capacity)
        {
            Data = new List<T>();
            Data.AddRange(Enumerable.Repeat(default(T), capacity));
        }
        
        public List<T> Data { get; private set; }
        public int UsedSize { get; private set; }

        public void Push(ref T val)
        {
            if (Data.Count == UsedSize)
            {
                Data.Add(val);
            }
            else
            {
                Data[UsedSize] = val;
            }

            ++UsedSize;
        }

        public void Clear()
        {
            UsedSize = 0;
        }
    }
}