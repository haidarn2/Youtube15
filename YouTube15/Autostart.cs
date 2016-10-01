using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace YouTube15
{
    class Autostart
    {
        private const string RunLocation = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string KeyName = "YouTube15Autostart";

        static private string RunValue()
        {
            return "\"" + Application.ExecutablePath + "\" -autostart";
        }

        static public bool IsEnabled()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RunLocation);
            if (key == null)
                return false;

            string val = (string)key.GetValue(KeyName);
            if (val == null)
                return false;

            return val == RunValue();
        }

        static public void Enable()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RunLocation);
            key.SetValue(KeyName, RunValue());
        }

        static public void Disable()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RunLocation);
            key.DeleteValue(KeyName);
        }
    }
}
