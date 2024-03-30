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
        public async Task<ActionResult<Paginate<GetUserDto>>> GetUsers(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Users.AsQueryable();

            // Calculate the number of items to skip
            int skip = (pageNumber - 1) * pageSize;

            // Get total number of items
            int totalItems = await query.CountAsync();

            // Apply pagination
            var users = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            // Generate next page link
            string? nextPageLink = (pageNumber * pageSize) < totalItems ? Url.Action("GetUsers", new { pageNumber = pageNumber + 1, pageSize }) : string.Empty;

            // Create pagination response
            return new Paginate<GetUserDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                Data = users.Select(toArrayResource).ToList(),
                NextPageLink = nextPageLink
            };
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDto>> GetUser(int id)
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
        public async Task<ActionResult<GetUserDto>> PostUser(User user)
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

        private GetUserDto toArrayResource(User user) {
            return new GetUserDto
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
