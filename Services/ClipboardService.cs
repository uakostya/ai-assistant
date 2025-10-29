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

        public async Task<string> GetSelectedTextAsync()
        {
            try
            {
                // Store the foreground window BEFORE any clipboard operations
                _lastForegroundWindow = GetForegroundWindow();

                // Store the current clipboard content
                string previousClipboard = string.Empty;
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        previousClipboard = Clipboard.GetText();
                    }
                }
                catch { }

                // Clear clipboard
                await ClearClipboardAsync();
                await Task.Delay(100);

                // Make sure the original window still has focus
                if (_lastForegroundWindow != IntPtr.Zero)
                {
                    SetForegroundWindow(_lastForegroundWindow);
                    await Task.Delay(100);
                }

                // Simulate Ctrl+C with proper key event timing
                SendCtrlC();

                // Wait longer for clipboard to be populated
                await Task.Delay(300);

                var selectedText = string.Empty;
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        selectedText = Clipboard.GetText();
                    }
                }
                catch { }

                // Restore previous clipboard if no text was selected
                if (string.IsNullOrEmpty(selectedText) && !string.IsNullOrEmpty(previousClipboard))
                {
                    try
                    {
                        Clipboard.SetText(previousClipboard);
                    }
                    catch { }
                }

                return selectedText;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetSelectedTextAsync error: {ex.Message}");
                return string.Empty;
            }
        }

        private async Task ClearClipboardAsync()
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    Clipboard.Clear();
                    return;
                }
                catch
                {
                    await Task.Delay(50);
                }
            }
        }

        private void SendCtrlC()
        {
            // Press Ctrl
            keybd_event(VK_CONTROL, 0, 0, UIntPtr.Zero);
            Thread.Sleep(50);

            // Press C
            keybd_event(VK_C, 0, 0, UIntPtr.Zero);
            Thread.Sleep(50);

            // Release C
            keybd_event(VK_C, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(50);

            // Release Ctrl
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private void SendCtrlV()
        {
            // Restore focus to the original window
            if (_lastForegroundWindow != IntPtr.Zero)
            {
                SetForegroundWindow(_lastForegroundWindow);
                Thread.Sleep(100);
            }

            // Press Ctrl
            keybd_event(VK_CONTROL, 0, 0, UIntPtr.Zero);
            Thread.Sleep(50);

            // Press V
            keybd_event(VK_V, 0, 0, UIntPtr.Zero);
            Thread.Sleep(50);

            // Release V
            keybd_event(VK_V, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(50);

            // Release Ctrl
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        public async Task ReplaceSelectedTextAsync(string newText)
        {
            try
            {
                // Copy new text to clipboard using UI thread (STA)
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            Clipboard.SetText(newText);
                            return;
                        }
                        catch
                        {
                            Thread.Sleep(100);
                        }
                    }
                });

                await Task.Delay(100);

                // Simulate Ctrl+V
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
