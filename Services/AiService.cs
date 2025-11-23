using AiAssistant.Models;
using AiAssistant.Models.Enums;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace AiAssistant.Services
{
    public class AiService
    {
        private readonly AppSettings _settings;

        public AiService(AppSettings settings)
        {
            _settings = settings;
        }

        public async Task<string> PolishTextAsync(string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return text;
                }

                ChatClient client;

                if (_settings.ApiProvider == ApiProvider.Azure)
                {
                    if (
                        string.IsNullOrWhiteSpace(_settings.AzureEndpoint)
                        || string.IsNullOrWhiteSpace(_settings.AzureApiKey)
                    )
                    {
                        throw new InvalidOperationException("Azure settings are not configured");
                    }

                    var azureClient = new AzureOpenAIClient(
                        new Uri(_settings.AzureEndpoint),
                        new AzureKeyCredential(_settings.AzureApiKey)
                    );

                    client = azureClient.GetChatClient(_settings.DeploymentName);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(_settings.OpenAIApiKey))
                    {
                        throw new InvalidOperationException("OpenAI API key is not configured");
                    }

                    var openAIClient = new OpenAI.OpenAIClient(_settings.OpenAIApiKey);
                    client = openAIClient.GetChatClient(_settings.Model);
                }

                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage(
                        "You are a helpful assistant that checks grammar and polishes text. " +
                        "Try to maintain the original level of formality. Don’t be unnecessarily formal, as it might sound rude. " +
                        "Return only the polished text without any explanations or additional comments."
                    ),
                    new UserChatMessage($"Check grammar and polish: {text}"),
                };

                var response = await client.CompleteChatAsync(messages);
                return response.Value.Content[0].Text;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to process text: {ex.Message}", ex);
            }
        }
    }
}
