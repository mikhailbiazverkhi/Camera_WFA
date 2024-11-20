using Emgu.CV;
using Emgu.CV.Structure;

namespace Camera_WFA
{

    public class ColorCorrection
    {
        private VideoCapture _camera;
        private Point _redMarker, _greenMarker, _blueMarker, _whiteMarker;

        public ColorCorrection()
        {
            // Инициализация камеры и маркеров
            _camera = new VideoCapture();

            int x1 = (int)(160 * 0.90625); int y1 = (int)(400 * 0.91667);
            int x2 = (int)(280 * 0.90625); int y2 = (int)(400 * 0.91667);
            int x3 = (int)(390 * 0.90625); int y3 = (int)(400 * 0.91667);
            int x4 = (int)(80 * 0.90625); int y4 = (int)(300 * 0.91667);

            _redMarker = new Point(x1, y1);  // координаты маркеров
            _greenMarker = new Point(x2, y2);
            _blueMarker = new Point(x3, y3);
            _whiteMarker = new Point(x4, y4);
        }

        public Mat CorrectImage(Mat frame)
        {
            // Извлечение цветовых значений маркеров
            var redValue = GetMarkerColor(frame, _redMarker);
            var greenValue = GetMarkerColor(frame, _greenMarker);
            var blueValue = GetMarkerColor(frame, _blueMarker);
            var whiteValue = GetMarkerColor(frame, _whiteMarker);

            // Проверка, чтобы цвета маркеров не были нулевыми
            if (redValue.Red == 0 || greenValue.Green == 0 || blueValue.Blue == 0)
            {
                throw new InvalidOperationException("Невозможно вычислить коррекцию: цвет маркера содержит нулевой канал.");
            }

            // Расчет коррекционной матрицы
            var correctionMatrix = CalculateCorrectionMatrix(redValue, greenValue, blueValue, whiteValue);

            // Применение коррекционной матрицы
            Mat correctedFrame = new Mat();
            ApplyColorCorrection(frame, correctionMatrix);

            return frame; // Возвращаем откорректированный кадр
        }

        private Bgr GetMarkerColor(Mat frame, Point marker)
        {
            // Извлекаем цвет пикселя в точке маркера

            Bgr color = frame.ToImage<Bgr, byte>()[marker.Y, marker.X];
            return color;
        }

        private Matrix<float> CalculateCorrectionMatrix(Bgr red, Bgr green, Bgr blue, Bgr white)
        {
            // Код для вычисления матрицы коррекции цвета
            // Вернуть матрицу, которую можно будет применить к изображению
            // Эталонные значения для маркеров в формате BGR
            Bgr referenceRed = new Bgr(0, 0, 255);
            Bgr referenceGreen = new Bgr(0, 255, 0);
            Bgr referenceBlue = new Bgr(255, 0, 0);
            Bgr referenceWhite = new Bgr(255, 255, 255);


            // Вычисление отклонений для каждого канала
            double redScale = referenceRed.Red / red.Red;
            double greenScale = referenceGreen.Green / green.Green;
            double blueScale = referenceBlue.Blue / blue.Blue;

            // Создание матрицы коррекции
            Matrix<float> correctionMatrix = new Matrix<float>(3, 3);

            // Заполнение матрицы коррекции на основе вычисленных коэффициентов
            correctionMatrix[0, 0] = (float)redScale;    // Коррекция для красного канала
            correctionMatrix[0, 1] = 0.0f;
            correctionMatrix[0, 2] = 0.0f;

            correctionMatrix[1, 0] = 0.0f;
            correctionMatrix[1, 1] = (float)greenScale;  // Коррекция для зеленого канала
            correctionMatrix[1, 2] = 0.0f;

            correctionMatrix[2, 0] = 0.0f;
            correctionMatrix[2, 1] = 0.0f;
            correctionMatrix[2, 2] = (float)blueScale;   // Коррекция для синего канала

            return correctionMatrix;
        }


        private void ApplyColorCorrection(Mat frame, Matrix<float> correctionMatrix)
        {
            // Создаем выходной Mat, который будет содержать скорректированное изображение
            Mat correctedFrame = new Mat();

            // Применяем матрицу коррекции к изображению
            CvInvoke.Transform(frame, correctedFrame, correctionMatrix);

            // Копируем скорректированное изображение обратно в исходный кадр
            correctedFrame.CopyTo(frame);
        }
    }
}
