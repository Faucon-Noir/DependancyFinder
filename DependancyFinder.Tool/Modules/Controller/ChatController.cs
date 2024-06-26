using System.Text;
using System.Text.Json;
using DependencyFinder.Tool.Modules.Entities;
using static DependencyFinder.Tool.Modules.EnumModule;

namespace DependancyFinder.Tool.Modules.ChatController;

public class ChatController
{
    public static HttpClient _httpClient = new();

    public static async Task<string> SendMessageAsync(string chatId, string token, string chatMessage, string chatUrl)
    {
        CustomWriteLine(UsageEnum.Processing, "Sending to chat...");
        try
        {
            var requestUri = chatUrl;
            string templateChat = @$"{File.ReadAllText("./DependancyFinder.Tool/Modules/Controller/template/template.txt")}
```SQL
{chatMessage}
```";
            ChatDTO chatDto = new()
            {
                chatId = chatId,
                chat = templateChat,
                token = token
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(chatDto), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(requestUri, requestContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            else
            {
                CustomWriteLine(UsageEnum.Error, $"Failed to send message. Status code: {response.StatusCode}. Reason: {response.ReasonPhrase}");
                return $"{response.StatusCode} {response.ReasonPhrase}";
            }
        }
        catch (Exception e)
        {
            CustomWriteLine(UsageEnum.Error, $"Error ChatController: {e.Message}");
            return e.Message;
        }
    }
}

