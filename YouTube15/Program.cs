using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Resources;
using System.ServiceModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace YouTube15
{
    [ServiceContract]
    public interface ISpoti15WCF
    {
        [OperationContract]
        bool Shutdown();

        [OperationContract]
        bool ShowTray();
    }

    class Spoti15WcfImpl : ISpoti15WCF
    {
        public bool Shutdown()
        {
            Program.Shutdown();
            return true;
        }

        public bool ShowTray()
        {
            Program.ShowTray();
            return true;
        }
    }

    class Program
    {
        private static PrivateFontCollection pFonts = new PrivateFontCollection();

        public static FontFamily[] FontFamilies
        {
            get
            {
                if(pFonts.Families.Length == 0)
                    LoadFonts();

                return pFonts.Families;
            }
        }

        public static FontFamily GetFontFamily(string family)
        {
            family = family.ToLower();

            foreach(FontFamily f in FontFamilies)
            {
                if(f.Name.ToLower() == family)
                {
                    return f;
                }
            }

            return null;
        }

        public static void LoadFonts()
        {
            ResourceSet res = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in res)
            {
                string resKey = (String)entry.Key;
                if (resKey == null || !resKey.StartsWith("font_"))
                    continue;

                byte[] resVal = (byte[])entry.Value;

                IntPtr data = Marshal.AllocCoTaskMem(resVal.Length);
                Marshal.Copy(resVal, 0, data, resVal.Length);

                pFonts.AddMemoryFont(data, resVal.Length);

                Marshal.FreeCoTaskMem(data);
            }
        }

        private static NotifyIcon notico;
        private static MenuItem autostartItem;
        private static ServiceHost host;

        static void Main(string[] args)
        {
            using (ChannelFactory<ISpoti15WCF> spotFactory = new ChannelFactory<ISpoti15WCF>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/Spoti15WCF")))
            {
                try
                {
                    ISpoti15WCF iface = spotFactory.CreateChannel();
                    iface.Shutdown();
                    spotFactory.Close();

                    System.Threading.Thread.Sleep(1000);
                }
                catch
                {
                    spotFactory.Abort();
                    spotFactory.Close();
                }
            }

            host = new ServiceHost(typeof(Spoti15WcfImpl), new Uri[] { new Uri("net.pipe://localhost") });
            host.AddServiceEndpoint(typeof(ISpoti15WCF), new NetNamedPipeBinding(), "Spoti15WCF");
            host.Open();

            if (args.Length == 0 || args[0] != "-autostart")
            {
                Properties.Settings.Default.HideIcon = false;
                Properties.Settings.Default.Save();
            }

            ContextMenu cm = new ContextMenu();
            MenuItem menu;

            menu = new MenuItem();
            menu.Text = "&Hide tray icon";
            menu.Click += HideClick;
            cm.MenuItems.Add(menu);

            menu = new MenuItem();
            menu.Checked = Autostart.IsEnabled();
            menu.Text = "&Autostart";
            menu.Click += AutostartClick;
            cm.MenuItems.Add(menu);
            autostartItem = menu;

            cm.MenuItems.Add("-");

            menu = new MenuItem();
            menu.Text = "E&xit";
            menu.Click += ExitClick;
            cm.MenuItems.Add(menu);

            notico = new NotifyIcon();
            notico.Text = "YouTube15";
            notico.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notico.ContextMenu = cm;
            notico.Visible = !Properties.Settings.Default.HideIcon;

            YouTube15 spoti15 = new YouTube15();

            Application.Run();
                        
            GC.KeepAlive(spoti15);
            host.Close();
        }

        private static void AutostartClick(Object sender, EventArgs e)
        {
            if (Autostart.IsEnabled())
                Autostart.Disable();
            else
                Autostart.Enable();

            autostartItem.Checked = !autostartItem.Checked;
        }

        private static void HideClick(Object sender, EventArgs e)
        {
            Properties.Settings.Default.HideIcon = true;
            Properties.Settings.Default.Save();

            notico.Visible = false;
        }

        public static void ShowTray()
        {
            Properties.Settings.Default.HideIcon = false;
            Properties.Settings.Default.Save();

            notico.Visible = true;
        }

        private static void ExitClick(Object sender, EventArgs e)
        {
            Shutdown();
        }

        public static void Shutdown()
        {
            if(notico != null)
                notico.Dispose();

            Application.Exit();
        }
    }
}
