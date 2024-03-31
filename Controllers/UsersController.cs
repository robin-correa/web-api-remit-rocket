using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_api_remit_rocket.Data;
using web_api_remit_rocket.Dtos.Users;
using web_api_remit_rocket.Helpers;
using web_api_remit_rocket.Models;

namespace web_api_remit_rocket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RemitRocketDbContext _context;

        public UsersController(RemitRocketDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<Paginate<GetUserResourceDto>>> GetUsers(int currentPage = 1, int perPage = 10)
        {
            var query = _context.Users.AsQueryable();

            // Calculate the number of items to skip
            int skip = (currentPage - 1) * perPage;

            // Get total number of items
            int totalItems = await query.CountAsync();

            // Calculate total number of pages
            int totalPages = (int)Math.Ceiling((double)totalItems / perPage);

            // Apply pagination
            var users = await query
                .Skip(skip)
                .Take(perPage)
                .ToListAsync();

            // Generate next page URL
            string? nextPageUrl = currentPage < totalPages ? Url.Action("GetUsers", new { currentPage = currentPage + 1, perPage }) : string.Empty;

            // Generate previous page URL
            string? prevPageUrl = currentPage > 1 ? Url.Action("GetUsers", new { currentPage = currentPage - 1, perPage }) : string.Empty;

            // Create pagination response
            var paginated = new Paginate<GetUserResourceDto>
            {
                currentPage = currentPage,
                perPage = perPage,
                total = totalItems,
                lastPage = totalPages,
                data = users.Select(toArrayResource).ToList(),
                nextPageUrl = nextPageUrl,
                previousPageUrl = prevPageUrl,
                firstPageUrl = Url.Action("GetUsers", new { currentPage = 1, perPage }) ?? string.Empty,
                lastPageUrl = Url.Action("GetUsers", new { currentPage = totalPages, perPage }) ?? string.Empty,
                path = Url.Action("GetUsers", new { currentPage, perPage }) ?? string.Empty,
                from = skip + 1,
                to = skip + users.Count
            };

            return paginated;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserResourceDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return this.toArrayResource(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GetUserResourceDto>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, this.toArrayResource(user));
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private GetUserResourceDto toArrayResource(User user) {
            return new GetUserResourceDto
            {
                Id = user.Id,
                Email = user.Email,
                LastName = user.LastName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                Gender = user.Gender,
                BirthDate = user.BirthDate.ToString("yyyy-MM-dd"),
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };
        }
    }
}
