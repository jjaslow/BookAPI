using BookAPIProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIProject.Services
{
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options)
            : base(options)
        {
            Database.Migrate();
        }


        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Reviewer> Reviewers { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<BookAuthor> BookAuthors { get; set; }
        public virtual DbSet<BookCategory> BookCategories { get; set; }



        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<BookCategory>()
                .HasKey(bc => new { bc.BookId, bc.CategoryId });
            modelbuilder.Entity<BookCategory>()
                .HasOne(b => b.Book)
                .WithMany(bc => bc.BookCategories)
                .HasForeignKey(b => b.BookId);
            modelbuilder.Entity<BookCategory>()
                .HasOne(c => c.Category)
                .WithMany(bc => bc.BookCategories)
                .HasForeignKey(c => c.CategoryId);


            modelbuilder.Entity<BookAuthor>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });
            modelbuilder.Entity<BookAuthor>()
                .HasOne(b => b.Book)
                .WithMany(ba => ba.BookAuthors)
                .HasForeignKey(b => b.BookId);
            modelbuilder.Entity<BookAuthor>()
                .HasOne(a => a.Author)
                .WithMany(ba => ba.BookAuthors)
                .HasForeignKey(a => a.AuthorId);
        }



    }
}
