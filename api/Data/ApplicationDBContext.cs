using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using api.Migrations;
using Microsoft.AspNetCore.Identity;

namespace api.Data
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }
        public DbSet<Models.Community> Communities {get; set;} = null!;
        public DbSet<Models.Tag> Tags {get; set; } = null!;
        public DbSet<Author> Authors {get; set; } = null!;
        public DbSet<Post> Posts {get; set; } = null!;
        public DbSet<PostTag> PostTags {get; set; } = null!;
        public DbSet<Like> Likes {get; set; } = null!;
        public DbSet<CommunityUser> CommunityUsers {get; set; } = null!;
        public DbSet<Comment> Comments {get; set; } = null!;
        public DbSet<UserToken> Tokens {get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                },
            };
            builder.Entity<IdentityRole>().HasData(roles);
            builder.Entity<Author>()
                .HasKey(a => a.UserId);

            builder.Entity<Author>()
                .HasOne(a => a.User)
                .WithOne(u => u.Author)
                .HasForeignKey<Author>(a => a.UserId);

            builder.Entity<PostTag>(x => x.HasKey(p => new {p.PostId, p.TagId}));
            
            builder.Entity<PostTag>()
                .HasOne(u => u.Post)
                .WithMany(u => u.PostTags)
                .HasForeignKey(p => p.PostId);

            builder.Entity<PostTag>()
                .HasOne(u => u.Tag)
                .WithMany(u => u.PostTags)
                .HasForeignKey(p => p.TagId);

            builder.Entity<Like>(x => x.HasKey(p => new {p.PostId, p.UserId}));
            
            builder.Entity<Like>()
                .HasOne(u => u.Post)
                .WithMany(u => u.LikedBy)
                .HasForeignKey(p => p.PostId);

            builder.Entity<Like>()
                .HasOne(u => u.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(p => p.UserId);

            builder.Entity<CommunityUser>(x => x.HasKey(p => new {p.CommunityId, p.UserId}));
            
            builder.Entity<CommunityUser>()
                .HasOne(u => u.Community)
                .WithMany(u => u.CommunityUsers)
                .HasForeignKey(p => p.CommunityId);

            builder.Entity<CommunityUser>()
                .HasOne(u => u.User)
                .WithMany(u => u.CommunityUsers)
                .HasForeignKey(p => p.UserId);

            builder.Entity<Comment>(x => x.HasKey(c => c.Id));
            
            builder.Entity<Comment>()
                .HasOne(u => u.Post)
                .WithMany(u => u.Comments)
                .HasForeignKey(p => p.PostId);

            builder.Entity<UserToken>(x => x.HasKey(t => t.Token));
        }
    }
}