using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Timers;

namespace NetworkToolbar.VM
{
    public class NetworkStats : INotifyPropertyChanged
    {
        public int MaxStats = 120;
        
        public double Upload
        {
            get => m_upload;
            set
            {
                m_upload = value;
                NotifyPropertyChanged();
            }
        }

        public double Download
        {
            get => m_download;
            set
            {
                m_download = value;
                NotifyPropertyChanged();
            }
        }
        public IEnumerable<NetworkFrame> Frames => m_frames;

        private List<NetworkFrame> m_frames;
        private double m_upload;
        private double m_download;
        private Timer m_timer;

        public NetworkStats()
        {
            PerfCounter sent = new PerfCounter("Network Interface", "Bytes Sent/sec");
            PerfCounter got = new PerfCounter("Network Interface", "Bytes Received/sec");
            m_frames = new List<NetworkFrame>(MaxStats);

            m_timer = new Timer(1000)
            {
                AutoReset = true,
            };

            m_timer.Elapsed += (sender, args) =>
            {
                Upload = sent.getData();
                Download = got.getData();
                
                m_frames.Add(new NetworkFrame()
                {
                    Upload = Upload,
                    Download = Download
                });
                
                if(m_frames.Count > MaxStats)
                {
                    m_frames.RemoveAt(0);
                }
                NotifyPropertyChanged(nameof(Frames));
            };
            
            m_timer.Start();
        }
        
        

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}