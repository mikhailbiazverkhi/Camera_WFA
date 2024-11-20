using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Windows.Forms;


namespace Camera_WFA
{
    public partial class pictureBox : Form
    {
        private VideoCapture _camera;
        private double brightnessFactor = 1.0; // Коэффициент яркости
        private Mat _frame;
        private ColorCorrection _colorCorrection; // Поле для коррекции цвета

        public pictureBox()
        {
            InitializeComponent();

            _camera = new VideoCapture(0); // Открываем тестовую USB камеру

            if (!_camera.IsOpened)
            {
                MessageBox.Show("Не удалось открыть камеру.");
                return;
            }

            // Устанавливаем разрешение
            _camera.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 640);
            _camera.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 480);

            _frame = new Mat();
            _camera.ImageGrabbed += ProcessFrame; // Подписываемся на событие, когда кадр захвачен
            _camera.Start();
            _colorCorrection = new ColorCorrection(); // Инициализация коррекции цвета
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if (_camera.IsOpened)
            {
                _camera.Retrieve(_frame);
                ApplyBrightness(_frame);
                
                _frame = _colorCorrection.CorrectImage(_frame);  // Применение коррекции цвета

                DisplayFrame(_frame);
            }
        }

        private void ApplyBrightness(Mat frame)
        {
            // Создаем временную матрицу для хранения результата
            using (Mat temp = new Mat())
            {
                // Умножаем пиксели кадра на коэффициент яркости
                CvInvoke.Multiply(frame, new ScalarArray(new MCvScalar(brightnessFactor, brightnessFactor, brightnessFactor)), temp);

                // Копируем результат обратно в исходный frame
                temp.CopyTo(frame);
            }
        }

        private void DisplayFrame(Mat frame)
        {
            // Отображаем кадр в PictureBox
            mainPictureBox.Image = frame.ToImage<Bgr, byte>().ToBitmap();
        }

        // Событие для изменения яркости при нажатии клавиш
        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        //{
        //    if (keyData == Keys.Add) // '+' увеличивает яркость
        //    {
        //        brightnessFactor += 0.1;
        //        Console.WriteLine($"Увеличение яркости: {brightnessFactor}");
        //    }
        //    else if (keyData == Keys.Subtract) // '-' уменьшает яркость
        //    {
        //        brightnessFactor = Math.Max(0, brightnessFactor - 0.1); // Ограничение на минимальное значение 0
        //        Console.WriteLine($"Уменьшение яркости: {brightnessFactor}");
        //    }
        //    return base.ProcessCmdKey(ref msg, keyData);
        //}

        // Форма закрывается
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _camera.Stop();
            _camera.Dispose();
            CvInvoke.DestroyAllWindows(); // Закрыть все окна OpenCV
        }
    }
}