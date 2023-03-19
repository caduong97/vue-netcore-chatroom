using System;
namespace vue_netcore_chatroom.Hubs
{
    public class ConnectionMapping
    {
        public string ConnectionId { get; set; }

        public string Email { get; set; }

        public ConnectionMapping(string connectionId, string email)
        {
            ConnectionId = connectionId;
            Email = email;
        }
    }

}

