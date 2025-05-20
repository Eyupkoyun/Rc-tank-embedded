using System;
using System.IO.Ports;
using SharpDX.DirectInput;

namespace f310
{
    class Program
    {
        static void Main(string[] args)
        {
            SerialPort serialPort = new SerialPort("COM3", 9600);
            bool isConnected = false;

            try
            {
                serialPort.Open();
                isConnected = true;
                Console.WriteLine("HC-06 bağlı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Port açılamadı: {ex.Message}");
                return;
            }

            DirectInput directInput = new DirectInput();
            var devices = directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly);

            if (devices.Count == 0)
            {
                Console.WriteLine("Joystick bulunamadı. Lütfen Logitech F310'u bağlayın.");
                return;
            }

            var joystickGuid = devices[0].InstanceGuid;
            var joystick = new Joystick(directInput, joystickGuid);
            joystick.Acquire();
            Console.WriteLine("F310 bağlı.");

            string solDurum = "";
            string sagDurum = "";
            int merkez = 32767;
            int esik = 10000;

            bool oncekiButton0 = false;
            bool oncekiButton1 = false;
            bool oncekiButton2 = false;
            bool oncekiButton3 = false;

            bool oncekiLB = false;
            bool oncekiLT = false;
            bool oncekiRB = false;
            bool oncekiRT = false;

            try
            {
                while (true)
                {
                    joystick.Poll();
                    var state = joystick.GetCurrentState();

                    int y = state.Y;
                    int ry = state.RotationY;

                    // Sol joystick kontrolü
                    string yeniSol;
                    if (y < merkez - esik) yeniSol = "7";
                    else if (y > merkez + esik) yeniSol = "1";
                    else yeniSol = "S1";

                    if (yeniSol != solDurum)
                    {
                        SendData(serialPort, yeniSol, isConnected);
                        solDurum = yeniSol;
                    }

                    // Sağ joystick kontrolü
                    string yeniSag;
                    if (ry < merkez - esik) yeniSag = "9";
                    else if (ry > merkez + esik) yeniSag = "3";
                    else yeniSag = "S2";

                    if (yeniSag != sagDurum)
                    {
                        SendData(serialPort, yeniSag, isConnected);
                        sagDurum = yeniSag;
                    }

                    // Tuşlar kontrolü
                    bool simdikiButton0 = state.Buttons[0];
                    bool simdikiButton1 = state.Buttons[1];
                    bool simdikiButton2 = state.Buttons[2];
                    bool simdikiButton3 = state.Buttons[3];

                    if (simdikiButton0 && !oncekiButton0) SendData(serialPort, "I", isConnected);
                    if (simdikiButton1 && !oncekiButton1) SendData(serialPort, "J", isConnected);
                    if (simdikiButton2 && !oncekiButton2) SendData(serialPort, "K", isConnected);
                    if (simdikiButton3 && !oncekiButton3) SendData(serialPort, "L", isConnected);

                    oncekiButton0 = simdikiButton0;
                    oncekiButton1 = simdikiButton1;
                    oncekiButton2 = simdikiButton2;
                    oncekiButton3 = simdikiButton3;

                    // LB, LT, RB, RT Buton Kontrolleri
                    bool simdikiLB = state.Buttons[4];
                    bool simdikiRB = state.Buttons[5];
                    bool simdikiLT = state.Buttons[6];
                    bool simdikiRT = state.Buttons[7];

                    if (simdikiLB && !oncekiLB) SendData(serialPort, "Z", isConnected);
                    if (simdikiLT && !oncekiLT) SendData(serialPort, "C", isConnected);
                    if (simdikiRB && !oncekiRB) SendData(serialPort, "X", isConnected);
                    if (simdikiRT && !oncekiRT) SendData(serialPort, "V", isConnected);

                    oncekiLB = simdikiLB;
                    oncekiLT = simdikiLT;
                    oncekiRB = simdikiRB;
                    oncekiRT = simdikiRT;

                    // D-Pad yönleri
                    int pov = state.PointOfViewControllers[0];
                    if (pov == 0) SendData(serialPort, "U", isConnected);
                    if (pov == 9000) SendData(serialPort, "R", isConnected);
                    if (pov == 18000) SendData(serialPort, "D", isConnected);
                    if (pov == 27000) SendData(serialPort, "L", isConnected);

                    System.Threading.Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Joystick hatası: {ex.Message}");
            }
            finally
            {
                joystick.Unacquire();
                serialPort.Close();
            }
        }

        static void SendData(SerialPort port, string data, bool isConnected)
        {
            if (isConnected && port.IsOpen)
            {
                port.WriteLine(data);
                Console.WriteLine("Gönderildi: " + data);
            }
        }
    }
}