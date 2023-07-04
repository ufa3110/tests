using Microsoft.AspNetCore.Mvc;
using TrueCodeTestSolution.Publications;

namespace TrueCodeTestSolution.Controllers
{
    [ApiController]
    [Route("publications")]
    public class PublicationController : ControllerBase
    {

        [HttpGet("last_{number}/{userId}")]
        public IEnumerable<Publication> GetLastNByUserId(int number, string userId)
        {
            return PublicationsContext.GetAllPosts()
                                      .Where(p => p.AuthorID == userId)
                                      .OrderBy(p => p.PublicationDate)
                                      .Take(number)
                                      .ToList();
        }

        [HttpGet("last_{number}/user_count_{userCount}")]
        public IEnumerable<Publication> GetLastN(int number, int userCount)
        {
            //TODO: возвращать View.
            return PublicationsContext.GetAllPosts()
                                      .OrderBy(p => p.PublicationDate)
                                      .GroupBy(p => p.AuthorID)
                                      .Take(userCount)
                                      .SelectMany(g => g.OrderBy(p => p.PublicationDate).Take(number))
                                      .ToList();
        }
    }
}