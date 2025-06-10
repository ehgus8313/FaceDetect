using CommunityToolkit.Mvvm.ComponentModel;

using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace APR_TEST.Models
{
    public class FileData : INotifyPropertyChanged
    {
        public string FilePath { get; set; }
        private BitmapSource _displayImage;
        public BitmapSource DisplayImage
        {
            get => _displayImage;
            set
            {
                if (_displayImage != value)
                {
                    _displayImage = value;
                    OnPropertyChanged(nameof(DisplayImage));
                }
            }
        }

        public FileData(string filePath)
        {
            FilePath = filePath;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
