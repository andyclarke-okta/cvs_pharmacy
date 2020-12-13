using cvs_SCIM20.Okta.SCIM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cvs_SCIM20.Exceptions
{
    // SCIM Exceptions
    public class QueryFilterNotImplemented : SCIMException
    {
        public QueryFilterNotImplemented()
        {
        }

        public QueryFilterNotImplemented(string message)
            : base(message)
        {
        }

        public QueryFilterNotImplemented(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    public class WebAPINotSuccess : SCIMException
    {
        public WebAPINotSuccess()
        {
        }

        public WebAPINotSuccess(string message)
            : base(message)
        {
        }

        public WebAPINotSuccess(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    public class DuplicateGroupException : SCIMException
    {
        public DuplicateGroupException()
        {
        }

        public DuplicateGroupException(string message)
            : base(message)
        {
        }

        public DuplicateGroupException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    public class EntityNotFoundException : SCIMException
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    public class OnPremUserManagementException : SCIMException
    {
        public OnPremUserManagementException()
        {
        }

        public OnPremUserManagementException(string message)
            : base(message)
        {
        }

        public OnPremUserManagementException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
