# AI Assistant

A Windows WPF application that provides AI-powered text polishing using OpenAI or Azure OpenAI services.

## Features

- **Global Hotkey (Ctrl+Win+A)**: Select text in any application and press the hotkey to automatically polish and replace it with AI-enhanced version
- **System Tray Integration**: Runs minimized in the system tray for easy access
- **Secure Settings Storage**: API keys are encrypted and stored securely using Windows Data Protection API
- **Dual Provider Support**: Works with both OpenAI and Azure OpenAI services

## Requirements

- Windows OS
- .NET 9.0
- OpenAI API key OR Azure OpenAI service credentials

## Setup

1. Build and run the application
2. On first launch, you'll be prompted to configure your API settings
3. Choose your provider:
   - **OpenAI**: Enter your API key and model name (e.g., gpt-4, gpt-3.5-turbo)
   - **Azure OpenAI**: Enter your Azure endpoint, API key, and deployment name

## Usage

1. **Polish Text**:
   - Select text in any application
   - Press `Ctrl + Win + A`
   - Wait for the AI to process your text
   - The selected text will be automatically replaced with the polished version

3. **Configure Settings**:
   - Right-click the system tray icon
   - Select "Settings"
   - Update your API credentials or switch providers

4. **Exit Application**:
   - Right-click the system tray icon
   - Select "Exit"

### Security

- API keys are encrypted using Windows Data Protection API (DPAPI)
- Settings are stored in: `%APPDATA%\AiAssistant\settings.json`
- Encryption is user-specific and tied to the current user account

## License

This project is licensed under the MIT License. See the LICENSE file for details.
