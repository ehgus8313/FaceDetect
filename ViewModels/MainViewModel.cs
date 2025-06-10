using APR_TEST.Models;
using APR_TEST.Utils;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DlibDotNet;

using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace APR_TEST.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private FileData? currentFile;

        private VideoCapture? _capture;
        private bool _isRunning;


        [RelayCommand]
        private void LoadImage()
        {
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
                    PlayVideoAsync();
                }
                else
                {
                    CurrentFile.DisplayImage = ImageProcessor.ProcessImage(CurrentFile.FilePath); // 결과 표시용
                }
            }
        }

        [RelayCommand]
        private async Task PlayVideoAsync()
        {
            _capture = new VideoCapture("video.mp4"); // 또는 new VideoCapture(0)

            if (!_capture.IsOpened())
                return;

            var detector = Dlib.GetFrontalFaceDetector();
            var predictor = ShapePredictor.Deserialize("shape_predictor_68_face_landmarks.dat");
            var mat = new Mat();
            _isRunning = true;

            while (_isRunning)
            {
                _capture.Read(mat);
                if (mat.Empty())
                    break;

                var array = new byte[mat.Width * mat.Height * mat.ElemSize()];
                Marshal.Copy(mat.Data, array, 0, array.Length);
                using var dlibImage = Dlib.LoadImageData<RgbPixel>(array, (uint)mat.Height, (uint)mat.Width, (uint)(mat.Width * mat.ElemSize()));

                var faces = detector.Operator(dlibImage);
                if (faces.Length > 0)
                {
                    var shape = predictor.Detect(dlibImage, faces[0]);
                    var nose = shape.GetPart(30); // nose tip
                                                  // Draw Rudolph nose on the original Mat
                    Cv2.Circle(mat, new OpenCvSharp.Point(nose.X, nose.Y), 20, Scalar.Red, -1);
                }

                // Convert Mat to BitmapSource
                var bmp = mat.ToBitmapSource();
                bmp.Freeze(); // WPF 쓰레드 안전 처리
                CurrentFile.DisplayImage = bmp;

                await Task.Delay(33); // 약 30fps
            }

            _capture.Release();
        }

        [RelayCommand]
        private void StopVideo()
        {
            _isRunning = false;
        }
    }
}
