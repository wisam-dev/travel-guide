using Configuration;
using DbContext.Extensions;
using DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;

namespace DbContext;

public class MainDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    DatabaseConnections _databaseConnections;

#if DEBUG
    public string dbConnection =>
        System.Text.RegularExpressions.Regex.Replace(
            this.Database.GetConnectionString() ?? "",
            @"(pwd|password)=[^;]*;?",
            "",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase
        );
#endif

    #region C# model of database tables
    public DbSet<CountryDbM> Countries { get; set; }
    public DbSet<CityDbM> Cities { get; set; }
    public DbSet<CategoryDbM> Categories { get; set; }
    public DbSet<AttractionDbM> Attractions { get; set; }
    public DbSet<UserDbM> Users { get; set; }
    public DbSet<CommentDbM> Comments { get; set; }
    #endregion

    #region constructors
    public MainDbContext() { }

    public MainDbContext(DbContextOptions options, DatabaseConnections databaseConnections)
        : base(options)
    {
        _databaseConnections = databaseConnections;
    }
    #endregion

    //Here we can modify the migration building
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region override modelbuilder

        // Country 1---* City
        modelBuilder
            .Entity<CityDbM>()
            .HasOne(c => c.Country)
            .WithMany(co => co.Cities)
            .HasForeignKey(c => c.CountryId)
            .OnDelete(DeleteBehavior.Restrict); // don't allow deleting a country that still has cities

        // City 1---* Attraction
        modelBuilder
            .Entity<AttractionDbM>()
            .HasOne(a => a.City)
            .WithMany(c => c.Attractions)
            .HasForeignKey(a => a.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        // Category 1---* Attraction
        modelBuilder
            .Entity<AttractionDbM>()
            .HasOne(a => a.Category)
            .WithMany(c => c.Attractions)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // User 1---* Comment (deleting a user deletes their comments)
        modelBuilder
            .Entity<CommentDbM>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Attraction 1---* Comment (deleting an attraction deletes its comments)
        modelBuilder
            .Entity<CommentDbM>()
            .HasOne(c => c.Attraction)
            .WithMany(a => a.Comments)
            .HasForeignKey(c => c.AttractionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes beyond the ones EF Core creates automatically for FKs
        modelBuilder.Entity<UserDbM>().HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<AttractionDbM>().HasIndex(a => a.Title);

        modelBuilder.Entity<CityDbM>().HasIndex(c => c.Name);

        modelBuilder.Entity<CountryDbM>().HasIndex(c => c.Name).IsUnique();

        modelBuilder.Entity<CategoryDbM>().HasIndex(c => c.Name).IsUnique();
        // Override the default varchar(200) convention for columns that can legitimately be longer
        modelBuilder.Entity<AttractionDbM>().Property(a => a.Title).HasColumnType("varchar(300)");
        modelBuilder
            .Entity<AttractionDbM>()
            .Property(a => a.Description)
            .HasColumnType("varchar(2000)");
        modelBuilder.Entity<CommentDbM>().Property(c => c.Text).HasColumnType("varchar(1000)");

        #endregion

        base.OnModelCreating(modelBuilder);
    }

    #region DbContext for some popular databases
    public class SqlServerDbContext : MainDbContext
    {
        public SqlServerDbContext() { }

        public SqlServerDbContext(DbContextOptions options, DatabaseConnections databaseConnections)
            : base(options, databaseConnections) { }

        //Used only for CodeFirst Database Migration and database update commands
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder = optionsBuilder.ConfigureForDesignTime(
                    (options, connectionString) =>
                        options.UseSqlServer(
                            connectionString,
                            options => options.EnableRetryOnFailure()
                        )
                );
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HaveColumnType("money");
            configurationBuilder.Properties<string>().HaveColumnType("varchar(200)");

            base.ConfigureConventions(configurationBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Add your own modelling based on done migrations
            base.OnModelCreating(modelBuilder);
        }
    }

    // public class MySqlDbContext : MainDbContext
    // {
    //     public MySqlDbContext() { }

    //     public MySqlDbContext(DbContextOptions options)
    //         : base(options, null) { }

    //     //Used only for CodeFirst Database Migration
    //     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //     {
    //         if (!optionsBuilder.IsConfigured)
    //         {
    //             optionsBuilder = optionsBuilder.ConfigureForDesignTime(
    //                 (options, connectionString) =>
    //                     options.UseMySql(
    //                         connectionString,
    //                         ServerVersion.AutoDetect(connectionString),
    //                         b =>
    //                             b.SchemaBehavior(
    //                                 Microting
    //                                     .EntityFrameworkCore
    //                                     .MySql
    //                                     .Infrastructure
    //                                     .MySqlSchemaBehavior
    //                                     .Translate,
    //                                 (schema, table) => $"{schema}_{table}"
    //                             )
    //                     )
    //             );
    //         }

    //         base.OnConfiguring(optionsBuilder);
    //     }

    //     protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    //     {
    //         configurationBuilder.Properties<string>().HaveColumnType("varchar(200)");

    //         base.ConfigureConventions(configurationBuilder);
    //     }
    // }

    // public class PostgresDbContext : MainDbContext
    // {
    //     public PostgresDbContext() { }

    //     public PostgresDbContext(DbContextOptions options)
    //         : base(options, null) { }

    //     //Used only for CodeFirst Database Migration
    //     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //     {
    //         if (!optionsBuilder.IsConfigured)
    //         {
    //             optionsBuilder = optionsBuilder.ConfigureForDesignTime(
    //                 (options, connectionString) => options.UseNpgsql(connectionString)
    //             );
    //         }

    //         base.OnConfiguring(optionsBuilder);
    //     }

    //     protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    //     {
    //         configurationBuilder.Properties<string>().HaveColumnType("varchar(200)");
    //         base.ConfigureConventions(configurationBuilder);
    //     }
    // }
    #endregion
}
