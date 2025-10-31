using AiAssistant.Models.Enums;

namespace AiAssistant.Models
{
    public class AppSettings
    {
        public ApiProvider ApiProvider { get; set; } = ApiProvider.OpenAI;
        public string OpenAIApiKey { get; set; } = string.Empty;
        public string AzureEndpoint { get; set; } = string.Empty;
        public string AzureApiKey { get; set; } = string.Empty;
        public string DeploymentName { get; set; } = "gpt-5-chat";
        public string Model { get; set; } = "gpt-5-chat";
    }
}
