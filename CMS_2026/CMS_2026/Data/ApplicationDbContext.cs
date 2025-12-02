using Microsoft.EntityFrameworkCore;
using CMS_2026.Data.Entities;

namespace CMS_2026.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Language and Configuration
        public DbSet<PP_Lang> PP_Langs { get; set; }
        public DbSet<PP_Compt> PP_Compts { get; set; }
        public DbSet<PP_Config> PP_Configs { get; set; }
        public DbSet<PP_Page> PP_Pages { get; set; }
        public DbSet<PP_Json> PP_Jsons { get; set; }

        // Content
        public DbSet<PP_Category> PP_Categories { get; set; }
        public DbSet<PP_Node> PP_Nodes { get; set; }
        public DbSet<PP_Product> PP_Products { get; set; }

        // E-commerce
        public DbSet<PP_Order> PP_Orders { get; set; }
        public DbSet<PP_productvariants> PP_ProductVariants { get; set; }
        public DbSet<PP_Variants> PP_Variants { get; set; }
        public DbSet<PP_variantValues> PP_VariantValues { get; set; }
        public DbSet<PP_productVariantValues> PP_ProductVariantValues { get; set; }

        // User and Permission
        public DbSet<PP_User> PP_Users { get; set; }
        public DbSet<PP_Roles> PP_Roles { get; set; }
        public DbSet<PP_UserRoles> PP_UserRoles { get; set; }
        public DbSet<PP_RoleClaims> PP_RoleClaims { get; set; }
        public DbSet<PP_User_log> PP_UserLogs { get; set; }

        // Statistics and Tracking
        public DbSet<PP_Visit> PP_Visits { get; set; }
        public DbSet<PP_Stats_Daily> PP_Stats_Dailies { get; set; }
        public DbSet<PP_Stats_Monthly> PP_Stats_Monthlies { get; set; }
        public DbSet<PP_Stats_Url> PP_Stats_Urls { get; set; }

        // Contact and Marketing
        public DbSet<PP_Contact> PP_Contacts { get; set; }
        public DbSet<PP_Advise> PP_Advises { get; set; }
        public DbSet<PP_Comment> PP_Comments { get; set; }
        public DbSet<PP_Register> PP_Registers { get; set; }
        public DbSet<PP_Subscribe> PP_Subscribes { get; set; }
        public DbSet<PP_Feedback> PP_Feedbacks { get; set; }
        public DbSet<PP_Evaluation> PP_Evaluations { get; set; }

        // Category Details
        public DbSet<PP_Category_details> PP_Category_Details { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure table names
            modelBuilder.Entity<PP_Lang>().ToTable("pp_lang");
            modelBuilder.Entity<PP_Compt>().ToTable("pp_compt");
            modelBuilder.Entity<PP_Config>().ToTable("pp_config");
            modelBuilder.Entity<PP_Page>().ToTable("pp_page");
            modelBuilder.Entity<PP_Json>().ToTable("pp_json");
            modelBuilder.Entity<PP_Category>().ToTable("pp_category");
            modelBuilder.Entity<PP_Node>().ToTable("pp_node");
            modelBuilder.Entity<PP_Product>().ToTable("pp_product");
            modelBuilder.Entity<PP_Order>().ToTable("pp_order");
            modelBuilder.Entity<PP_productvariants>().ToTable("pp_productvariants");
            modelBuilder.Entity<PP_Variants>().ToTable("pp_variants");
            modelBuilder.Entity<PP_variantValues>().ToTable("pp_variantvalues");
            modelBuilder.Entity<PP_productVariantValues>().ToTable("pp_productvariantvalues");
            modelBuilder.Entity<PP_User>().ToTable("pp_user");
            modelBuilder.Entity<PP_Roles>().ToTable("pp_roles");
            modelBuilder.Entity<PP_UserRoles>().ToTable("pp_user_roles");
            modelBuilder.Entity<PP_RoleClaims>().ToTable("pp_role_claims");
            modelBuilder.Entity<PP_User_log>().ToTable("pp_user_log");
            modelBuilder.Entity<PP_Visit>().ToTable("pp_visit");
            modelBuilder.Entity<PP_Stats_Daily>().ToTable("pp_stats_daily");
            modelBuilder.Entity<PP_Stats_Monthly>().ToTable("pp_stats_monthly");
            modelBuilder.Entity<PP_Stats_Url>().ToTable("pp_stats_url");
            modelBuilder.Entity<PP_Contact>().ToTable("pp_contact");
            modelBuilder.Entity<PP_Advise>().ToTable("pp_advise");
            modelBuilder.Entity<PP_Comment>().ToTable("pp_comment");
            modelBuilder.Entity<PP_Register>().ToTable("pp_register");
            modelBuilder.Entity<PP_Subscribe>().ToTable("pp_subscribe");
            modelBuilder.Entity<PP_Feedback>().ToTable("pp_feedback");
            modelBuilder.Entity<PP_Evaluation>().ToTable("pp_evaluation");
            modelBuilder.Entity<PP_Category_details>().ToTable("pp_category_details");

            // Configure indexes
            modelBuilder.Entity<PP_Config>()
                .HasIndex(c => new { c.LangId, c.PageId, c.ConfigKey })
                .IsUnique();

            modelBuilder.Entity<PP_Page>()
                .HasIndex(p => new { p.LangId, p.PathPattern });

            modelBuilder.Entity<PP_Category>()
                .HasIndex(c => new { c.LangId, c.CategoryPath });
        }
    }
}

