using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using cvs_SCIM20.Okta.SCIM.Models;
using cvs_SCIM_SAML.Connectors;


using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

//using Okta.AspNetCore;
//using okta_aspnetcore_mvc_example.Models;


using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.MvcCore;

using Microsoft.AspNetCore.Authorization;

using cvs_SCIM_SAML.Services;
using Microsoft.Extensions.Options;
using System.Security.Authentication;
using cvs_SCIM_SAML.Models;

namespace cvs_SCIM_SAML.Controllers
{
    public class HomeController : Controller
    {


        private readonly ILogger<HomeController> _logger;
        private static ISCIMConnector _connector;
        private readonly IConfiguration _config;
        public List<SCIMUser> _scimUsers = null;
        public List<SCIMGroup> _scimGroups = null;

        //private readonly string _redirectUrl;
        private readonly string _goToPortalUrl;

        const string relayStateReturnUrl = "ReturnUrl";
        private readonly Saml2Configuration _samlConfig;

        public HomeController(ILogger<HomeController> logger, IConfiguration config, ISCIMConnector conn, IOptions<Saml2Configuration> configAccessor)
        {
            _connector = conn;
            _logger = logger;
            _config = config;
            _scimUsers = new List<SCIMUser>();
            _scimGroups = new List<SCIMGroup>();

            _samlConfig = configAccessor.Value;
        }



        [Route("")]
        [Route("Home")]
        [Route("Home/Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("Home/About")]
        public IActionResult About()
        {
            return View();
        }

        [Route("Home/Login")]
        public IActionResult Login(string returnUrl = null)
        {
            var binding = new Saml2RedirectBinding();
            binding.SetRelayStateQuery(new Dictionary<string, string> { { relayStateReturnUrl, returnUrl ?? Url.Content("~/") } });

            return binding.Bind(new Saml2AuthnRequest(_samlConfig)).ToActionResult();
        }

        [Route("Home/PortalLogin")]
        public ActionResult PortalLogin()
        {
            return Redirect(_goToPortalUrl);
            //return Redirect(_redirectUrl);
        }

        [Route("Home/Logout")]
        public async Task<IActionResult> Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect(Url.Content("~/"));
            }

            var binding = new Saml2PostBinding();
            var saml2LogoutRequest = await new Saml2LogoutRequest(_samlConfig, User).DeleteSession(HttpContext);
            return Redirect("~/");
        }

        [Route("Home/PostLogin")]
        public ActionResult PostLogin()
        {

            return View();
        }

        [Route("Home/PostLogOut")]
        public ActionResult PostLogOut()
        {

            return View();
        }

        [Route("Home/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        [Route("Home/Profile")]
        public IActionResult Profile()
        {
            return View(HttpContext.User.Claims);
        }

        [Route("Home/DisplayUsers")]
        public IActionResult DisplayUsers()
        {
            SCIMFilter myFilter = null;
            PaginationProperties pp = new PaginationProperties(200, 1);
            cvs_SCIM20.Okta.SCIM.Models.SCIMUserQueryResponse rGetUsers = _connector.getUsers(pp, myFilter);
            _scimUsers = rGetUsers.Resources;

            return View(_scimUsers);
        }


        [Route("Home/DisplayGroups")]
        public IActionResult DisplayGroups()
        {

            PaginationProperties pp = new PaginationProperties(200, 1);
            cvs_SCIM20.Okta.SCIM.Models.SCIMGroupQueryResponse rGetGroups = _connector.getGroups(pp);
            _scimGroups = rGetGroups.Resources;
            return View(_scimGroups);
        }

    }
}
