using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketSystemDesktop
{
    public static class LocalStorage
    {
        private static Dictionary<string, object> Storage { get; set; } = new Dictionary<string, object>();

        public static object? Get(string key)
        {
            if (Storage.ContainsKey(key))
                return Storage[key];
            return null;
        }

        public static void Set(string key, string value)
        {
            if (Storage.ContainsKey(key))
                Storage[key] = value;

            Storage.Add(key, value);
        }
    }
}
