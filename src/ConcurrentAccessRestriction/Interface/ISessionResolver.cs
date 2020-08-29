using ConcurrentAccessRestriction.Storage;
using ConcurrentAccessRestriction.Storage.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Interface
{
    public interface ISessionResolver
    {
        Session CurrentSession(HttpContext httpContext);
    }
}
