using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core3.Data;
using Core3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core3.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private ILogger<ValuesController> _logger;
        private IBlogRepository _repo;

        public ValuesController(IBlogRepository repo, ILogger<ValuesController> logger)
        {
            _logger = logger;
            _repo = repo;
        }


        // GET api/values/5
        [HttpGet]
        public IActionResult Get()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();
            _logger.LogError(JsonConvert.SerializeObject(configuration));
            var blogs = _repo.GetAllBlogs();

            return Ok(blogs);

        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]BlogModel model)
        {
            try
            {
               
                var blog = new Blog
                {
                    Url = model.Url,
                    BlogId = model.BlogId
                };
                
                _repo.Add(blog);
                if (await _repo.SaveAllAsync())
                {
                    return Ok("OK!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception when trying to post {ex}");
                _logger.LogError($"Exception when trying to post {ex}");
                return BadRequest($"Exception when trying to post {ex}");
            }
            return BadRequest("Somthings wrong I can feel it");

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
