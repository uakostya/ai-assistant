using System.Runtime.InteropServices;
using System.Windows.Input;

namespace AiAssistant.Services
{
    public class GlobalHotkeyService : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_WIN = 0x0008;
        private const uint VK_A = 0x41;

        private IntPtr _windowHandle;
        private bool _isRegistered;

        public event EventHandler? HotkeyPressed;

        public bool Register(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
            _isRegistered = RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_WIN, VK_A);
            return _isRegistered;
        }

        public void Unregister()
        {
            if (_isRegistered)
            {
                UnregisterHotKey(_windowHandle, HOTKEY_ID);
                _isRegistered = false;
            }
        }

        public void OnHotKeyPressed()
        {
            HotkeyPressed?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            Unregister();
        }
    }
}
