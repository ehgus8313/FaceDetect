using APR_TEST.Models;
using APR_TEST.Utils;
using APR_TEST.Views;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DlibDotNet;

using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace APR_TEST.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private VideoCapture? _capture;

        private MediaPlayer? _mediaPlayer;

        [ObservableProperty]
        private FileData? currentFile;

        [ObservableProperty]
        private bool isVideoPlaying;

        [ObservableProperty]
        private bool isWebcamRunning;

        private bool _isRunning;

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        [ObservableProperty]
        private string? selectedTabTag;


        public MainViewModel()
        {
            ImageProcessor.InitFaceModel(); // 전체 앱 최초 진입 시 초기화
        }

        [RelayCommand]
        private async Task LoadImageAsync()
        {
            StopVideo();
            var dlg = new OpenFileDialog
            {
                Filter = "Image or Video|*.jpg;*.png;*.bmp;*.mp4"
            };

            if (dlg.ShowDialog() == true)
            {
                var ext = System.IO.Path.GetExtension(dlg.FileName).ToLower();
                CurrentFile = new FileData(dlg.FileName);
                if (ext == ".mp4")
                {
                    await PlayVideoAsync();
                }
                else
                {
                    ProcessViewImage(); // 결과 표시용
                }
            }
        }

        [RelayCommand]
        public async Task StartWebcamAsync()
        {
            StopVideo();
            if (IsRunning) return;

            var webcamNames = CameraHelper.GetVideoInputDeviceNames();
            if (webcamNames.Count == 0)
            {
                MessageBox.Show("사용 가능한 웹캠이 없습니다.");
                return;
            }

            var dialog = new SelectWebcamDialog(webcamNames);
            if (dialog.ShowDialog() != true || string.IsNullOrEmpty(dialog.SelectedDeviceName))
                return;

            var realIndex = CameraHelper.GetCameraIndexByName(dialog.SelectedDeviceName);
            if (realIndex is null)
            {
                MessageBox.Show("선택한 웹캠을 열 수 없습니다.");
                return;
            }

            _capture = new VideoCapture(realIndex.Value);
            if (!_capture.IsOpened())
            {
                MessageBox.Show("웹캠 열기에 실패했습니다.");
                return;
            }

            IsRunning = true;
            var mat = new Mat();
            int frameCount = 0;
            CurrentFile = new FileData("");
            try
            {
                IsVideoPlaying = false;
                IsWebcamRunning = true;

                while (IsRunning)
                {
                    _capture.Read(mat);
                    if (mat.Empty()) break;

                    frameCount++;
                    if (frameCount % 2 == 0)
                    {
                        var matCopy = mat.Clone();
                        _ = Task.Run(() =>
                        {
                            var bmp = ImageProcessor.DetectAndDecorateFace(matCopy, CurrentFile);
                            bmp.Freeze();
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                if (IsRunning) CurrentFile.DisplayImage = bmp;
                            });
                        });
                    }

                    await Task.Delay(33);
                }
            }
            finally
            {
                mat.Dispose();
                _capture?.Release();
                _capture?.Dispose();
                StopVideo();
            }
        }


        [RelayCommand]
        private void StopVideo()
        {
            IsVideoPlaying = false;
            IsWebcamRunning = false;

            _mediaPlayer?.Stop();
            _mediaPlayer = null;
            IsRunning = false;
            CurrentFile = null;
        }

        public void ProcessViewImage()
        {
            BitmapSource source = null;
            try
            {
                using var mat = new Mat(CurrentFile.FilePath, ImreadModes.Color);

                source = ImageProcessor.DetectAndDecorateFace(mat, CurrentFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[ProcessViewImage] 예외 발생: {ex.Message}");
                source = ImageProcessor.CreateBlankFallbackImage(600, 400, Colors.LightGray);
            }
            finally
            {
                IsVideoPlaying = false;
                IsWebcamRunning = false;

                CurrentFile.DisplayImage = source;
            }
        }

        private async Task PlayVideoAsync()
        {
            _capture = new VideoCapture(CurrentFile.FilePath);
            if (!_capture.IsOpened())
                return;



            IsRunning = true;
            var mat = new Mat();
            var sw = new System.Diagnostics.Stopwatch();
            int targetFps = 30;
            int targetFrameTime = 1000 / targetFps;

            int frameCount = 0;
            int processEveryNthFrame = 2;
            BitmapSource? lastImage = null;

            if (MessageBox.Show("소리를 재생하시겠습니까?", "소리", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // 오디오 재생
                _mediaPlayer = new MediaPlayer();
                _mediaPlayer.Open(new Uri(CurrentFile.FilePath));
                _mediaPlayer.Volume = 1.0;

                _mediaPlayer.MediaOpened += (s, e) => _mediaPlayer.Play();
            }


            try
            {
                IsVideoPlaying = true;
                IsWebcamRunning = false;

                while (IsRunning)
                {
                    sw.Restart();

                    _capture.Read(mat);
                    if (mat.Empty()) break;

                    frameCount++;

                    if (frameCount % processEveryNthFrame == 0)
                    {
                        // 얼굴 인식은 백그라운드에서 처리
                        var matCopy = mat.Clone(); // 별도 쓰레드에서 처리 위해 복제
                        _ = Task.Run(() =>
                        {
                            var bmp = ImageProcessor.DetectAndDecorateFace(matCopy, CurrentFile);
                            bmp.Freeze();
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                if (IsRunning) CurrentFile.DisplayImage = bmp;
                            });
                        });
                    }

                    sw.Stop();
                    int elapsed = (int)sw.ElapsedMilliseconds;
                    int wait = Math.Max(0, targetFrameTime - elapsed);
                    await Task.Delay(wait);
                }
            }
            finally
            {
                mat.Dispose();
                _capture?.Release();
                _capture?.Dispose();
                StopVideo();
            }
        }
    }
}
