internal static class ChatControllerHelpers
{
    private static readonly HttpClient httpClient = new();
    public static HttpClient _httpClient = httpClient;
}