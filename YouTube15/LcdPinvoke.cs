using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace YouTube15
{
    class LcdPinvoke
    {
        public const Int32 LOGI_LCD_TYPE_MONO = 1;
        public const Int32 LOGI_LCD_TYPE_COLOR = 2;

        public const Int32 LOGI_LCD_MONO_BUTTON_0 = 1;
        public const Int32 LOGI_LCD_MONO_BUTTON_1 = 2;
        public const Int32 LOGI_LCD_MONO_BUTTON_2 = 4;
        public const Int32 LOGI_LCD_MONO_BUTTON_3 = 8;
    
        public const Int32 LOGI_LCD_COLOR_BUTTON_LEFT = 256;
        public const Int32 LOGI_LCD_COLOR_BUTTON_RIGHT = 512;
        public const Int32 LOGI_LCD_COLOR_BUTTON_OK = 1024;
        public const Int32 LOGI_LCD_COLOR_BUTTON_CANCEL = 2048;
        public const Int32 LOGI_LCD_COLOR_BUTTON_UP = 4096;
        public const Int32 LOGI_LCD_COLOR_BUTTON_DOWN = 8192;
        public const Int32 LOGI_LCD_COLOR_BUTTON_MENU = 16384;

        public const Int32 LOGI_LCD_MONO_WIDTH = 160;
        public const Int32 LOGI_LCD_MONO_HEIGHT = 43;

        public const Int32 LOGI_LCD_COLOR_WIDTH = 320;
        public const Int32 LOGI_LCD_COLOR_HEIGHT = 240;

        [DllImport("LogitechLcd.dll", CallingConvention=CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool LogiLcdInit([MarshalAs(UnmanagedType.LPWStr)] string friendlyName, Int32 lcdType);
        [DllImport("LogitechLcd.dll", CallingConvention=CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool LogiLcdIsConnected(Int32 lcdType);
        [DllImport("LogitechLcd.dll", CallingConvention=CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool LogiLcdIsButtonPressed(Int32 button);
        [DllImport("LogitechLcd.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void LogiLcdUpdate();
        [DllImport("LogitechLcd.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void LogiLcdShutdown();

        // Monochrome LCD functions
        [DllImport("LogitechLcd.dll", CallingConvention=CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool LogiLcdMonoSetBackground([MarshalAs(UnmanagedType.LPArray)] Byte[] monoBitmap);
        [DllImport("LogitechLcd.dll", CallingConvention=CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool LogiLcdMonoSetText(Int32 lineNumber, [MarshalAs(UnmanagedType.LPWStr)] string text);

        // Color LCD functions
        [DllImport("LogitechLcd.dll", CallingConvention=CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool LogiLcdColorSetBackground([MarshalAs(UnmanagedType.LPArray)] Byte[] colorBitmap);
        [DllImport("LogitechLcd.dll", CallingConvention=CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool LogiLcdColorSetTitle([MarshalAs(UnmanagedType.LPWStr)] string text, Int32 red, Int32 green, Int32 blue);
        [DllImport("LogitechLcd.dll", CallingConvention=CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool LogiLcdColorSetText(Int32 lineNumber, [MarshalAs(UnmanagedType.LPWStr)] string text, Int32 red, Int32 green, Int32 blue);
    }

    public class LogiLcdException : Exception
    {
        public LogiLcdException() : base() {}
        public LogiLcdException(string msg) : base(msg) {}
    }

    public class LogiLcd
    {
        [Flags]
        public enum LcdType
        {
            Mono = LcdPinvoke.LOGI_LCD_TYPE_MONO,
            Color = LcdPinvoke.LOGI_LCD_TYPE_COLOR
        }

        [Flags]
        public enum LcdButton
        {
            Mono0 = LcdPinvoke.LOGI_LCD_MONO_BUTTON_0,
            Mono1 = LcdPinvoke.LOGI_LCD_MONO_BUTTON_1,
            Mono2 = LcdPinvoke.LOGI_LCD_MONO_BUTTON_2,
            Mono3 = LcdPinvoke.LOGI_LCD_MONO_BUTTON_3,
            ColorLeft = LcdPinvoke.LOGI_LCD_COLOR_BUTTON_LEFT,
            ColorRight = LcdPinvoke.LOGI_LCD_COLOR_BUTTON_RIGHT,
            ColorOk = LcdPinvoke.LOGI_LCD_COLOR_BUTTON_OK,
            ColorCancel = LcdPinvoke.LOGI_LCD_COLOR_BUTTON_CANCEL,
            ColorUp = LcdPinvoke.LOGI_LCD_COLOR_BUTTON_UP,
            ColorDown = LcdPinvoke.LOGI_LCD_COLOR_BUTTON_DOWN,
            ColorMenu = LcdPinvoke.LOGI_LCD_COLOR_BUTTON_MENU
        }

        public const int MonoWidth = LcdPinvoke.LOGI_LCD_MONO_WIDTH;
        public const int MonoHeight = LcdPinvoke.LOGI_LCD_MONO_HEIGHT;
        public const int ColorWidth = LcdPinvoke.LOGI_LCD_COLOR_WIDTH;
        public const int ColorHeight = LcdPinvoke.LOGI_LCD_COLOR_HEIGHT;

        private static bool hasInstance = false;
        private bool disposed = false;

        public LogiLcd(string friendlyName, LcdType lcdtype)
        {
            if (hasInstance)
                throw new LogiLcdException("There can only be one instance of this class at a time!");

            if (!LcdPinvoke.LogiLcdInit(friendlyName, (int)lcdtype))
                throw new LogiLcdException("LogiLcdInit failed");

            hasInstance = true;
        }

        public LogiLcd(string friendlyName) : this(friendlyName, LcdType.Mono)
        {
        }

        public void Dispose()
        {
            if (disposed)
                return;

            LcdPinvoke.LogiLcdShutdown();
            hasInstance = false;

            disposed = true;
        }

        ~LogiLcd()
        {
            Dispose();
        }

        public bool IsConnected(LcdType lcdtype)
        {
            return LcdPinvoke.LogiLcdIsConnected((int)lcdtype);
        }

        public bool IsButtonPressed(LcdButton button)
        {
            return LcdPinvoke.LogiLcdIsButtonPressed((int)button);
        }

        public void Update()
        {
            LcdPinvoke.LogiLcdUpdate();
        }

        public bool MonoSetBackground(Byte[] monoBitmap)
        {
            if (monoBitmap.Length != MonoWidth * MonoHeight)
                throw new LogiLcdException("Bitmap size does not match expected size");

            return LcdPinvoke.LogiLcdMonoSetBackground(monoBitmap);
        }

        public bool MonoSetBackground(Bitmap bitmap)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new LogiLcdException("Bitmap PixelFormat has to be 32bppArgb");

            if (bitmap.Width != MonoWidth || bitmap.Height != MonoHeight)
                throw new LogiLcdException("Bitmap size does not match the mono LCD size");

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

            int srcBytes = Math.Abs(data.Stride) * bitmap.Height;
            Byte[] rgbData = new Byte[srcBytes];
            Marshal.Copy(data.Scan0, rgbData, 0, srcBytes);

            Byte[] resultData = new Byte[bitmap.Width * bitmap.Height];

            for (int y = 0, ypos = 0; y < bitmap.Height; ++y, ypos += Math.Abs(data.Stride))
                for (int x = 0, pos = ypos; x < bitmap.Width; ++x, pos += 4)
                {
                    byte b = rgbData[pos + 0];
                    byte g = rgbData[pos + 1];
                    byte r = rgbData[pos + 2];

                    resultData[y * bitmap.Width + x] = (byte)((0.2125 * r) + (0.7154 * g) + (0.0721 * b));
                }

            bitmap.UnlockBits(data);

            return MonoSetBackground(resultData);
        }

        public bool MonoSetText(int lineNumber, string text)
        {
            return LcdPinvoke.LogiLcdMonoSetText(lineNumber, text);
        }

        public bool ColorSetBackground(Byte[] colorBitmap)
        {
            if(colorBitmap.Length != ColorWidth * ColorHeight)
                throw new LogiLcdException("Bitmap size does not match expected size");

            return LcdPinvoke.LogiLcdColorSetBackground(colorBitmap);
        }

        public bool ColorSetTitle(string text, byte red = 255, byte green = 255, byte blue = 255)
        {
            return LcdPinvoke.LogiLcdColorSetTitle(text, red, green, blue);
        }

        public bool ColorSetText(int lineNumber, string text, byte red = 255, byte green = 255, byte blue = 255)
        {
            return LcdPinvoke.LogiLcdColorSetText(lineNumber, text, red, green, blue);
        }
    }
}
