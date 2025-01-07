﻿using Newtonsoft.Json;

namespace ACardGameServer
{
    public class ServerResponse
    {
        public StatusCode StatusCode { get; set; }
        public string Data { get; set; }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class UserAuthenticatedResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
