using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Models
{
	public class User : IdentityUser
	{
		public string NickName { get; set; } = "";
		public string FirstName { get; set; } = "";
		public string? MiddleName { get; set; } = "";
		public string FamilyName { get; set; } = "";
		public string City { get; set; } = "";
		public string Country { get; set; } = "";
        public DateTime BirthDate { get; set; }
		public DateTime RegisterDate { get; set; }
		public bool Gender { get; set; }


		[StringLength(5000)]
		public string? Bio { get; set; } = "";
        [NotMapped]
        public string? Password { get; set; }
        [NotMapped]
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }

		[JsonIgnore]
		public List<Post>? Posts { get; set; }

		[JsonIgnore]
		public List<Like>? Likes { get; set; }


    }
}

