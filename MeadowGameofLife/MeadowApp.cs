﻿using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace MeadowGameofLife
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        St7789 display;
        const int displayWidth = 240;
        const int displayHeight = 240;
        
        RgbPwmLed onboardLed;

        Random rand;

        public MeadowApp()
        {
            Stopwatch sw = new Stopwatch();
            Initialize();

            int g = 0;
            while (!dead())
            {
                sw.Restart();
                DrawLife();
                Console.WriteLine($"Draw {g}: {sw.ElapsedMilliseconds}ms");

                sw.Restart();
                g += 1;
                Life = Generation(displayWidth / 8, displayHeight / 8);
                Console.WriteLine($"Compute {g}: {sw.ElapsedMilliseconds}ms");
            }
            Console.WriteLine($"Dead!");
            onboardLed.Stop();
        }

        void Initialize()
        {
            Console.WriteLine("Initializing...");

            var config = new SpiClockConfiguration(6000, SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO, config);

            display = new St7789(
                device: Device,
                spiBus: spiBus,
                chipSelectPin: null,
                dcPin: Device.Pins.D01,
                resetPin: Device.Pins.D00,
                width: (uint)displayWidth, height: (uint)displayHeight);

            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                3.3f, 3.3f, 3.3f,
                Meadow.Peripherals.Leds.IRgbLed.CommonType.CommonAnode);

            rand = new Random();

            Changes = new bool[displayWidth / 8, displayHeight / 8];
            Life = Seed(displayWidth / 8, displayHeight / 8);

            // a pixel is an 8x8 rectangle one bit per pixel
            pixel = new byte[8];
            for (int r = 0; r < pixel.Length; r++)
                pixel[r] = 170; //10101010

            display.Clear();
        }

        /// <summary>
        ///  LIFE
        /// </summary>

        byte[] pixel;

        int[,] Life;
        bool[,] Changes;

        /// <summary>
        /// uses display.Drawbitmap - bypassing the graphics library
        /// </summary>
        void DrawLife()
        {
            var c = RandColor();
            onboardLed.SetColor(c);

            for (int x = 0; x < displayWidth / 8; x++)
            {
                for (int y = 0; y < displayHeight / 8; y++)
                {
                    if (Life[x, y] == 1)
                        display.DrawBitmap(x * 8, y * 8, 1, 8, pixel, c);
                    else if (Changes[x, y])
                        display.DrawBitmap(x * 8, y * 8, 1, 8, pixel, Color.Black);
                }
            }
            display.Show();
        }

        /// <summary>
        /// Randomize initial layout
        /// </summary>
        int[,] Seed(int x, int y)
        {
            var r = new int[x, y];
            for (var i = 0; i < x; i++)
                for (var j = 0; j < y; j++)
                {
                    r[i, j] = rand.Next(0, 2);
                    Changes[i, j] = true;
                }

            return r;
        }

        /// <summary>
        /// Compute the next generation - uses rules of "Life"
        /// </summary>
        int[,] Generation(int x, int y)
        {
            var r = new int[x, y];
            for (var i = 0; i < x; i++)
                for (var j = 0; j < y; j++)
                {
                    var n = GetNeighbours(i, j);
                    var me = Life[i, j];

                    // alive needs 2 or 3 to stay alive
                    if ((me == 1) && (n == 2 || n == 3))
                        r[i, j] = 1;

                    // dead comes to life with 3
                    else if ((me == 0) && (n == 3))
                        r[i, j] = 1;

                    else
                        r[i, j] = 0;

                    Changes[i, j] = !(me == r[i, j]);
                }
            return r;
        }

        int GetNeighbours(int x, int y)
        {
            return GetValue(x - 1, y) + GetValue(x + 1, y) +
                   GetValue(x, y - 1) + GetValue(x, y + 1) +
                   GetValue(x - 1, y - 1) + GetValue(x + 1, y - 1) +
                   GetValue(x - 1, y + 1) + GetValue(x + 1, y + 1);
        }

        int GetValue(int x, int y)
        {
            // do not wrap edges
            //if (x < 0)
            //    return 0;
            //if (y < 0)
            //    return 0;
            //if (y > isplayHeight / 8 - 1)
            //    return 0;
            //if (x > displayWidth / 8 -1)
            //    return 0;

            //edges wrap
            if (x < 0)
                x = displayWidth / 8 - 1;
            else if (x > displayWidth / 8 - 1)
                x = 0;
            if (y < 0)
                y = displayHeight / 8 - 1;
            else if (y > displayHeight / 8 - 1)
                y = 0;

            return Life[x, y];
        }

        /// <summary>
        /// True if all cells are empty
        /// </summary>
        /// <returns></returns>
        bool dead()
        {
            for (var i = 0; i < displayWidth / 8; i++)
                for (var j = 0; j < displayHeight / 8; j++)
                    if (Life[i, j] == 1)
                        return false;
            return true;
        }

        Color RandColor()
        {
            return Color.FromRgb(rand.Next(255), rand.Next(255), rand.Next(255));
        }
    }
}

