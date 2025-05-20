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
            var joystickGuid = directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly)[0].InstanceGuid;
            var joystick = new Joystick(directInput, joystickGuid);
            joystick.Acquire();
            Console.WriteLine("F310 bağlı.");

            string solDurum = "";
            string sagDurum = "";
            int merkez = 32767;
            int esik = 10000;

            // --- Toggle durumları ---
            bool lazerDurum = false;
            bool kornaDurum = false;
            bool labirentDurum = false;

            bool oncekiButton0 = false;
            bool oncekiButton1 = false;
            bool oncekiButton2 = false;
            bool oncekiButton3 = false;

            try
            {
                while (true)
                {
                    joystick.Poll();
                    var state = joystick.GetCurrentState();

                    int y = state.Y;
                    int ry = state.RotationY;

                    // SOL JOYSTICK
                    string yeniSol;
                    if (y < merkez - esik) yeniSol = "7";         // ileri
                    else if (y > merkez + esik) yeniSol = "1";    // geri
                    else yeniSol = "S1";                          // dur

                    if (yeniSol != solDurum)
                    {
                        SendData(serialPort, yeniSol, isConnected);
                        solDurum = yeniSol;
                    }

                    // SAĞ JOYSTICK
                    string yeniSag;
                    if (ry < merkez - esik) yeniSag = "9";        // ileri
                    else if (ry > merkez + esik) yeniSag = "3";   // geri
                    else yeniSag = "S2";                          // dur

                    if (yeniSag != sagDurum)
                    {
                        SendData(serialPort, yeniSag, isConnected);
                        sagDurum = yeniSag;
                    }

                    // --- BUTONLAR (Toggle) ---
                    bool simdikiButton0 = state.Buttons[0];
                    bool simdikiButton1 = state.Buttons[1];
                    bool simdikiButton2 = state.Buttons[2];
                    bool simdikiButton3 = state.Buttons[3];

                    // Button0 - Korna (I)
                    if (simdikiButton0 && !oncekiButton0) // sadece basış anında
                    {
                        kornaDurum = !kornaDurum;
                        if (kornaDurum)
                            SendData(serialPort, "I", isConnected);
                    }

                    // Button1 - Lazer (J)
                    if (simdikiButton1 && !oncekiButton1)
                    {
                        lazerDurum = !lazerDurum;
                        SendData(serialPort, "J", isConnected); // Arduino toggle yapacak!
                    }

                    // Button2 - Ateş Et (K) (Direkt bastıkça gönderecek)
                    if (simdikiButton2 && !oncekiButton2)
                    {
                        SendData(serialPort, "K", isConnected);
                    }

                    // Button3 - Labirent (A)
                    if (simdikiButton3 && !oncekiButton3)
                    {
                        labirentDurum = !labirentDurum;
                        if (labirentDurum)
                            SendData(serialPort, "A", isConnected);
                        else
                            SendData(serialPort, "P", isConnected); // P: labirent iptal
                    }

                    // DPAD
                    int pov = state.PointOfViewControllers[0];
                    if (pov == 0) SendData(serialPort, "U", isConnected);
                    if (pov == 9000) SendData(serialPort, "R", isConnected);
                    if (pov == 18000) SendData(serialPort, "D", isConnected);
                    if (pov == 27000) SendData(serialPort, "L", isConnected);

                    // Son durumları güncelle
                    oncekiButton0 = simdikiButton0;
                    oncekiButton1 = simdikiButton1;
                    oncekiButton2 = simdikiButton2;
                    oncekiButton3 = simdikiButton3;

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
