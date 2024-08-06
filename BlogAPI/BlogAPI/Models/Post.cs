using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlogAPI.Models
{
	public class Post
	{
		public long Id { get; set; }
		public string Content { get; set; } = "";
		public DateTime CreatedTime { get; set; }

		public long LikeCount { get; set; }

		[JsonIgnore]
		public string UserId { get; set; } = "";

		[JsonIgnore]
		public List<Comment>? Comments { get; set; }

		[JsonIgnore]
        public List<Like>? Likes { get; set; }

		[JsonIgnore]
        [ForeignKey(nameof(UserId))]
		public User? User { get; set; }

	}
}

