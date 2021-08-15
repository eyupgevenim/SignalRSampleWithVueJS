namespace User.API.Models
{
    using Newtonsoft.Json;
    using System;

    public class TokenModel
    {
        public string AccessToken { get; set; }

        public DateTime Expires { get; set; }

        public string UserId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
    }
}
