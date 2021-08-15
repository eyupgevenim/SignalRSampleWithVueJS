namespace User.API.Data.Entities.Chat
{
    using System;
    using global::User.API.Data.Entities.Identity;

    public class Message : BaseEntity
    {
        public string FromUserId { get; set; }
        public User FromUser{ get; set; }

        public string ToUserId { get; set; }
        public User ToUser { get; set; }

        public string Content { get; set; }
        public bool IsOpened { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
