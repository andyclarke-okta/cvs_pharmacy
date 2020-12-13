using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Http;
using cvs_SCIM_SAML.Connectors;
using cvs_SCIM20.Exceptions;
using cvs_SCIM20.Okta.SCIM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
//using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;

namespace cvs_SCIM_SAML.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IConfiguration _config;
        private static ISCIMConnector _connector;

        public UsersController(ILogger<UsersController> logger, IConfiguration config, ISCIMConnector conn)
        {
            _connector = conn;
            _logger = logger;
            _config = config;
        }


        [HttpGet]
        public IActionResult getUsers([FromQuery] string filter, [RequiredFromQuery] int startIndex, [RequiredFromQuery] int count)
        {
            _logger.LogDebug("Enter getAllUsers by Filter ");
            SCIMFilter myFilter = null;

            if (!string.IsNullOrEmpty(filter))
            {
                myFilter = SCIMFilter.TryParse(filter);
            }

            
            PaginationProperties pp = new PaginationProperties(count, startIndex);
            try
            {
                cvs_SCIM20.Okta.SCIM.Models.SCIMUserQueryResponse rGetUsers = _connector.getUsers(pp, myFilter);
                if (rGetUsers == null)
                {
                    _logger.LogDebug("Exit no users not found");
                    return NotFound();
                }
                else
                {
                    _logger.LogDebug("Exit Successfully user found ");
                    return Ok(rGetUsers);
                }

            }
            catch (EntityNotFoundException e)
            {
                _logger.LogDebug("Exit entity not found trying to get user");
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogDebug("Exit Error at getUser  ");
                _logger.LogError(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }



        //for debug only
        //[HttpGet]
        //public IActionResult getBulkUsers()
        //{
        //    _logger.LogDebug("Enter getBulkUsers ");

        //    SCIMFilter f = new SCIMFilter();
        //    PaginationProperties pp = new PaginationProperties(200, 1);

        //    try
        //    {


        //        Okta.SCIM.Models.SCIMUserQueryResponse rGetUsers = _connector.getUsers(pp, f);
        //        _logger.LogDebug("Exit Successful getBulk Users ");
        //        return Ok(rGetUsers);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogDebug("Exit Error at getBulkUsers ");
        //        _logger.LogError(e.ToString());
        //        return StatusCode(StatusCodes.Status500InternalServerError, e);
        //    }
        //}

        [HttpGet("{id}")]
        public IActionResult getUser(string id)
        {

            SCIMUser scimUserOut = new SCIMUser();

            if (id == null)
            {
                _logger.LogError("Error at getUser by Id, null value");
                return BadRequest();
            }
            else
            {
                _logger.LogDebug("Enter getUser " + id);
            }

            try
            {
                scimUserOut = _connector.getUser(id);
                if (string.IsNullOrEmpty(scimUserOut.id))
                {
                    _logger.LogDebug("Exit Success but user not found");
                    return NotFound();
                }
                else
                {
                    _logger.LogDebug("Exit Successfully found  user  username " + scimUserOut.userName + "  appId " + scimUserOut.id);
                    return Ok(scimUserOut);
                }
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogDebug("Exit entity not found trying to get user");
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogDebug("Exit Error at getUser  ");
                _logger.LogError(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError,e);
            }
        }


        //Add User
        [HttpPost]
        public IActionResult PostUser([FromBody] SCIMUser user)
       {
            _logger.LogDebug("Enter PostUser " + user.userName);
            SCIMUser scimUserOut = new SCIMUser();
            try
            {
                scimUserOut = _connector.createUser(user);
                if (string.IsNullOrEmpty(scimUserOut.id))
                {
                    _logger.LogError("Exit error create user name " + user.userName);
                    SCIMException createException = new SCIMException();
                    createException.ErrorMessage = "error create user name " + user.userName;
                    createException.ErrorSummary = "error create user name " + user.userName;
                    //return InternalServerError(createException);
                    
                    return StatusCode(StatusCodes.Status500InternalServerError, createException);

                }
                else
                {
                    //return Ok();
                    _logger.LogDebug("Exit Successfully created  user  username " + scimUserOut.userName + "  appId " + scimUserOut.id);
                    //string uri = Url.Link("DefaultAPI", new { id = user.id });
                    string uri = "https://default.com";
                    return Created(uri, scimUserOut);
                }

            }
            catch (Exception e)
            {
                _logger.LogDebug("Exit Exception at PostUser ");
                _logger.LogError(e.ToString());

                //return InternalServerError(e);
                return StatusCode(StatusCodes.Status500InternalServerError, e);

            }
        }

        //update user
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] SCIMUser user)
        {

            SCIMUser scimUserOut = new SCIMUser();
            if (id == null)
            {
                _logger.LogError("Error at PUT User, id missing ");
                return BadRequest();
            }
            else
            {
                _logger.LogDebug("Enter Put " + user.displayName + " Id " + id);
            }

            try
            {
                user.id = id;
                scimUserOut = _connector.updateUser(user);
                if (string.IsNullOrEmpty(scimUserOut.id))
                {
                    _logger.LogError("Exit error update user id " + id);
                    SCIMException updateException = new SCIMException();
                    updateException.ErrorMessage = "error update user id " + id;
                    updateException.ErrorSummary = "error update user id " + id;

                    return StatusCode(StatusCodes.Status500InternalServerError, updateException);
                }
                else
                {
                    //return Ok();
                    _logger.LogDebug("Exit Successfully updated  user  username " + scimUserOut.userName + "  appId " + scimUserOut.id);
                    return Ok(scimUserOut);
                }
            }

            catch (Exception e)
            {
                _logger.LogDebug("Exit Error at PUT User");
                _logger.LogError(e.ToString());
               
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        //Okta SCIM interface uses PATCH to delete/disable user
        [HttpPatch("{id}")]
        public IActionResult Patch(String id, [FromBody] SCIMUserOperation operation)
        {
            bool result = true;
            SCIMUser scimUserOut = new SCIMUser();
            if (id == null)
            {
                _logger.LogError("Error at PATCH User, id missing ");
                return BadRequest();
            }
            else
            {
                _logger.LogDebug("Enter Patch  Id " + id);
            }
            try
            {
                if (operation.Operations[0].op == "replace")
                {
                    if (operation.Operations[0].value.active)
                    {
                        scimUserOut = _connector.reactivateUser(id);
                    }
                    else
                    {
                        scimUserOut = _connector.deactivateUser(id);
                    }

                    if (string.IsNullOrEmpty(scimUserOut.id))
                    {
                        _logger.LogDebug("Exit error update user id " + id);
                        SCIMException updateException = new SCIMException();
                        updateException.ErrorMessage = "error update user id " + id;
                        updateException.ErrorSummary = "error update user id " + id;
            
                        return StatusCode(StatusCodes.Status500InternalServerError, updateException);
                    }
                    else
                    {
                        //return Ok();
                        _logger.LogDebug("Exit Successfully updated  user  username " + scimUserOut.userName + "  appId " + scimUserOut.id);
                        //return Ok<SCIMUser>(scimUserOut);
                        return Ok(scimUserOut);
                    }

                }
                else
                {
                    _logger.LogDebug("Exit Patch user failed with unknown operation " + id);
                    SCIMException patchException = new SCIMException();
                    patchException.ErrorMessage = "Patch user failed with unknown operation id " + id;
                    patchException.ErrorSummary = "Patch user failed with unknown operation " + id;
                    
                    return StatusCode(StatusCodes.Status500InternalServerError, patchException);
                }

            }
            catch (Exception e)
            {
                _logger.LogDebug("Exit Error at Patching User Status  ");
                _logger.LogError(e.ToString());
        
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        //Delete is not supported for Okta SCIM interface
        [HttpDelete("{id}")]
        public IActionResult Delete(String id)
        {
            _logger.LogDebug("Enter Delete " + id);


            try
            {
                _connector.deactivateUser(id);
                _logger.LogDebug("Exit delete user id " + id);

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception e)
            {
                _logger.LogDebug("Exit Error at Delete User  ");
                _logger.LogError(e.ToString());

                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }
    }

    public class RequiredFromQueryActionConstraint : IActionConstraint
    {
        private readonly string _parameter;

        public RequiredFromQueryActionConstraint(string parameter)
        {
            _parameter = parameter;
        }

        public int Order => 999;

        public bool Accept(ActionConstraintContext context)
        {
            if (!context.RouteContext.HttpContext.Request.Query.ContainsKey(_parameter))
            {
                return false;
            }

            return true;
        }
    }

    public class RequiredFromQueryAttribute : FromQueryAttribute, IParameterModelConvention
    {
        public void Apply(ParameterModel parameter)
        {
            if (parameter.Action.Selectors != null && parameter.Action.Selectors.Any())
            {
                parameter.Action.Selectors.Last().ActionConstraints.Add(new RequiredFromQueryActionConstraint(parameter.BindingInfo?.BinderModelName ?? parameter.ParameterName));
            }
        }
    }


}
