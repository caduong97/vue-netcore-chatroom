using System;
using Azure.Core;

namespace vue_netcore_chatroom.Models
{
    public class AuthenticationResponse
    {
        public string AccessToken { get; set; }

        public AuthenticationResponse(string accessToken)
        {
            AccessToken = accessToken;
        }

    }
}
