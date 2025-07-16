# RC Tank Embedded System Project 

Bu proje, gömülü sistemler bilgisiyle geliştirilmiş uzaktan kumandalı bir tank sistemidir. Proje, Arduino Mega2560 Pro kullanılarak C++ ile kodlanmış ve joystick kontrollü, engelden kaçabilen, lazer/ateş/sinyal/selektör gibi özellikler barındıran bir kara aracı (UGV) tasarımını içermektedir.

## Donanım Bileşenleri

- **Arduino Mega2560 Pro**
- **HC-06 Bluetooth modülü**
- **SG90 servo motorlar** (kule ve namlu kontrolü)
- **MZ80 IR sensör** (engel algılama)
- **IRFZ44 MOSFET motor sürücü**
- **Motorlar ve palet sistemi**
- **LED'ler** (farlar, sinyaller, selektör)
- **Buzzer**
- **Lazer**
- **Joystick & Logitech F310 gamepad**

## Kontrol

Bluetooth üzerinden joystick (Logitech F310) ile aşağıdaki komutlar gönderilir:

| Komut | Fonksiyon |
|-------|-----------|
| 7 / 1 | Sol palet ileri / geri |
| 9 / 3 | Sağ palet ileri / geri |
| U / D | Namlu yukarı / aşağı |
| L / R | Kule sola / sağa |
| I     | Buzzer |
| K     | Ateş et (simülasyon) |
| J     | Lazer aç/kapat |
| A     | Sol sinyal |
| Z     | Sol farlar yanıp sönme |
| X     | Sağ farlar yanıp sönme |
| C     | Tüm farlar açık / kapalı |
| V     | Selektör (ön farlar yanar/söner) |

## Akıllı Davranışlar

- **Engel Algılama & Kurtulma**: Tank, önündeki engeli algılayıp geri kaçar, ardından manuel kontrole geçer.
- **Yanıp Sönen Sinyaller**: "Z" ve "X" komutlarıyla ilgili farlar sürekli yanıp söner.
- **Selektör Özelliği**: "V" komutuyla kısa süreli ön far yanması.

## 🔧 Kod Yapısı

Kodlar `tank.ino` içinde toplanmıştır. Proje ilerledikçe modüler yapıya ayrılacaktır (motor.h, servo.h, logic.cpp vb.).

## Geliştirme Aşamaları

- [x] Palet motor kontrolü
- [x] Servo ile turret ve barrel yönlendirme
- [x] Lazer, ateş, korna kontrolü
- [x] Engel algılayıp kaçma
- [x] Far, selektör ve sinyal sistemleri
- [ ] EEPROM’a yol kaydetme (gelecek planı)
- [ ] Otonom şekilde labirent çözme

## Görseller

*Görseller ileride eklenecektir.*
