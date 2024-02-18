using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace TicketSystemDesktop.Models
{
    public interface IDbEntity
    {
        public long Id { get; }

        public static IDbEntity ParseFromJson(JsonObject parseObject)
        {
            throw new NotImplementedException();
        }

        public static IDbEntity ParseArrayFromJson(string jsonString)
        {
            throw new NotImplementedException();
        }
    }
}
