using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AiAssistant.Models;

namespace AiAssistant.Services
{
    public class SettingsService
    {
        private static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AiAssistant",
            "settings.json"
        );

        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("AiAssistant-Entropy-2024");

        public AppSettings LoadSettings()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                {
                    return new AppSettings();
                }

                var encryptedData = File.ReadAllBytes(SettingsFilePath);
                var decryptedData = ProtectedData.Unprotect(
                    encryptedData,
                    Entropy,
                    DataProtectionScope.CurrentUser
                );
                var json = Encoding.UTF8.GetString(decryptedData);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void SaveSettings(AppSettings settings)
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(
                    settings,
                    new JsonSerializerOptions { WriteIndented = true }
                );
                var dataToEncrypt = Encoding.UTF8.GetBytes(json);
                var encryptedData = ProtectedData.Protect(
                    dataToEncrypt,
                    Entropy,
                    DataProtectionScope.CurrentUser
                );
                File.WriteAllBytes(SettingsFilePath, encryptedData);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to save settings", ex);
            }
        }
    }
}
