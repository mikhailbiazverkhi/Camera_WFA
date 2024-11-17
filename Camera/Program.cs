//// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");


/*

using Emgu.CV;
using Emgu.CV.Structure;
using System;

public class CameraTest
{
    private VideoCapture _camera;

    public CameraTest()
    {
        _camera = new VideoCapture(0); // Открываем встроенную камеру
        if (!_camera.IsOpened)
        {
            Console.WriteLine("Не удалось открыть камеру.");
            return;
        }

        //_camera.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, 640);
        //_camera.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, 480);

        _camera.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 640);
        _camera.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 480);
    }

    public void Start()
    {
        _camera.ImageGrabbed += ProcessFrame;
        _camera.Start();
    }

    private void ProcessFrame(object sender, EventArgs e)
    {
        Mat frame = new Mat();
        _camera.Retrieve(frame);
        ApplyColorCorrection(frame);
        CvInvoke.Imshow("Processed Frame", frame);
        CvInvoke.WaitKey(1);
    }

    private void ApplyColorCorrection(Mat frame)
    {
        //frame *= 1.2; // Увеличиваем яркость на 20% для примера
    }

    public void Stop()
    {
        _camera.Stop();
        _camera.Dispose();
    }
}

class Program
{
    static void Main()
    {
        CameraTest cameraTest = new CameraTest();
        cameraTest.Start();
        Console.WriteLine("ddd Нажмите любую клавишу для выхода...");
        Console.ReadKey();
        cameraTest.Stop();
    }
}
*/


using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System;
//using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

public class CameraTest
{
    private VideoCapture _camera;
    private double brightnessFactor = 1.0; // Коэффициент яркости, начальное значение
    private Mat _frame;

    public CameraTest()
    {
        _camera = new VideoCapture(0); // Открываем встроенную камеру
        if (!_camera.IsOpened)
        {
            Console.WriteLine("Не удалось открыть камеру.");
            return;
        }

        // Устанавливаем разрешение
        //_camera.SetCaptureProperty(CapProp.FrameWidth, 640);
        //_camera.SetCaptureProperty(CapProp.FrameHeight, 480);

        _camera.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 640);
        _camera.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 480);

        _frame = new Mat();

        // Подписываемся на событие Idle для обновления кадра
        Application.Idle += ProcessFrame;

    }

    public void Start()
    {
        _camera.Start();
        Console.WriteLine("Нажмите '+' или '-' для изменения яркости, 'ESC' для выхода.");

        while (true)
        {
            int key = CvInvoke.WaitKey(1);

            if (key == 43) // '+' увеличивает яркость
            {
                brightnessFactor += 0.1;
                Console.WriteLine($"Увеличение яркости: {brightnessFactor}");
            }
            else if (key == 45) // '-' уменьшает яркость
            {
                brightnessFactor = Math.Max(0, brightnessFactor - 0.1); // Ограничение на минимальное значение 0
                Console.WriteLine($"Уменьшение яркости: {brightnessFactor}");
            }
            else if (key == 27) // 'ESC' завершает программу
            {
                Stop();
                break;
            }
        }
    }

    private void ProcessFrame(object sender, EventArgs e)
    {
        if (_camera.IsOpened)
        {
            // Захват кадра
            _camera.Retrieve(_frame);
            ApplyBrightness(_frame);
            CvInvoke.Imshow("Processed Frame", _frame);
        }
    }

    private void ApplyBrightness(Mat frame)
    {
        frame *= brightnessFactor; // Применяем текущий коэффициент яркости
    }

    public void Stop()
    {
        _camera.Stop();
        _camera.Dispose();
        CvInvoke.DestroyAllWindows(); // Закрыть все окна
        Application.Idle -= ProcessFrame; // Отписка от события
    }
}

class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        CameraTest cameraTest = new CameraTest();
        cameraTest.Start();
    }
}