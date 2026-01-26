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
        public string SystemChatMessage { get; set; } =
            "You are a helpful assistant that checks grammar and polishes English text " +
            "or translates it from Ukrainian to English. Try to maintain the original " +
            "level of formality. Don’t be unnecessarily formal, as it might sound rude. " +
            "Return only the polished text without any explanations or additional comments.";
        public string UserChatMessage { get; set; } = "Check grammar and polish or translate if the text is in Ukrainian: {0}";
    }
}
