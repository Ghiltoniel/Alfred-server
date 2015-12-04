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
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var headerValues = actionContext.Request.Headers.GetValues("token");
            var token = headerValues.FirstOrDefault();
            if (!string.IsNullOrEmpty(token))
            {
                var verifiedToken = UserManager.ValidateToken(token);
                return string.IsNullOrEmpty(verifiedToken);
            }
            return base.IsAuthorized(actionContext);
        }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }
    }
}
