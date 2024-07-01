using DependencyFinder.App.Entities;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static DependencyFinder.App.Utils.EnumUtils;


namespace DependencyFinder.App.Controller;

public class ChatController
{
    public static async Task<string> SendMessageAsync(string chatMessage)
    {
        string chatId = Environment.GetEnvironmentVariable("CHAT_ID")!;
        string token = Environment.GetEnvironmentVariable("TOKEN")!;
        string chatUrl = Environment.GetEnvironmentVariable("CHAT_URL")!;
        CustomWriteLine(UsageEnum.Processing, "Sending to chat...");
        try
        {
            var requestUri = chatUrl;
            CustomWriteLine(UsageEnum.Log, requestUri);
            string templateChat = @$"{File.ReadAllText("./App/Controller/template/template.txt")}
```SQL
{chatMessage}
```";
            string signalRId = Environment.GetEnvironmentVariable("SIGNALR_ID")!;
            ChatDTO chatDto = new()
            {
                ChatId = chatId,
                Chat = templateChat,
                Culture = "fr-FR",
                DataSources = [],
                SignalRConnectionId = signalRId,
                PromptOptions = new PromptOption()
                {
                    Temperature = 1,
                    Model = "gpt4o",
                    Capacities = []
                }
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(chatDto), Encoding.UTF8, "application/json");
            ChatControllerHelpers.
                        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await ChatControllerHelpers._httpClient.PostAsync(requestUri, requestContent);
            await Task.Delay(10000);
            if (response.IsSuccessStatusCode)
            {
                string chatResponse = Environment.GetEnvironmentVariable("CHAT_RESPONSE")!;
                CustomWriteLine(UsageEnum.Log, chatResponse);
                var json = await ChatControllerHelpers._httpClient.GetAsync(chatResponse);


                if (json.IsSuccessStatusCode)
                {
                    var jsonResponse = await json.Content.ReadFromJsonAsync<List<Message>>();
                    var message = jsonResponse?.LastOrDefault()?.Message1; //?? "No message";
                    return message?.ToString() ?? "NoMessage";
                }
                else
                {
                    throw new Exception($"Failed to get chat response. Status code: {json.StatusCode}. Reason: {json.ReasonPhrase}");
                }
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

