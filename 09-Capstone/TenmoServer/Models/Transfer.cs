using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public string type { get; set; }
        public string status { get; set; }
        public int transferId { get; set; }
        public int sender { get; set; }
        public int reciever { get; set; }
        public decimal balance { get; set; }
        public string senderName { get; set; }
        public string recieverName { get; set; }
    }
}
