using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Data;
using BlogAPI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public LikesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Likes
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Like>>> GetLikes()
        {
          if (_context.Likes == null)
          {
              return NotFound();
          }
            return await _context.Likes.ToListAsync();
        }

        // GET: api/Likes/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Like>> GetLike(string id)
        {
          if (_context.Likes == null)
          {
              return NotFound();
          }
            var like = await _context.Likes.FindAsync(id);

            if (like == null)
            {
                return NotFound();
            }

            return like;
        }

        // PUT: api/Likes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLike(string id, Like like)
        {
            if (id != like.UsersId)
            {
                return BadRequest();
            }

            _context.Entry(like).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LikeExists(id))
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

        // POST: api/Likes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Like>> PostLike(Like like)
        {

            Post post = _context.Posts.FirstOrDefault(b => b.Id == like.PostsId);
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_context.Likes == null)
          {
              return Problem("Entity set 'ApplicationContext.Likes'  is null.");
          }

            post.LikeCount += 1;
            like.UsersId = userId;

            _context.Likes.Add(like);

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LikeExists(userId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            } 

            return CreatedAtAction("GetLike", new { id = like.UsersId }, like);
        }

        // DELETE: api/Likes/5
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteLike(string userId, long postId)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_context.Likes == null)
            {
                return NotFound();
            }
            var like = await _context.Likes.FindAsync(userId,postId);
            if (like.UsersId != _userId)
            {
                return Unauthorized("Wrong Like Id");
            }

            if (like == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(postId);

            _context.Likes.Remove(like);

            post.LikeCount -= 1;
            _context.Posts.Update(post);


            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LikeExists(string id)
        {
            return (_context.Likes?.Any(e => e.UsersId == id)).GetValueOrDefault();
        }
    }
}
