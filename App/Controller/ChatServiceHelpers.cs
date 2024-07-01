internal static class ChatServiceHelpers
{
    private static readonly HttpClient httpClient = new();
    public static HttpClient _httpClient = httpClient;
}