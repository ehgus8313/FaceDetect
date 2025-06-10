using CommunityToolkit.Mvvm.ComponentModel;

using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace APR_TEST.Models
{
    public class FileData : INotifyPropertyChanged
    {
        public string FilePath { get; set; }
        public string _detectPerson;
        public string DetectPerson
        {
            get => _detectPerson;
            set
            {
                if (_detectPerson != value)
                {
                    _detectPerson = value;
                    OnPropertyChanged(nameof(DetectPerson));
                }
            }
        }
        private BitmapSource _displayImage;
        public BitmapSource DisplayImage
        {
            get => _displayImage;
            set
            {
                if (_displayImage != value)
                {
                    if (_displayImage is IDisposable disposable)
                        disposable.Dispose(); // 대부분 BitmapSource는 IDisposable이 아님. 단, 커스텀 객체인 경우만 적용

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
