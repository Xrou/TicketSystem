using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketSystemDesktop
{
    public static class Notifications
    {
        public static event Action TicketsChanged;

        public static void CallTicketsChanged()
            => TicketsChanged?.Invoke();
    }
}
