namespace AiAssistant.Models
{
    public class AppSettings
    {
        public string ApiProvider { get; set; } = "OpenAI"; // "OpenAI" or "Azure"
        public string OpenAIApiKey { get; set; } = string.Empty;
        public string AzureEndpoint { get; set; } = string.Empty;
        public string AzureApiKey { get; set; } = string.Empty;
        public string DeploymentName { get; set; } = "gpt-4";
        public string Model { get; set; } = "gpt-4";
    }
}
