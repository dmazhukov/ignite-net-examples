using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Apache.Ignite.Core.Cache.Configuration;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace IgniteEFCacheStore
{
    public class BloggingContext : DbContext
    {
        public BloggingContext() : base("DataSource=blogs.db")
        {
            // No-op.
        }

        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
    }

    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [QuerySqlField]
        public int BlogId { get; set; }
        public string Name { get; set; }
        public double Prop1 { get; set; }
        public double Prop2 { get; set; }
        public double Prop3 { get; set; }
        public double Prop4 { get; set; }
        public double Prop5 { get; set; }
        public double Prop6 { get; set; }
        public double Prop7 { get; set; }
        public double Prop8 { get; set; }
        public double Prop9 { get; set; }
        public double Prop10 { get; set; }
        public double Prop11 { get; set; }
        public double Prop12 { get; set; }
        public double Prop13 { get; set; }
        public double Prop14 { get; set; }
        public double Prop15 { get; set; }
        public double Prop16 { get; set; }
        public double Prop17 { get; set; }
        public double Prop18 { get; set; }
        public double Prop19 { get; set; }
        public double Prop20 { get; set; }

        // Navigation property
        public virtual List<Post> Posts { get; set; }
    }

    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [QuerySqlField]
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int BlogId { get; set; }
        [QuerySqlField]
        public double Prop1 { get; set; }
        public double Prop2 { get; set; }
        public double Prop3 { get; set; }
        public double Prop4 { get; set; }
        public double Prop5 { get; set; }
        public double Prop6 { get; set; }
        public double Prop7 { get; set; }
        public double Prop8 { get; set; }
        public double Prop9 { get; set; }
        public double Prop10 { get; set; }
        public double Prop11 { get; set; }
        public double Prop12 { get; set; }
        public double Prop13 { get; set; }
        public double Prop14 { get; set; }
        public double Prop15 { get; set; }
        public double Prop16 { get; set; }
        public double Prop17 { get; set; }
        public double Prop18 { get; set; }
        public double Prop19 { get; set; }
        public double Prop20 { get; set; }

        // Navigation property
        public virtual Blog Blog { get; set; }
    }
}
