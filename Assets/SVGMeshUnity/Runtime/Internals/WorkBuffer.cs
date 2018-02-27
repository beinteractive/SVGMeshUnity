using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public Func<T> NewForClass;

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

        public void Push(Action<T> f)
        {
            if (Data.Count == UsedSize)
            {
                var val = NewForClass();
                Data.Add(val);
                f(val);
            }
            else
            {
                var val = Data[UsedSize];
                if (val == null)
                {
                    val = NewForClass();
                    Data[UsedSize] = val;
                }
                f(val);
            }

            ++UsedSize;
        }

        public void Insert(int index, Action<T> f)
        {
            if (index == UsedSize)
            {
                Push(f);
                return;
            }
            
            if (Data.Count == UsedSize)
            {
                var val = NewForClass();
                Data.Insert(index, val);
                f(val);
            }
            else
            {
                var val = Data[UsedSize];
                
                for (var i = UsedSize - 1; i >= index; --i)
                {
                    Data[i + 1] = Data[i];
                }

                if (val == null)
                {
                    val = NewForClass();
                }
                
                Data[index] = val;

                f(val);
            }

            ++UsedSize;
        }

        public void RemoveAt(int index)
        {
            var old = Data[index];
            
            for (var i = index; i < UsedSize - 1; ++i)
            {
                Data[i] = Data[i + 1];
            }

            Data[UsedSize - 1] = old;

            --UsedSize;
        }

        public void Sort(Comparison<T> c)
        {
            if (Data.Count > UsedSize)
            {
                Data.RemoveRange(UsedSize, Data.Count - UsedSize);
            }
            Data.Sort(c);
        }

        public void Clear()
        {
            UsedSize = 0;
        }

        public void Dump()
        {
            Debug.LogFormat("{0}{1}", UsedSize, Data.Select(_ => string.Format("{0:x}",_ != null ? _.GetHashCode() : 0)).Aggregate("", (_, s) => _ + ", " + s));
        }
    }
}