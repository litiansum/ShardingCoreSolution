using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.EF;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly DemoDBContext _context;

        public TestController(ILogger<TestController> logger, DemoDBContext context)
        {
            _logger = logger;
            _context = context;
        }


        [HttpGet]
        public async Task<object?> Get(string referenceId)
        {
            var queryable = from references in _context.FileReferences
                            join files in _context.FileInfos on references.FileInfoId equals files.Id
                            where references.ReferenceId == referenceId
                            select new
                            {
                                references.ReferenceId,
                                Id = references.Id,
                                references.CreateTime,
                                references.LastUpdateTime,
                                FileInfo = files
                            };

            return await queryable.AsNoTracking().FirstOrDefaultAsync();
        }
    }
}
