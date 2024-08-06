using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Text.Json.Serialization;

namespace BlogAPI.Models
{
	public class Comment
	{
        [Key]
        public long Id { get; set; }

        public long PostId { get; set; }

        public long? CommentId { get; set; }

        [JsonIgnore]
        public string UserId { get; set; } = "";

        [StringLength(2000)]
        public string UserComment { get; set; } = "";

        [ForeignKey(nameof(PostId))]
        [JsonIgnore]
        public Post? Post { get; set; }

        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public User? User { get; set; }

    }
}

