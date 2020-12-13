
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

namespace cvs_SCIM_SAML.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {

        private readonly ILogger<GroupsController> _logger;

        private readonly IConfiguration _config;


        // this connector will be initilized via the IOC container.  Passed to the controller constructor.
        // see SimpleInjectorWebAppInitializer.cs
        private static ISCIMConnector _connector;

        public GroupsController(ILogger<GroupsController> logger, IConfiguration config, ISCIMConnector conn)
        {
            _connector = conn;
            _logger = logger;
            _config = config;
        }


        [HttpGet]
        public IActionResult getAllGroups(int startIndex, int count)
        {
            _logger.LogDebug("getAllGroups ");
            try
            {
                PaginationProperties pp = new PaginationProperties(count, startIndex);
                return Ok(_connector.getGroups(pp));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        [HttpGet("{id}")]
        public IActionResult getGroup(string id)
        {
            _logger.LogDebug("getGroup " + id);
            try
            {
                return Ok(_connector.getGroup(id));
            }
            catch (EntityNotFoundException e)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        //Add Group
        [HttpPost]
        public IActionResult PostGroup([FromBody] SCIMGroup group)
        {
            _logger.LogDebug("PostGroup " + group.displayName);
            try
            {
                group = _connector.createGroup(group);
                string uri = "https://default.com";
                return Created(uri, group);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        //Update Group
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] SCIMGroup group)
        {

            SCIMGroup scimGroupOut = new SCIMGroup();
            if (id == null)
            {
                _logger.LogError("Error at PUT Group, id missing ");
                return BadRequest();
            }
            else
            {
                _logger.LogDebug("Enter Put " + group.displayName + " Id " + id);
            }

            try
            {
                group.id = id;
                scimGroupOut = _connector.updateGroup(group);
                if (string.IsNullOrEmpty(scimGroupOut.id))
                {
                    _logger.LogError("Exit error update group id " + id);
                    SCIMException updateException = new SCIMException();
                    updateException.ErrorMessage = "error update group id " + id;
                    updateException.ErrorSummary = "error update group id " + id;
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                else
                {
                    //return Ok();
                    _logger.LogDebug("Exit Successfully updated  group " + scimGroupOut.displayName);
                    return Ok(scimGroupOut);
                }
            }

            catch (Exception e)
            {
                _logger.LogDebug("Exit Error at PUT User");
                _logger.LogError(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }

        }

        //Add / Remove member from group
        [HttpPatch("{id}")]
        public IActionResult Patch(String id, [FromBody] SCIMGroupOperation operation)
        {
            bool result = false;
            bool response = false;


            if (id == null)
            {
                _logger.LogError("Error at PATCH Group, id missing ");
                return BadRequest();
            }
            else
            {
                _logger.LogDebug("Enter Patch  Id " + id);
            }
            try
            {
                foreach (var nextOp in operation.Operations)
                {

                    switch (nextOp.op)
                    {
                        case "add":
                            Member addMember = new Member();
                            response = false;
                            addMember.value = nextOp.value[0].value;
                            addMember.display = nextOp.value[0].display;

                            response = _connector.addGroupMember(id, addMember);
                            if (response)
                            {
                                result = true;
                            }

                            break;
                        case "remove":
                            Member removeMember = new Member();
                            response = false;

                            string path = nextOp.path;
                            var index = path.IndexOf("eq");
                            var path1 = path.Substring(index + 2);
                            var text = Regex.Replace(path1, "[^\\w\\._]", "");
                            removeMember.value = text;
                            response = _connector.removeGroupMember(id, removeMember);
                            if (response)
                            {
                                result = true;
                            }

                            break;
                        case "replace":
                            Member replaceMember = new Member();
                            response = false;
                            response = _connector.removeGroupMember(id, replaceMember);
                            if (response)
                            {
                                response = _connector.addGroupMember(id, replaceMember);
                                if (response)
                                {
                                    result = true;
                                }
                            }

                            break;
                        default:
                            _logger.LogDebug("Exit Patch group failed with unknown operation id" + id);
                            SCIMException patchException = new SCIMException();
                            patchException.ErrorMessage = "Patch group failed with unknown operation id " + id;
                            patchException.ErrorSummary = "Patch group failed with unknown operation id" + id;
                            return StatusCode(StatusCodes.Status500InternalServerError, patchException);
                    }

                }//end foreach
                if (result)
                {
                    _logger.LogDebug("Exit Successfully Patch group id " + id);
                    //both a 204 and 200 with full object are legal
                    return Ok(_connector.getGroup(id));
                    //return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    _logger.LogDebug("Exit Patch group failed id " + id);
                    SCIMException patchException = new SCIMException();
                    patchException.ErrorMessage = "Patch group failed  id " + id;
                    patchException.ErrorSummary = "Patch group failed id " + id;
                    return StatusCode(StatusCodes.Status500InternalServerError, patchException);
                }

            }//end try
            catch (Exception e)
            {
                _logger.LogDebug("Exit Error at Patching Group  ");
                _logger.LogError(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }


        //Delete Group
        [HttpDelete("{id}")]
        public IActionResult Delete(String id)
        {
            _logger.LogDebug("Delete id " + id);
            try
            {
                SCIMGroup group = _connector.getGroup(id);
                if (group == null)
                {
                    return NotFound();
                }

                _connector.deleteGroup(id);
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

    }
}
