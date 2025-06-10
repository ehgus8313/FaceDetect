using DlibDotNet;

using OpenCvSharp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace APR_TEST.Utils
{
    public static class ImageProcessor
    {
        private static readonly ShapePredictor Predictor = ShapePredictor.Deserialize("dlib-model/shape_predictor_68_face_landmarks.dat");

        public static BitmapSource ProcessImage(string filePath)
        {
            try
            {
                // 1. dlib 이미지 로딩
                using var img = Dlib.LoadImage<RgbPixel>(filePath);
                var faces = Dlib.GetFrontalFaceDetector().Operator(img);

                if (faces.Length == 0)
                {
                    using var fallbackMat = new Mat(filePath, ImreadModes.Color);
                    return OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(fallbackMat);
                }

                // 2. 원본 이미지 불러오기
                using var mat = new Mat(filePath, ImreadModes.Color);

                // 3. 루돌프 코 이미지 로드
                string noseImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "nose.png");
                using var originalNoseImg = new Mat(noseImagePath, ImreadModes.Unchanged);
                if (originalNoseImg.Empty())
                {
                    // fallback: 원으로 코 표시
                    foreach (var face in faces)
                    {
                        var shape = Predictor.Detect(img, face);
                        if (shape == null || shape.Parts == 0) continue;

                        var nose = shape.GetPart(30);
                        Cv2.Circle(mat, new OpenCvSharp.Point(nose.X, nose.Y), 20, new Scalar(0, 0, 255), -1);
                    }
                    return OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(mat);
                }

                // 4. 얼굴별 루돌프 코 합성
                foreach (var face in faces)
                {
                    var shape = Predictor.Detect(img, face);
                    if (shape == null || shape.Parts == 0) continue;

                    var nose = shape.GetPart(30);

                    // 얼굴 폭 기준 코 이미지 크기 계산
                    int left = shape.GetPart(2).X;
                    int right = shape.GetPart(14).X;
                    int faceWidth = Math.Max(1, right - left);
                    int targetWidth = faceWidth / 4;
                    int targetHeight = targetWidth;

                    // 코 이미지 리사이즈
                    using var scaledNose = new Mat();
                    Cv2.Resize(originalNoseImg, scaledNose, new OpenCvSharp.Size(targetWidth, targetHeight));

                    // 합성 위치 계산 (중심 기준)
                    int x = nose.X - scaledNose.Width / 2;
                    int y = nose.Y - scaledNose.Height / 2;

                    // 경계 검사 및 자르기
                    x = Math.Max(0, Math.Min(mat.Width - scaledNose.Width, x));
                    y = Math.Max(0, Math.Min(mat.Height - scaledNose.Height, y));
                    int w = scaledNose.Width;
                    int h = scaledNose.Height;

                    if (w <= 0 || h <= 0) continue;

                    var roi = new OpenCvSharp.Rect(x, y, w, h);
                    var matROI = new Mat(mat, roi);
                    using var noseCrop = new Mat(scaledNose, new OpenCvSharp.Rect(0, 0, w, h));

                    OverlayImageWithAlpha(noseCrop, matROI);
                }

                return OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(mat);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[ProcessImage] 예외 발생: {ex.Message}");
                return CreateBlankFallbackImage(600, 400, Colors.LightGray);
            }
        }


        private static BitmapSource CreateBlankFallbackImage(int width, int height, Color color)
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
                    if (overlayPixel.Item3 == 0) continue; // alpha = 0이면 skip

                    byte alpha = overlayPixel.Item3;
                    var bgPixel = background.Get<Vec3b>(y, x);

                    // 단순 알파 블렌딩 (255 기준)
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

    }
}
