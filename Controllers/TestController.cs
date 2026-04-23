using BookingSystem.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Controllers
{
    public class TestController : Controller
    {
        private readonly DapperContext _context;

        public TestController(DapperContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            using var connection = _context.CreateConnection();
            var result = connection.QuerySingle<int>("SELECT 1");
            return Content($"DB Connected: {result}");
        }
    }
}

