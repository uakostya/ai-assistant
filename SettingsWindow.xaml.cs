using System.Windows;
using System.Windows.Controls;
using AiAssistant.Models;
using AiAssistant.Services;

namespace AiAssistant
{
    public partial class SettingsWindow : Window
    {
      private readonly SettingsService _settingsService;
        private AppSettings _settings;

        public AppSettings Settings => _settings;

        public SettingsWindow(SettingsService settingsService, AppSettings settings)
        {
   InitializeComponent();
            _settingsService = settingsService;
     _settings = settings;
            LoadSettings();
   }

        private void LoadSettings()
        {
         // Set API provider
  if (_settings.ApiProvider == "Azure")
            {
        ApiProviderComboBox.SelectedIndex = 1;
          }
      else
  {
      ApiProviderComboBox.SelectedIndex = 0;
      }

   // Load OpenAI settings
    if (!string.IsNullOrEmpty(_settings.OpenAIApiKey))
            {
         OpenAIApiKeyBox.Password = _settings.OpenAIApiKey;
        }
            ModelTextBox.Text = _settings.Model;

     // Load Azure settings
     AzureEndpointTextBox.Text = _settings.AzureEndpoint;
        if (!string.IsNullOrEmpty(_settings.AzureApiKey))
            {
   AzureApiKeyBox.Password = _settings.AzureApiKey;
            }
            DeploymentNameTextBox.Text = _settings.DeploymentName;

            UpdateVisibility();
        }

        private void ApiProviderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
       UpdateVisibility();
        }

private void UpdateVisibility()
        {
        if (ApiProviderComboBox.SelectedIndex == 0) // OpenAI
  {
       OpenAISettingsGroup.Visibility = Visibility.Visible;
      AzureSettingsGroup.Visibility = Visibility.Collapsed;
    }
    else // Azure
 {
        OpenAISettingsGroup.Visibility = Visibility.Collapsed;
     AzureSettingsGroup.Visibility = Visibility.Visible;
 }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
        try
         {
   _settings.ApiProvider = ApiProviderComboBox.SelectedIndex == 0 ? "OpenAI" : "Azure";
          _settings.OpenAIApiKey = OpenAIApiKeyBox.Password;
      _settings.Model = ModelTextBox.Text;
           _settings.AzureEndpoint = AzureEndpointTextBox.Text;
         _settings.AzureApiKey = AzureApiKeyBox.Password;
      _settings.DeploymentName = DeploymentNameTextBox.Text;

      _settingsService.SaveSettings(_settings);
             MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
         DialogResult = true;
       Close();
         }
     catch (Exception ex)
      {
        MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
{
        DialogResult = false;
            Close();
        }
    }
}
