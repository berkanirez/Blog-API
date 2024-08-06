using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.Metrics;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UsersController(ApplicationContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/Users
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user, string? currentPassword = null)
        {

            if (id != user.Id)
            {
                return BadRequest();
            }

            // Kullanıcıyı mevcut DbContext içinde izlenip izlenmediğini kontrol et
            var existingUser = _userManager.FindByIdAsync(id).Result;
            if (existingUser != null)
            {
                // Mevcut kullanıcıyı izlemeyi bırak
                _context.Entry(existingUser).State = EntityState.Detached;
            }

            existingUser.UserName = user!.UserName;
            existingUser.NickName = user!.NickName;
            existingUser.FirstName = user!.FirstName;
            existingUser.MiddleName = user!.MiddleName;
            existingUser.FamilyName = user!.FamilyName;
            existingUser.City = user!.City;
            existingUser.Country = user!.Country;
            existingUser.Gender = user!.Gender;
            existingUser.BirthDate = user!.BirthDate;
            existingUser.RegisterDate = user!.RegisterDate;
            existingUser.Bio = user!.Bio;
            existingUser.Password = user!.Password;
            existingUser.ConfirmPassword = user!.ConfirmPassword;

            // Güncellenmiş kullanıcıyı ekle veya güncelle
            _userManager.UpdateAsync(existingUser).Wait();

            /* try
            {
                // Veritabanına değişiklikleri kaydet
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
            } */

            // Şifre değişikliği varsa işlemi yap
            if (currentPassword != null)
            {
                _userManager.ChangePasswordAsync(existingUser, currentPassword, existingUser.Password).Wait();
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
          if (_context.Users == null)
          {
              return Problem("Entity set 'ApplicationContext.Users'  is null.");
          }
            _userManager.CreateAsync(user!, user!.Password).Wait();


            /* _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            } */

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("Login")]
        public ActionResult Login(string userName, string password)
        {
            User user = _userManager.FindByNameAsync(userName).Result;
            Microsoft.AspNetCore.Identity.SignInResult signInResult;

            if (user != null)
            {
                signInResult = _signInManager.PasswordSignInAsync(user, password, false, false).Result;
                if (signInResult.Succeeded == true)
                {
                    return Ok();
                }
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpGet("Logout")]
        public ActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost("ForgetPassword")]
        public ActionResult<string> ForgetPassword(string userName)
        {
            User user = _userManager.FindByNameAsync(userName).Result;

            string token = _userManager.GeneratePasswordResetTokenAsync(user).Result;
            return token;
        }

        [HttpPost("ResetPassword")]
        public ActionResult ResetPassword(string userName, string token, string newPassword)
        {
            User user = _userManager.FindByNameAsync(userName).Result;
            _userManager.ResetPasswordAsync(user, token, newPassword).Wait();

            return Ok();
        }

        private bool UserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
