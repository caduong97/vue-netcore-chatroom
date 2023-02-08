using System;
using System.ComponentModel.DataAnnotations;

namespace vue_netcore_chatroom.Models
{
    public class AuthenticationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
