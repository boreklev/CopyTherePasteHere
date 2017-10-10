using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CopyTherePasteHere
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly static int SERVER_PORT = 8179;
        public readonly static double VERSION = 0.7;
        public readonly static double MAX_VERSION = 0.7;
        public readonly static double MIN_VERSION = 0.1;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private bool isExit;
        private TcpServer tcp;
        private UDPBroadcastServer udp;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.Closing += MainWindow_Closing;
            MainWindow.StateChanged += MainWindow_StateChanged1;

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.DoubleClick += (s, args) => ShowMainWindow();
            notifyIcon.Icon = CopyTherePasteHere.Properties.Resources.ICON;
            notifyIcon.Visible = true;

            CreateContextMenu();

            udp = new UDPBroadcastServer();
            tcp = new TcpServer();
            tcp.Start();
        }

        private void MainWindow_StateChanged1(object sender, EventArgs e)
        {
            if (MainWindow.WindowState == WindowState.Minimized)
            {
                MainWindow.ShowInTaskbar = false;
            }
        }

        private void CreateContextMenu()
        {
            notifyIcon.ContextMenuStrip =
              new System.Windows.Forms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Visa").Click += (s, e) => ShowMainWindow();
            notifyIcon.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add("Avsluta").Click += (s, e) => ExitApplication();
        }

        private void ExitApplication()
        {
            isExit = true;
            MainWindow.Close();
            notifyIcon.Dispose();
            notifyIcon = null;
            udp.Stop();
            tcp.Stop();
        }

        private void ShowMainWindow()
        {
            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.WindowState = WindowState.Normal;
                }
                MainWindow.Activate();
            }
            else
            {
                MainWindow.Show();
            }
            MainWindow.ShowInTaskbar = true;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!isExit)
            {
                e.Cancel = true;
                MainWindow.ShowInTaskbar = false;
                MainWindow.Hide(); // A hidden window can be shown again, a closed one not
            }
        }
        
    }
}
