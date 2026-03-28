using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace OpenApiGuard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        // GET: api/projects
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetProjects()
        {
            return new string[] { "Project1", "Project2" };
        }

        // Other action methods
    }
}