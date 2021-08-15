using System;

namespace User.API.Models
{
    public class MessageModel
    {
        public int MessageId { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string FormatedDate { get; set; }
    }
}
