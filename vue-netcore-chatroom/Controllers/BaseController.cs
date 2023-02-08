using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace vue_netcore_chatroom.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        private readonly ILogger<BaseController> _logger;
        public IConfiguration _configuration;


        public BaseController(ILogger<BaseController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
    }
}

