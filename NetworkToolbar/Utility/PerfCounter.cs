using System.Diagnostics;
using System.Linq;

namespace NetworkToolbar.Utility
{
    public class PerfCounter
    {
        public string Category { get; }
        public string Name { get; }

        
        private PerformanceCounter[] m_counter;

        public PerfCounter(string category, string name)
        {
            Category = category;
            Name = name;

            bool catExists = PerformanceCounterCategory.Exists(Category);
            bool exists = catExists && PerformanceCounterCategory.CounterExists(name, Category);
            if(!exists)
            {
                return;
            }
            
            PerformanceCounterCategory counterCategory = new PerformanceCounterCategory(Category);

            string[] names = counterCategory.GetInstanceNames();
            m_counter = new PerformanceCounter[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                m_counter[i] = new PerformanceCounter(Category, Name, names[i]);
            }
        }

        public float getData()
        {
            if(m_counter == null) return 0;
            return m_counter.Sum(c => c.NextValue());
        }
    }
}