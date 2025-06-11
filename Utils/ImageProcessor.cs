using APR_TEST.Models;

using DlibDotNet;

using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace APR_TEST.Utils
{
    public static class ImageProcessor
    {
        private static readonly string PredictorPath = "dlib-model/shape_predictor_68_face_landmarks.dat";
        private static string noseImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "nose.png");
        private static FrontalFaceDetector? _faceDetector;
        private static ShapePredictor? _predictor;
        private static Mat originalNoseImg;

        public static void InitFaceModel()
        {
            if (_faceDetector == null)
                _faceDetector = Dlib.GetFrontalFaceDetector();

            if (_predictor == null)
                _predictor = ShapePredictor.Deserialize(PredictorPath);

            if (originalNoseImg == null)
            {
                try
                {
                    originalNoseImg = new Mat(noseImagePath, ImreadModes.Unchanged);
                }
                catch { }
            }
        }

        

        public static BitmapSource DetectAndDecorateFace(Mat mat, FileData CurrentFile)
        {
            try
            {
                var array = new byte[mat.Width * mat.Height * mat.ElemSize()];
                Marshal.Copy(mat.Data, array, 0, array.Length);

                using var dlibImage = Dlib.LoadImageData<RgbPixel>(array, (uint)mat.Height, (uint)mat.Width, (uint)(mat.Width * mat.ElemSize()));
                var faces = _faceDetector.Operator(dlibImage);
                CurrentFile.DetectPerson = $"{faces.Length} faces detected";
                foreach (var face in faces)
                {
                    var shape = _predictor.Detect(dlibImage, face);
                    if (shape != null)
                        DrawRudolphCircleOnMat(mat, shape);
                }

                return ConvertToBitmapSource(mat);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DetectAndDecorateFace] 예외 발생: {ex.Message}");
                return CreateBlankFallbackImage(600, 400, Colors.LightGray);
            }
        }

        public static void DrawRudolphCircleOnMat(Mat mat, FullObjectDetection shape)
        {
            if (originalNoseImg.Empty())
            {
                var nose = shape.GetPart(30);
                Cv2.Circle(mat, new OpenCvSharp.Point(nose.X, nose.Y), 20, new Scalar(0, 0, 255), -1);
            }
            else
            {
                OverlayNoseImage(mat, shape, originalNoseImg);
            }

            
        }

        public static void OverlayNoseImage(Mat mat, FullObjectDetection shape, Mat originalNoseImg)
        {
            var nose = shape.GetPart(30);
            int left = shape.GetPart(2).X;
            int right = shape.GetPart(14).X;
            int faceWidth = Math.Max(1, right - left);
            int targetWidth = faceWidth / 4;
            int targetHeight = targetWidth;

            using var scaledNose = new Mat();
            Cv2.Resize(originalNoseImg, scaledNose, new OpenCvSharp.Size(targetWidth, targetHeight));

            int x = nose.X - scaledNose.Width / 2;
            int y = nose.Y - scaledNose.Height / 2;
            x = Math.Max(0, Math.Min(mat.Width - scaledNose.Width, x));
            y = Math.Max(0, Math.Min(mat.Height - scaledNose.Height, y));

            if (scaledNose.Width <= 0 || scaledNose.Height <= 0) return;

            var roi = new OpenCvSharp.Rect(x, y, scaledNose.Width, scaledNose.Height);
            var matROI = new Mat(mat, roi);
            using var noseCrop = new Mat(scaledNose, new OpenCvSharp.Rect(0, 0, roi.Width, roi.Height));

            OverlayImageWithAlpha(noseCrop, matROI);
        }

        public static BitmapSource ConvertToBitmapSource(Mat mat)
        {
            var bmp = mat.ToBitmapSource();
            bmp.Freeze();
            return bmp;
        }

        public static BitmapSource CreateBlankFallbackImage(int width, int height, Color color)
        {
            var wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            byte[] pixels = new byte[width * height * 4];

            for (int i = 0; i < width * height; i++)
            {
                pixels[i * 4 + 0] = color.B;
                pixels[i * 4 + 1] = color.G;
                pixels[i * 4 + 2] = color.R;
                pixels[i * 4 + 3] = color.A;
            }

            wb.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
            return wb;
        }

        private static void OverlayImageWithAlpha(Mat overlay, Mat background)
        {
            for (int y = 0; y < overlay.Rows; y++)
            {
                for (int x = 0; x < overlay.Cols; x++)
                {
                    Vec4b overlayPixel = overlay.Get<Vec4b>(y, x);
                    if (overlayPixel.Item3 == 0) continue;

                    byte alpha = overlayPixel.Item3;
                    var bgPixel = background.Get<Vec3b>(y, x);

                    Vec3b blended = new Vec3b
                    {
                        Item0 = (byte)((overlayPixel.Item0 * alpha + bgPixel.Item0 * (255 - alpha)) / 255),
                        Item1 = (byte)((overlayPixel.Item1 * alpha + bgPixel.Item1 * (255 - alpha)) / 255),
                        Item2 = (byte)((overlayPixel.Item2 * alpha + bgPixel.Item2 * (255 - alpha)) / 255)
                    };

                    background.Set(y, x, blended);
                }
            }
        }

        public static void Dispose()
        {
            _predictor?.Dispose();
            _predictor = null;

            _faceDetector = null;

            originalNoseImg?.Dispose();
            originalNoseImg = null;
        }

    }
}
