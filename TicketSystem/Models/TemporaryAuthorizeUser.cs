namespace TicketSystem.Models
{
    public class TemporaryAuthorizeUser
    {
        public long TelegramId { get; set; } 
        public string Code { get; set; }

        public TemporaryAuthorizeUser(long telegramId, string code)
        {
            TelegramId = telegramId;
            Code = code;
        }
    }
}
