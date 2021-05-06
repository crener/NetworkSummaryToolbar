using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Navigation;

namespace NetworkToolbar.Utility
{
    public class DropOutStack<T> : IEnumerable<T>
    {
        /// <summary>
        /// Size of the stored data
        /// </summary>
        public int Capacity => m_items.Length;
        
        private T[] m_items;
        private int m_top = 0;
        
        public DropOutStack(int capacity)
        { 
            m_items = new T[capacity];
        }

        public T this [int key]
        {
            get => m_items[(key + m_top) % m_items.Length];
        }

        public void Push(T item)
        {
            m_items[m_top] = item;
            m_top = (m_top + 1) % m_items.Length;
        }
        public T Pop()
        {
            m_top = (m_items.Length + m_top - 1) % m_items.Length;
            return m_items[m_top];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < m_items.Length; i++)
            {
                yield return m_items[(i + m_top) % m_items.Length];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}