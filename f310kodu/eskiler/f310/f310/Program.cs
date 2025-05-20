using System;
using System.IO.Ports;
using SharpDX.DirectInput;

namespace f310
{
    class Program
    {
        static void Main(string[] args)
        {
            // Seri port nesnesi oluşturma
            SerialPort serialPort = new SerialPort("COM3", 9600); // HC-06 için COM port ve baud hızı
            bool isConnected = false;

            try
            {
                // Seri portu açmaya çalış
                serialPort.Open();
                isConnected = true;
                Console.WriteLine("İletişim var: HC-06 bağlandı.");
            }
            catch (Exception ex)
            {
                isConnected = false;
                Console.WriteLine($"İletişim yok: Seri port açılamadı. Hata: {ex.Message}");
            }

            // DirectInput nesnesi oluşturma
            DirectInput directInput = new DirectInput();
            var devices = directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly);

            if (devices.Count > 0)
            {
                var deviceInstance = devices[0];
                var joystick = new Joystick(directInput, deviceInstance.InstanceGuid);
                joystick.Acquire();

                Console.WriteLine("Logitech F310 algılandı!");

                try
                {
                    while (true)
                    {
                        joystick.Poll();
                        var state = joystick.GetCurrentState();

                        // D-pad kontrolü: Basılı olduğu sürece veri gönder
                        if (state.PointOfViewControllers.Length > 0)
                        {
                            int currentDpad = state.PointOfViewControllers[0];
                            string command = null;

                            if (currentDpad == 0) command = "U";         // Yukarı
                            else if (currentDpad == 9000) command = "R"; // Sağ
                            else if (currentDpad == 18000) command = "D";// Aşağı
                            else if (currentDpad == 27000) command = "L";// Sol

                            if (command != null)
                            {
                                Console.WriteLine($"D-pad tuşu algılandı: {command}");
                                if (isConnected)
                                {
                                    try
                                    {
                                        serialPort.WriteLine(command); // Basılı olduğu sürece sürekli gönder
                                        Console.WriteLine($"İletildi: {command}");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"İletilemedi: {ex.Message}");
                                    }
                                }
                            }
                        }

                        // Sol joystick: Basılı olduğu sürece veri gönder
                        if (state.Y < 30000) // İleri
                        {
                            Console.WriteLine("Sol Joystick İleri: 7");
                            if (isConnected)
                            {
                                try
                                {
                                    serialPort.WriteLine("7"); // Sürekli '7' gönder
                                    Console.WriteLine("İletildi: 7");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"İletilemedi: {ex.Message}");
                                }
                            }
                        }
                        else if (state.Y > 40000) // Geri
                        {
                            Console.WriteLine("Sol Joystick Geri: 1");
                            if (isConnected)
                            {
                                try
                                {
                                    serialPort.WriteLine("1"); // Sürekli '1' gönder
                                    Console.WriteLine("İletildi: 1");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"İletilemedi: {ex.Message}");
                                }
                            }
                        }

                        // Sağ joystick: Basılı olduğu sürece veri gönder
                        if (state.RotationY < 30000) // İleri
                        {
                            Console.WriteLine("Sağ Joystick İleri: 9");
                            if (isConnected)
                            {
                                try
                                {
                                    serialPort.WriteLine("9"); // Sürekli '9' gönder
                                    Console.WriteLine("İletildi: 9");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"İletilemedi: {ex.Message}");
                                }
                            }
                        }
                        else if (state.RotationY > 40000) // Geri
                        {
                            Console.WriteLine("Sağ Joystick Geri: 3");
                            if (isConnected)
                            {
                                try
                                {
                                    serialPort.WriteLine("3"); // Sürekli '3' gönder
                                    Console.WriteLine("İletildi: 3");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"İletilemedi: {ex.Message}");
                                }
                            }
                        }

                        // Tuşları kontrol et: Basılı olduğu sürece veri gönder
                        for (int i = 0; i < joystick.Capabilities.ButtonCount; i++)
                        {
                            if (state.Buttons[i])
                            {
                                string buttonCommand = null;

                                if (i == 0) buttonCommand = "I"; // Tuş 0 ışık
                                else if (i == 1) buttonCommand = "F"; // Tuş 1 ates et
                                else if (i == 2) buttonCommand = "A"; // Tuş 2 lab çözme başlat kapat
                                else if (i == 3) buttonCommand = "P"; // Tuş 3 lazer

                                if (buttonCommand != null)
                                {
                                    Console.WriteLine($"Tuş {i} basılı: {buttonCommand}");
                                    if (isConnected)
                                    {
                                        try
                                        {
                                            serialPort.WriteLine(buttonCommand); // Basılı olduğu sürece sürekli gönder
                                            Console.WriteLine($"İletildi: {buttonCommand}");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"İletilemedi: {ex.Message}");
                                        }
                                    }
                                }
                            }
                        }

                        System.Threading.Thread.Sleep(100); // Döngü hızını kontrol et
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Joystick hatası: {ex.Message}");
                }
                finally
                {
                    joystick.Unacquire();
                }
            }
            else
            {
                Console.WriteLine("Joystick bulunamadı. Lütfen Logitech F310'u bağlayın.");
            }

            // Seri portu kapatmadan önce bekleme
            Console.WriteLine("Program sonlandırılıyor. Devam etmek için bir tuşa basın...");
            serialPort.Close();
            Console.ReadKey();
        }
    }
}