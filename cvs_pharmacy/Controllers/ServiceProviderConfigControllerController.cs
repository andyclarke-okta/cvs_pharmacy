using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cvs_SCIM_SAML.Connectors;
using cvs_SCIM20.Exceptions;
using cvs_SCIM20.Okta.SCIM.Models;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace cvs_SCIM_SAML.Controllers
{
    //this is for SCIM 2.0
    //note there in NOT an 's' on Config
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderConfigControllerController : ControllerBase
    {
        private readonly ILogger<ServiceProviderConfigControllerController> _logger;
        private readonly IConfiguration _config;
        private static ISCIMConnector _connector;

        public ServiceProviderConfigControllerController(ILogger<ServiceProviderConfigControllerController> logger, IConfiguration config, ISCIMConnector conn)
        {
            _connector = conn;
            _logger = logger;
            _config = config;
        }

        public IActionResult getAll()
        {
            _logger.LogDebug(" enter getAll serviceproviderConfig");
            return Ok(_connector.getServiceProviderConfig());
        }

    }
}
