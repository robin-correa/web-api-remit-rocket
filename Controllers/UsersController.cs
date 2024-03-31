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
        private const int DefaultPage = 1;
        private const int DefaultPerPage = 10;
        private const string BirthDateFormat = "yyyy-MM-dd";
        private readonly RemitRocketDbContext _context;

        public UsersController(RemitRocketDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<PaginationDto<GetUserResourceDto>>> GetUsers(int currentPage = DefaultPage, int perPage = DefaultPerPage)
        {
            var paginated = await GetPaginatedData(currentPage, perPage);
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

            return ToArrayResource(user);
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

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, ToArrayResource(user));
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

        private GetUserResourceDto ToArrayResource(User user)
        {
            return new GetUserResourceDto
            {
                Id = user.Id,
                Email = user.Email,
                LastName = user.LastName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                Gender = user.Gender,
                BirthDate = user.BirthDate.ToString(format: BirthDateFormat),
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };
        }

        private async Task<PaginationDto<GetUserResourceDto>> GetPaginatedData(int currentPage, int perPage)
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
            string? nextPageUrl = currentPage < totalPages ? Url.Action(nameof(GetUsers), new { currentPage = currentPage + 1, perPage }) : null;

            // Generate previous page URL
            string? prevPageUrl = currentPage > 1 ? Url.Action(nameof(GetUsers), new { currentPage = currentPage - 1, perPage }) : null;

            // Create pagination response
            var paginated = new PaginationDto<GetUserResourceDto>
            {
                currentPage = currentPage,
                perPage = perPage,
                total = totalItems,
                lastPage = totalPages,
                data = users.Select(ToArrayResource).ToList(),
                nextPageUrl = nextPageUrl,
                previousPageUrl = prevPageUrl,
                firstPageUrl = Url.Action(nameof(GetUsers), new { currentPage = 1, perPage }) ?? null,
                lastPageUrl = Url.Action(nameof(GetUsers), new { currentPage = totalPages, perPage }) ?? null,
                path = Url.Action(nameof(GetUsers)) ?? null,
                from = currentPage <= totalPages ? skip + 1 : null,
                to = currentPage <= totalPages ? Math.Min(skip + users.Count, totalItems) : null
            };

            return paginated;
        }
    }
}
