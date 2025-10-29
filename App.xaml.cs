﻿using System.Windows;
using AiAssistant.Services;
using AiAssistant.Models;
using Hardcodet.Wpf.TaskbarNotification;

namespace AiAssistant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon? _trayIcon;
        private GlobalHotkeyService? _hotkeyService;
        private ClipboardService? _clipboardService;
        private SettingsService? _settingsService;
        private AppSettings? _settings;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Initialize services
            _settingsService = new SettingsService();
            _settings = _settingsService.LoadSettings();
            _clipboardService = new ClipboardService();
            _hotkeyService = new GlobalHotkeyService();

            // Create system tray icon using embedded icon resource
            _trayIcon = new TaskbarIcon
            {
                IconSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/icon.ico")),
                ToolTipText = "AI Assistant - Press Ctrl+Win+A to polish text",
                ContextMenu = (System.Windows.Controls.ContextMenu)FindResource("TrayContextMenu")
            };

            _trayIcon.TrayMouseDoubleClick += (s, args) => ShowSettings();

            // Set main window to invisible
            MainWindow = new MainWindow();
            MainWindow.Visibility = Visibility.Hidden;

            // Register global hotkey
            var handle = new System.Windows.Interop.WindowInteropHelper(MainWindow).EnsureHandle();
            if (!_hotkeyService.Register(handle))
            {
                MessageBox.Show("Failed to register global hotkey Ctrl+Win+A", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            _hotkeyService.HotkeyPressed += OnHotkeyPressed;

            // Add hook for hotkey messages
            System.Windows.Interop.HwndSource.FromHwnd(handle)?.AddHook(HwndHook);

            // Show welcome message if no settings
            if (string.IsNullOrWhiteSpace(_settings.OpenAIApiKey) &&
                string.IsNullOrWhiteSpace(_settings.AzureApiKey))
            {
                var result = MessageBox.Show(
                    "Welcome to AI Assistant!\n\nWould you like to configure your API settings now?",
                    "First Time Setup",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    ShowSettings();
                }
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                _hotkeyService?.OnHotKeyPressed();
                handled = true;
            }
            return IntPtr.Zero;
        }

        private async void OnHotkeyPressed(object? sender, EventArgs e)
        {
            try
            {
                if (_settings == null || _settingsService == null || _clipboardService == null)
                    return;

                // Get selected text
                var selectedText = await _clipboardService.GetSelectedTextAsync();

                if (string.IsNullOrWhiteSpace(selectedText))
                {
                    MessageBox.Show("No text selected. Please select some text and try again.",
                        "AI Assistant", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Show processing window
                var processingWindow = new ProcessingWindow();
                processingWindow.Show();

                try
                {
                    // Process text with AI
                    var aiService = new AiService(_settings);
                    var polishedText = await aiService.PolishTextAsync(selectedText);

                    processingWindow.UpdateStatus("Replacing text...");

                    // Replace selected text
                    await _clipboardService.ReplaceSelectedTextAsync(polishedText);

                    processingWindow.Close();
                }
                catch (Exception ex)
                {
                    processingWindow.Close();
                    MessageBox.Show($"Error processing text: {ex.Message}\n\nPlease check your API settings.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowSettings()
        {
            if (_settingsService == null || _settings == null)
                return;

            var settingsWindow = new SettingsWindow(_settingsService, _settings);
            if (settingsWindow.ShowDialog() == true)
            {
                _settings = settingsWindow.Settings;
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _hotkeyService?.Dispose();
            _trayIcon?.Dispose();
        }
    }
}
