using Alfred.Utils.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Alfred.Server.Attributes
{
    public class TokenAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            IEnumerable<string> headerValues;
            var headerExists = actionContext.Request.Headers.TryGetValues("token", out headerValues);

            if (!headerExists)
            {
                HandleUnauthorizedRequest(actionContext);
                return;
            }

            var token = headerValues.FirstOrDefault();
            if (!string.IsNullOrEmpty(token))
            {
                var verifiedToken = UserManager.ValidateToken(token);
                if (string.IsNullOrEmpty(verifiedToken))
                {
                    HandleUnauthorizedRequest(actionContext);
                    return;
                }
            }
        }
    }
}
