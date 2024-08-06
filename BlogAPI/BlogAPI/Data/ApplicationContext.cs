using System;
using BlogAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogAPI.Data
{
	public class ApplicationContext : IdentityDbContext<User>
	{

		public ApplicationContext(DbContextOptions<ApplicationContext> options) :base(options)
		{
		}
		public DbSet<User>? Users { get; set; }
		public DbSet<Post>? Posts { get; set; }
		public DbSet<Like>? Likes { get; set; }
		public DbSet<Comment>? Comments { get; set; }
        



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Like>().HasKey(a => new { a.UsersId, a.PostsId });
            modelBuilder.Entity<Like>()
                .HasOne(ab => ab.User)
                .WithMany(a => a.Likes)
                .HasForeignKey(ab => ab.UsersId);



                 modelBuilder.Entity<Like>()
                .HasOne(ab => ab.Post)
                .WithMany(b => b.Likes)
                .HasForeignKey(ab => ab.PostsId)
                .OnDelete(DeleteBehavior.NoAction);




                modelBuilder.Entity<Comment>()
                .HasOne(b => b.Post)
                .WithMany().OnDelete(DeleteBehavior.NoAction);

            

        }

    }
}

