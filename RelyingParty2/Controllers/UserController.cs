namespace RelyingParty2.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Web.Mvc;

    using ViewModels;

    public class UserController : Controller
    {
        [Route("~/")]
        [Route("user/get")]
        [HttpGet]
        public ViewResult Get()
        {
            return
                View(
                    new UserViewModel
                        {
                            UserName = ClaimsPrincipal.Current.Claims.First(c => c.Type == ClaimTypes.Name)
                                .Value,
                            Claims = ClaimsPrincipal.Current.Claims
                        });
        }
    }
}