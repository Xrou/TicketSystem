using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
    public static class CallbackListener
    {
        public static async void Listen()
        {
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
                    string codeRequst = url.Query.Split("=")[1];

                    if (int.TryParse(codeRequst, out code))
                    {
                        if (StaticStorage.Codes.ContainsKey(code))
                        {
                            response.StatusCode = (int)HttpStatusCode.OK;
                            responseText = StaticStorage.Codes[code].ToString(); // telegram id
                            StaticStorage.Codes.Remove(code);
                        }
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
