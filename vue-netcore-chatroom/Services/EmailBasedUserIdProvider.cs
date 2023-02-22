using System;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace vue_netcore_chatroom.Services
{
	public class EmailBasedUserIdProvider : IUserIdProvider
	{
        public virtual string GetUserId(HubConnectionContext connection)
        {
            string email;

            try
            {
                email = connection.User.FindFirstValue(ClaimTypes.Email);
            }
            catch
            {
                email = "";
            }

            return email;
        }
    }
}

