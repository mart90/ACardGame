﻿namespace ACardGameLibrary
{
    public class MessageToPlayerParams
    {
        public string Message { get; set; }
        public MessageSeverity Severity { get; set; }
        public bool ActivePlayerOnly { get; set; }
    }
}