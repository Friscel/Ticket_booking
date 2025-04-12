using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketBookingCommon
{
    public class Movie
    {
        public string Title { get; set; }
        public int AvailableTickets { get; set; }
        public int BookedTickets { get; set; }
    }
}
