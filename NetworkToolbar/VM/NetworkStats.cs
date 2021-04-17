using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Input;
using NetworkToolbar.Utility;
using NetworkToolbar.VM.Container;

namespace NetworkToolbar.VM
{
    public class NetworkStats : INotifyPropertyChanged
    {
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

        public int FrameDataQty
        {
            get => m_frameDataQty;
            set
            {
                int size = Math.Max(value, c_maxFrameData);
                if(size != m_frameDataQty)
                {
                    m_frameDataQty = size;
                    NotifyPropertyChanged();
                }
            }
        }

        public RenderingMode Display
        {
            get => m_settings.RenderMode;
            set => m_settings.RenderMode = value;
        }

        public ICommand ChangeRenderMode { get; }

        public IReadOnlyList<NetworkFrame> Frames => m_frames.ToArray();

        
        private const int c_maxFrameData = 120;
        
        private double m_upload;
        private double m_download;
        private int m_frameDataQty = c_maxFrameData;
        private LinkedList<NetworkFrame> m_frames;
        private Timer m_timer;
        private Settings m_settings;

        public NetworkStats()
        {
            PerfCounter sent = new PerfCounter("Network Interface", "Bytes Sent/sec");
            PerfCounter got = new PerfCounter("Network Interface", "Bytes Received/sec");
            m_frames = new LinkedList<NetworkFrame>();

            m_settings = Settings.LoadData();
            m_settings.PropertyChanged += OnSettingsPropertyChanged;

            ChangeRenderMode = new DelegateCommand<RenderingMode>(r => Display = r);

            m_timer = new Timer(1000)
            {
                AutoReset = true,
            };

            m_timer.Elapsed += (sender, args) =>
            {
                Upload = sent.getData();
                Download = got.getData();

                m_frames.AddLast(new NetworkFrame()
                {
                    Upload = Upload,
                    Download = Download
                });

                if(m_frames.Count > FrameDataQty)
                {
                    m_frames.RemoveFirst();
                }

                NotifyPropertyChanged(nameof(Frames));
            };

            m_timer.Start();
        }

        private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Settings.RenderMode))
            {
                NotifyPropertyChanged(nameof(Display));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}