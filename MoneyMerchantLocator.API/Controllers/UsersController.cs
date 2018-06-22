using MoneyMerchantLocator.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MoneyMerchantLocator.API.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : BaseController
    {
        public class LoginModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }

        }

        public class ChangePasswordModel
        {
            [Required]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(265, MinimumLength = 3)]
            public string NewPassword { get; set; }

        }


        public class UpdateUserModel
        {
            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }
        }

        [Route("login")]
        [HttpPost]
        public IHttpActionResult Login([FromBody]LoginModel model)
        {
            var user = DB.Users.FirstOrDefault(x => x.Username == model.Username);
            if (user != null && Helpers.PasswordHelpers.AreEqual(user.PasswordSalt, user.PasswordHash, model.Password))
            {
                return Ok(new UserDto(user));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, "Invalid username or password"));
        }

        [Route("myself")]
        [Authorize]
        public UserDto GetMyself()
        {
            return new UserDto(CurrentUser);
        }

        [Authorize]
        public IHttpActionResult Put(int id, [FromBody]UpdateUserModel model)
        {
            var user = DB.Users.Find(id);
            if (user == null)
                return NotFound();

            if (user.FirstName != model.FirstName)
                user.FirstName = model.FirstName;

            if (user.LastName != model.LastName)
                user.LastName = model.LastName;

            DB.SaveChanges();

            return Ok(user);
        }

        [Route("register")]
        [HttpPost]
        public IHttpActionResult Register([FromBody]RegisterUserModel model)
        {
            if (DB.Users.Any(x => x.Username == model.Username))
            {
                return BadRequest("Username already used");
            }

            string salt;
            string pwdHash = Helpers.PasswordHelpers.Generate(model.Password, out salt);

            var user = new User()
            {
                DateRegistered = DateTime.UtcNow,
                FirstName = model.FirstName,
                LastName = model.LastName,
                AccountType = model.AccountType,
                Username = model.Username,
                PasswordHash = pwdHash,
                PasswordSalt = salt
            };

            DB.Users.Add(user);
            DB.SaveChanges();

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, user));
        }

        [Authorize]
        [Route("change/password")]
        [HttpPost]
        public IHttpActionResult ChangePassword(ChangePasswordModel model)
        {
            var user = DB.Users.Find(CurrentUser.Id);

            if (!Helpers.PasswordHelpers.AreEqual(user.PasswordSalt, user.PasswordHash, model.OldPassword))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            string salt;
            string hash = Helpers.PasswordHelpers.Generate(model.NewPassword, out salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            DB.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

    }
}
