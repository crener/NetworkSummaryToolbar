using System.Collections;
using System.Collections.Generic;
using System.Windows.Navigation;

namespace NetworkToolbar.Utility
{
    public class DropOutStack<T> : IEnumerable<T>
    {
        private T[] items;
        private int m_top = 0;
        
        public DropOutStack(int capacity)
        { 
            items = new T[capacity];
        }

        public void Push(T item)
        {
            items[m_top] = item;
            m_top = (m_top + 1) % items.Length;
        }
        public T Pop()
        {
            m_top = (items.Length + m_top - 1) % items.Length;
            return items[m_top];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < items.Length; i++)
            {
                yield return items[(i + m_top) % items.Length];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}