using System.Net.Http.Headers;
using System.Net;

namespace TicketSystem.External
{
    public class SUZApi
    {
        private readonly static string username = "cl\\bot.telegram";
        private readonly static string password = "Te1e6r@mb0t";
        private readonly static string url = "http://localhost:81/";

        private HttpClient httpClient;

        public SUZApi()
        {
            var uri = new Uri(url);
            var credentialsCache = new CredentialCache { { uri, "NTLM", CredentialCache.DefaultNetworkCredentials } };
            var handler = new HttpClientHandler { Credentials = new NetworkCredential(username, password) };
            httpClient = new HttpClient(handler) { BaseAddress = uri, Timeout = new TimeSpan(0, 0, 30) };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<int?> SendCommentAsync(string userId, string telegramId, string messageId, string message, string? tgCommId = null, string tgChatId = null)
        {
            var contentList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("telegram_id", telegramId),
                new KeyValuePair<string, string>("user_id", userId),
                new KeyValuePair<string, string>("message_id", messageId),
                new KeyValuePair<string, string>("comment_type", "2"),
                new KeyValuePair<string, string>("source_id", "30"),
                new KeyValuePair<string, string>("comment_text", message),
            };

            if (tgCommId != null && tgChatId != null)
            {
                contentList.Add(new KeyValuePair<string, string>("tg_comm_id", tgCommId));
                contentList.Add(new KeyValuePair<string, string>("tg_chat_id", tgChatId));
            }

            var content = new FormUrlEncodedContent(contentList);

            var response = await httpClient.PostAsync("telebotAPI/addComment_telegram", content);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            var text = await response.Content.ReadAsStringAsync();

            string try_comment_id = text.Replace("<br>", "").Replace("<br >", "");
            int comment_id;

            if (int.TryParse(try_comment_id, out comment_id))
            {
                return comment_id;
            }
            else
            {
                return null;
            }
        }

        public async Task<long?> SendMessageAsync(long senderId, long telegramId, string message)
        {
            var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("sender_id", senderId.ToString()),
                new KeyValuePair<string, string>("telegram", telegramId.ToString()),
                new KeyValuePair<string, string>("subject_id", "1"),
                new KeyValuePair<string, string>("message", message),
            });

            var response = await httpClient.PostAsync("telebotAPI/addMessage_telegram", content);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            var text = await response.Content.ReadAsStringAsync();

            string try_message_id = text.Substring(2);
            long message_id;

            if (long.TryParse(try_message_id, out message_id))
            {
                return message_id;
            }
            else
            {
                return null;
            }
        }
    }
}
