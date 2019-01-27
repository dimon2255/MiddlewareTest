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
