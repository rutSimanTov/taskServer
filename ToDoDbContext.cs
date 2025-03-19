using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ToDoApi;

///<summary>
///Represents the database context for the ToDo application that manages the entities.
/// </summary>
public partial class ToDoDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToDoDbContext"/> class.
    /// </summary>
    public ToDoDbContext()
    {
    }

      /// <summary>
    /// Initializes a new instance of the <see cref="ToDoDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {
    }

     /// <summary>
    /// Gets or sets the collection of <see cref="Item"/> entities.
    /// </summary>
    public virtual DbSet<Item> Items { get; set; }

    /// <summary>
    /// Gets or sets the collection of <see cref="User"/> entities.
    /// </summary>
    public virtual DbSet<User> Users { get; set; }

    
    /// <summary>
    /// Configures the model for the context by using the <see cref="ModelBuilder"/>.
    /// </summary>
    /// <param name="modelBuilder">The model builder to be used for configuring the model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Item");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

      
         modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("User");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
        });
      
        OnModelCreatingPartial(modelBuilder);
    }

     /// <summary>
    /// A partial method that can be implemented in another part of the class for additional configuration.
    /// </summary>
    /// <param name="modelBuilder">The model builder to be used for additional configurations.</param>
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
