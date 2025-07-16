using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    public class Group
    {
        public string TransactionId { get; set; }
        public string Content { get; set; }

        public Group(String transactionId, String content)
        {
            this.TransactionId = transactionId;
            this.Content = content;
        }

    }
}
