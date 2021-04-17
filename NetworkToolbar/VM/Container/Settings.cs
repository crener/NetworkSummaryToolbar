using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace NetworkToolbar.VM.Container
{
    [Serializable]
    public class Settings : INotifyPropertyChanged
    {
        public RenderingMode RenderMode
        {
            get => m_renderMode;
            set
            {
                m_renderMode = value;
                NotifyPropertyChanged();
            }
        }

        public static Settings LoadData()
        {
            string path = FindPath();
            Settings settings;
            if(!File.Exists(path))
            {
                settings = new Settings();
                settings.SaveData();
            }
            else
            {
                using(StreamReader writer = new StreamReader(FindPath()))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                    settings = xmlSerializer.Deserialize(writer) as Settings;
                }
            }
            
            settings.PropertyChanged += settings.OnSelfChanged;
            return settings;
        }

        private void OnSelfChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveData();
        }

        public void SaveData()
        {
            string path = FindPath();
            
            string dir = Path.GetDirectoryName(path);
            if(!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            
            using(TextWriter writer = new StreamWriter(path, false))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                xmlSerializer.Serialize(writer, this);
            }
        }

        private static string FindPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Network Summary Taskbar", "Settings.txt");
        }

        private RenderingMode m_renderMode = RenderingMode.Average;
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}