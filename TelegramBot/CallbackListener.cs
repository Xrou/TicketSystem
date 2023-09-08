using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using static System.Net.Mime.MediaTypeNames;
using Telegram.Bot.Requests.Abstractions;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TelegramBot
{
    public static class CallbackListener
    {
        public static async void Listen(object botObj)
        {
            TelegramBotClient bot = (TelegramBotClient)botObj;
            HttpListener server = new HttpListener();

            server.Prefixes.Add("http://localhost:8888/");
            server.Start(); // начинаем прослушивать входящие подключения

            while (true)
            {
                var context = await server.GetContextAsync();

                var response = context.Response;
                var url = context.Request.Url;

                response.StatusCode = (int)HttpStatusCode.NotFound;

                string? responseText = null;

                if (url.LocalPath == "/code/")
                {
                    int code;
                    string codeRequest = url.Query.Split("=")[1];

                    if (int.TryParse(codeRequest, out code))
                    {
                        if (StaticStorage.Codes.ContainsKey(code))
                        {
                            response.StatusCode = (int)HttpStatusCode.OK;
                            responseText = StaticStorage.Codes[code].ToString(); // telegram id
                            await bot.SendTextMessageAsync(StaticStorage.Codes[code], "Ожидайте регистрацию");
                            StaticStorage.Codes.Remove(code);
                        }
                    }
                }
                else if (url.LocalPath == "/registrationVerified/")
                {
                    long telegramId;
                    string telegramRequest = url.Query.Split("=")[1];
                    
                    if (long.TryParse(telegramRequest, out telegramId))
                    {
                        response.StatusCode = (int)HttpStatusCode.OK;
                        await bot.SendTextMessageAsync(telegramId, "Ваша учетная запись создана");
                    }
                }

                if (responseText != null)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                    response.ContentLength64 = buffer.Length;
                    using Stream output = response.OutputStream;
                    await output.WriteAsync(buffer);
                    await output.FlushAsync();
                }
            }

            server.Stop(); // останавливаем сервер
        }
    }
}
