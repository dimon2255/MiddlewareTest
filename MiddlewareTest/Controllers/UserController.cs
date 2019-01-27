using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiddlewareTest.DataModels;

namespace MiddlewareTest.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {

        [Route("api/user/save")]
        [HttpPost]
        public void SaveUser(User user)
        {

            var x = 9;
        }
       
    }
}
