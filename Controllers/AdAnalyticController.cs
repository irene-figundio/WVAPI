using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AI_Integration.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdAnalyticController : ControllerBase
    {

        private readonly IWebHostEnvironment _environment;
        private readonly IUnitOfWork _unitOfWork;

        public AdAnalyticController(IWebHostEnvironment environment, IUnitOfWork unitOfWork)
        {
            _environment = environment;
            _unitOfWork = unitOfWork;
        }
        // GET: api/<AdAnalyticController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AdAnalyticController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AdAnalyticController>
        [HttpPost("create")]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AdAnalyticController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AdAnalyticController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
