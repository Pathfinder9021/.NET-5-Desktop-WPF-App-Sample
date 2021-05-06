using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using WpfSample.App.Configuration;
using WpfSample.App.Helpers;
using WpfSample.App.Localization;
using WpfSample.App.Resources;
using WpfSample.App.Views;

namespace WpfSample.App
{
    public partial class App : Application
    {
        private readonly IHost host;
        private MainWindow mainWindow;

        private LocalizationManager LocalizationManager => LocalizationManager.Instance;
        private GeneralOptions GeneralOptions => ServiceProvider.GetRequiredService<IOptionsMonitor<GeneralOptions>>().CurrentValue;
        private SystemIntegrationOptions SystemIntegrationOptions => ServiceProvider.GetRequiredService<IOptionsMonitor<SystemIntegrationOptions>>().CurrentValue;

        private SkinEngine SkinEngine { get; set; }
        private AppSplashScreen SplashScreen { get; set; }
        public AppTrayIcon TrayIcon { get; private set; }

        public static IServiceProvider ServiceProvider { get; private set; }
  
        public App()
        {
            host = HostFactory.Create();
            ServiceProvider = host.Services;
        }

        private void InitializeLocalizationManager()
        {
            LocalizationManager.SecondaryResourceManagers.Add(LocalizedStrings.ResourceManager);
            LocalizationManager.CurrentCulture = CultureInfo.GetCultureInfo(GeneralOptions.Language);
        }

        private void InitializeSkinEngine() 
        {
            SkinEngine = new SkinEngine(this);

            try
            {
                SkinEngine.ApplySkin(GeneralOptions.Skin);
            }
            catch (Exception ex)
            {
                SkinEngine.ApplySkin(Constants.Skins.Default);
            }
        }

        private void InitializeSpashScreen(bool isSplashScreenVisible)
        {
            SplashScreen = new AppSplashScreen();
            SplashScreen.IsVisible = isSplashScreenVisible;
        }

        private void InitializeTrayIcon()
        {
            TrayIcon = new AppTrayIcon();
        }

        private void InitializeMainWindow()
        {
            Dispatcher.Invoke(() => {
                mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.Closing += MainWindowClosingEventHandler;

                MainWindow = mainWindow;
            });
        }

        private async Task StartApplication()
        {
            InitializeLocalizationManager();
            InitializeSkinEngine();
            InitializeSpashScreen(isSplashScreenVisible: true);

            await host.StartAsync();

            // To remove in the future. Just for testing
            System.Threading.Thread.Sleep(4000);

            InitializeMainWindow();
            InitializeTrayIcon();

            if (SystemIntegrationOptions.ShowTrayIcon)
            {
                TrayIcon.IsVisible = true;
            }

            SplashScreen.IsVisible = false;

            if (!SystemIntegrationOptions.ShowTrayIcon || !SystemIntegrationOptions.MinimizeToTrayOnStartup)
            {
                ShowMainWindow();
            }
        }

        private async Task CloseApplication()
        {
            using (host)
            {
                TrayIcon.Dispose();
                SplashScreen.Dispose();

                await host.StopAsync(TimeSpan.FromSeconds(5));
            }
        }

        private void ShowMainWindow()
        {
            Dispatcher.Invoke(() =>
            {
                if (MainWindow.IsVisible)
                {
                    if (mainWindow.WindowState == WindowState.Minimized)
                    {
                        mainWindow.WindowState = WindowState.Normal;
                    }

                    MainWindow.Activate();
                }
                else
                {
                    MainWindow.Show();
                }
            });
        }

        private void MainWindowClosingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TrayIcon.IsVisible)
            {
                e.Cancel = true;
                Dispatcher.Invoke(() => MainWindow.Hide());
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // To prevent opening of a second app instance
            if (!MutexLockHelper.CreateMutex())
            {
                Current.Shutdown();
                return;
            }

            base.OnStartup(e);

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await StartApplication();
                }
                catch (Exception ex)
                {

                }
            });
        }

        protected async override void OnExit(ExitEventArgs e)
        {
            await CloseApplication();
            base.OnExit(e);
        }          
    }

    public class AppSplashScreen : IDisposable
    {
        private string message;
        private SplashScreenWindow splashScreenWindow;

        public bool IsVisible
        {
            get => splashScreenWindow != null;
            set
            {
                if (value)
                {
                    InitializeSplashScreen();
                }
                else
                {
                    DisposeSplashScreen();
                }
            }
        }

        public string Message {
            get => splashScreenWindow?.message.Text;
            set => SetSplashScreenMessage(value);
        }

        public double? Progress
        {
            get => splashScreenWindow?.progressBar.Value;
            set => SetSplashScreenProgress(value ?? 0);
        }

        private void InitializeSplashScreen()
        {
            Application.Current.Dispatcher.Invoke(() => {
                splashScreenWindow = new SplashScreenWindow();
                splashScreenWindow.message.Text = message;
                splashScreenWindow.Show();
            });
        }

        private void DisposeSplashScreen()
        {
            Application.Current.Dispatcher.Invoke(() => {
                if (splashScreenWindow == null)
                {
                    return;
                }

                splashScreenWindow.Close();
                splashScreenWindow = null;
            });
        }

        private void SetSplashScreenMessage(string message)
        {
            this.message = message;

            if (!IsVisible) 
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                splashScreenWindow.message.Text = message;
            });
        }

        private void SetSplashScreenProgress(double progress)
        {
            if (IsVisible)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (splashScreenWindow.progressBar.IsIndeterminate)
                {
                    splashScreenWindow.progressBar.IsIndeterminate = false;
                }

                splashScreenWindow.progressBar.Value = progress;
            });
        }

        public void Dispose()
        {
            DisposeSplashScreen();
        }
    }

    public class AppTrayIcon : IDisposable
    {
        private System.Windows.Forms.NotifyIcon trayIcon;
        private LocalizationManager LocalizationManager => LocalizationManager.Instance;

        public bool IsVisible
        {
            get => trayIcon != null && trayIcon.Visible;
            set
            {
                if (value)
                {
                    InitializeTrayIcon();
                }
                else
                {
                    DisposeTrayIcon();
                }
            }
        }

        private void InitializeTrayIcon()
        {
            Application.Current.Dispatcher.Invoke(() => {
                trayIcon = new System.Windows.Forms.NotifyIcon();
                trayIcon.Text = LocalizationManager.GetString("ApplicationName");
                trayIcon.Icon = Resources.Resources.TrayIcon;
                trayIcon.ContextMenuStrip = BuildTrayContextMenu();
                trayIcon.DoubleClick += (sender, args) => ShowApplicationMainWindow();
                trayIcon.Visible = true;
            });
        }

        private System.Windows.Forms.ContextMenuStrip BuildTrayContextMenu()
        {
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();

            contextMenu.Items.Add(LocalizationManager.GetString("TrayContextMenuShowApplication"),
                null,
                (sender, args) => ShowApplicationMainWindow()
            );

            contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

            contextMenu.Items.Add(LocalizationManager.GetString("TrayContextMenuOptions"),
              null,
              (sender, args) => { }
            );

            contextMenu.Items.Add(LocalizationManager.GetString("TrayContextMenuAbout"),
               null,
               (sender, args) => { }
             );

            contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

            contextMenu.Items.Add(LocalizationManager.GetString("TrayContextMenuExitApplication"),
                null,
                (sender, args) => ExitApplication()
            );

            return contextMenu;
        }

        private void ShowApplicationMainWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow == null)
                {
                    return;
                }

                if (mainWindow.IsVisible)
                {
                    if (mainWindow.WindowState == WindowState.Minimized)
                    {
                        mainWindow.WindowState = WindowState.Normal;
                    }

                    mainWindow.Activate();
                }
                else
                {
                    mainWindow.Show();
                }
            });
        }

        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }

        private void DisposeTrayIcon()
        {
            var dispatcher = App.Current.Dispatcher;
            dispatcher.Invoke(() => {
                if (trayIcon == null)
                {
                    return;
                }

                trayIcon.Visible = false;
                trayIcon = null;
            });
        }

        public void Dispose()
        {
            DisposeTrayIcon();
        }
    }
}
