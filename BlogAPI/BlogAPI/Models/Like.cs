using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlogAPI.Models
{
	public class Like
	{

        [JsonIgnore]
        public string? UsersId { get; set; } = "";

        public long PostsId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UsersId))]
        public User? User { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(PostsId))]
        public Post? Post { get; set; }

    }
}

