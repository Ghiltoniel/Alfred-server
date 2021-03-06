﻿using Alfred.Model.Core;
using Alfred.Server.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Alfred.Server.WebApi.Controllers
{
    [TokenAuthorize]
    public class AlfredController : ApiController
    {
        [HttpPost]
        [Route("alfred/launchcommands")]
        public void LaunchCommands(IEnumerable<AlfredTask> tasks)
        {
            foreach(var task in tasks)
            {
                Launcher.EnqueueTask(task);
            }
        }

        [HttpPost]
        [Route("alfred/task")]
        public void Task(AlfredTask task)
        {
            Launcher.EnqueueTask(task);
        }
    }
}
