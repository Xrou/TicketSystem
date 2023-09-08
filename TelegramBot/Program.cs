using Microsoft.VisualBasic;
using System.Text.Json.Nodes;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TelegramBot;

namespace ITLTelegram
{
    class Launch
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Запуск бота");

                ITelegramBotClient bot = new TelegramBotClient(Constants.BOT_TOKEN);

                var cts = new CancellationTokenSource();
                var cancellationToken = cts.Token;
                var receiverOptions = new ReceiverOptions()
                {
                    AllowedUpdates = Array.Empty<UpdateType>(),
                    ThrowPendingUpdates = true,
                };

                bot.StartReceiving(updateHandler: Handlers.HandleUpdateAsync,
                                   pollingErrorHandler: Handlers.PollingErrorHandler,
                                   receiverOptions: receiverOptions,
                                   cancellationToken: cts.Token);

                Thread callbackListener = new Thread(new ParameterizedThreadStart(CallbackListener.Listen));

                callbackListener.Start(bot);
            }
            catch
            {
            }

            Console.ReadLine();
        }
    }
}