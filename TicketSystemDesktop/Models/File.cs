using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace TicketSystemDesktop.Models
{
    public class File : IDbEntity
    {
        public long Id { get; set; }
        public string Path { get; set; }

        public static File ParseFromJson(JsonObject parseObject)
        {
            File file = new File();

            file.Id = parseObject["id"]!.GetValue<long>();
            file.Path = parseObject["path"]!.GetValue<string>();

            return file;
        }

        public static List<File> ParseArrayFromJson(string response)
        {
            List<File> parsed = new List<File>();
            var array = JsonArray.Parse(response);

            foreach (var t in array.AsArray())
            {
                var obj = t.AsObject();
                parsed.Add(ParseFromJson(obj));
            }

            return parsed;
        }
    }
}
