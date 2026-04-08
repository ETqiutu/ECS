using System;

namespace ECS.Library.Core
{
    public class SparseSet<T>
    {
        private T[] dense;
        private int[] sparse;
        private int count;

        public SparseSet(int capacity = 1024)
        {
            dense = new T[capacity];
            sparse = new int[capacity];
            count = 0;
        }

        public void AddComponent(int entity_id, T component)
        {
            sparse[entity_id] = count;
            dense[count] = component;
            count ++;
        }

        public T GetComponent(int entity_id) => dense[sparse[entity_id]];

        public Span<T> AsSpan() => dense.AsSpan(0, count);
    }
}