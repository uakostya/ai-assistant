using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace AiAssistant.Services
{
    public class ClipboardService
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void keybd_event(
            byte bVk,
            byte bScan,
            uint dwFlags,
            UIntPtr dwExtraInfo
        );

        private const byte VK_CONTROL = 0x11;
        private const byte VK_C = 0x43;
        private const byte VK_V = 0x56;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        private IntPtr _lastForegroundWindow;

        private const int RetryCount = 3;

        public async Task<string> GetSelectedTextAsync()
        {
            for (int attempt = 0; attempt < RetryCount - 1; attempt++)
            {
                var selectedText = await GetSelectedTextInternalAsync();
                if (!string.IsNullOrEmpty(selectedText))
                {
                    return selectedText;
                }
            }
            return string.Empty;
        }

        private async Task<string> GetSelectedTextInternalAsync()
        {
            try
            {
                _lastForegroundWindow = GetForegroundWindow();

                string previousClipboard = string.Empty;
                if (Clipboard.ContainsText())
                {
                    previousClipboard = Clipboard.GetText();
                }

                Clipboard.Clear();
                await Task.Delay(100);

                if (_lastForegroundWindow != IntPtr.Zero)
                {
                    SetForegroundWindow(_lastForegroundWindow);
                    await Task.Delay(100);
                }

                SendCtrlC();
                await Task.Delay(300);

                var selectedText = string.Empty;
                if (Clipboard.ContainsText())
                {
                    selectedText = Clipboard.GetText();
                }

                if (string.IsNullOrEmpty(selectedText) && !string.IsNullOrEmpty(previousClipboard))
                {
                    Clipboard.SetText(previousClipboard);
                }

                return selectedText;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetSelectedTextAsync error: {ex.Message}");
                return string.Empty;
            }
        }

        private void SendCtrlC()
        {
            keybd_event(VK_CONTROL, 0, 0, UIntPtr.Zero);
            Thread.Sleep(50);

            keybd_event(VK_C, 0, 0, UIntPtr.Zero);
            Thread.Sleep(50);

            keybd_event(VK_C, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(50);

            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private void SendCtrlV()
        {
            if (_lastForegroundWindow != IntPtr.Zero)
            {
                SetForegroundWindow(_lastForegroundWindow);
                Thread.Sleep(100);
            }

            keybd_event(VK_CONTROL, 0, 0, UIntPtr.Zero);
            Thread.Sleep(50);

            keybd_event(VK_V, 0, 0, UIntPtr.Zero);
            Thread.Sleep(50);

            keybd_event(VK_V, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(50);

            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        public async Task ReplaceSelectedTextAsync(string newText)
        {
            try
            {
                // Copy new text to clipboard using UI thread (STA)
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Clipboard.SetText(newText);
                });

                await Task.Delay(100);

                SendCtrlV();

                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ReplaceSelectedTextAsync error: {ex.Message}");
            }
        }
    }
}
