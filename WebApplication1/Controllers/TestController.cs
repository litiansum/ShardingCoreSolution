using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.EF;
using WebApplication1.Entities;

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


        [HttpPost]
        public async Task<IEnumerable<FileReference>> Get(IEnumerable<string> referenceIds)
        {
            return await _context.FileReferences.Where(i => referenceIds.Contains(i.ReferenceId)).AsNoTracking().ToListAsync();
        }
    }
}
