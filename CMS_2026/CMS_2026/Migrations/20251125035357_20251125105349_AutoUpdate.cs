using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS_2026.Migrations
{
    /// <inheritdoc />
    public partial class _20251125105349_AutoUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pp_advise",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Content = table.Column<string>(type: "ntext", nullable: true),
                    ProcessNote = table.Column<string>(type: "ntext", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_advise", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LangId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    PageId = table.Column<int>(type: "int", nullable: false),
                    PageIdItem = table.Column<int>(type: "int", nullable: false),
                    NodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CategoryLevel = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Breadcrumb = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CategoryPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MetaKeywords = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_category_details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LangId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    PageId = table.Column<int>(type: "int", nullable: false),
                    PageIdItem = table.Column<int>(type: "int", nullable: false),
                    NodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Idproduct = table.Column<int>(type: "int", nullable: true),
                    Idcat = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_category_details", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_comment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Comment = table.Column<string>(type: "ntext", nullable: true),
                    ProcessNote = table.Column<string>(type: "ntext", nullable: true),
                    notetype = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    idproduct = table.Column<int>(type: "int", nullable: false),
                    iduser = table.Column<int>(type: "int", nullable: false),
                    star = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_comment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_compt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComptKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ComptType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ComptName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PathPostfix = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    JsonSchema = table.Column<string>(type: "ntext", nullable: true),
                    JsonDefault = table.Column<string>(type: "ntext", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_compt", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LangId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PageId = table.Column<int>(type: "int", nullable: false),
                    ConfigKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    JsonContent = table.Column<string>(type: "ntext", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_config", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_contact",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProcessNote = table.Column<string>(type: "ntext", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_contact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_evaluation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Message = table.Column<string>(type: "ntext", nullable: true),
                    ProcessNote = table.Column<string>(type: "ntext", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_evaluation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_feedback",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Message = table.Column<string>(type: "ntext", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProcessNote = table.Column<string>(type: "ntext", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_feedback", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_json",
                columns: table => new
                {
                    JsonKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    JsonContent = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_json", x => x.JsonKey);
                });

            migrationBuilder.CreateTable(
                name: "pp_lang",
                columns: table => new
                {
                    LangId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_lang", x => x.LangId);
                });

            migrationBuilder.CreateTable(
                name: "pp_node",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LangId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    PageId = table.Column<int>(type: "int", nullable: false),
                    NodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NodeStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NodePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Featured = table.Column<bool>(type: "bit", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Content = table.Column<string>(type: "ntext", nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MetaKeywords = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    listcat = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_node", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_order",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrderStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TotalWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShipFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Ward = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "ntext", nullable: true),
                    JsonData = table.Column<string>(type: "ntext", nullable: true),
                    ReasonForCancel = table.Column<string>(type: "ntext", nullable: true),
                    GhtkStatusId = table.Column<int>(type: "int", nullable: false),
                    GhtkLabel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GhtkFee = table.Column<int>(type: "int", nullable: false),
                    GhTkEstimatedPickTime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GhtkEstimatedDeliverTime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_page",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LangId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PageStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PathPattern = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ComptKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ComptName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MetaKeywords = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_page", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LangId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PageId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    NodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NodeStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NodePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Content = table.Column<string>(type: "ntext", nullable: true),
                    Des = table.Column<string>(type: "ntext", nullable: true),
                    Note = table.Column<string>(type: "ntext", nullable: true),
                    AttrbEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AttrbName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AttrbValues = table.Column<string>(type: "ntext", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PromotionPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PromotionLabel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PromotionEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PromotionExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BestSeller = table.Column<bool>(type: "bit", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StockQty = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    ViewCounter = table.Column<int>(type: "int", nullable: false),
                    SoldCounter = table.Column<int>(type: "int", nullable: false),
                    MetaDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MetaKeywords = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImagesJson = table.Column<string>(type: "ntext", nullable: true),
                    listcat = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    View = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_productvariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LangId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    PageId = table.Column<int>(type: "int", nullable: false),
                    PageIdItem = table.Column<int>(type: "int", nullable: false),
                    NodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProductIP = table.Column<int>(type: "int", nullable: false),
                    IDSKD = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValueID = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    VariantID = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_productvariants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_productvariantvalues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LangId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    PageId = table.Column<int>(type: "int", nullable: false),
                    PageIdItem = table.Column<int>(type: "int", nullable: false),
                    NodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProductVariantID = table.Column<int>(type: "int", nullable: false),
                    Idproduct = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VariantID = table.Column<int>(type: "int", nullable: false),
                    ValueID = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_productvariantvalues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_register",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Message = table.Column<string>(type: "ntext", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PASSWORD = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Idorder = table.Column<int>(type: "int", nullable: true),
                    ProcessNote = table.Column<string>(type: "ntext", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_register", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_role_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ClaimValue = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_role_claims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ConcurrencyStamp = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_stats_daily",
                columns: table => new
                {
                    Date = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitCount = table.Column<int>(type: "int", nullable: false),
                    OrderCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_stats_daily", x => x.Date);
                });

            migrationBuilder.CreateTable(
                name: "pp_stats_monthly",
                columns: table => new
                {
                    Month = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitCount = table.Column<int>(type: "int", nullable: false),
                    OrderCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_stats_monthly", x => x.Month);
                });

            migrationBuilder.CreateTable(
                name: "pp_stats_url",
                columns: table => new
                {
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    VisitCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_stats_url", x => x.Url);
                });

            migrationBuilder.CreateTable(
                name: "pp_subscribe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SubscribeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_subscribe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_user",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_user_log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Surname = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_user_log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_user_roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_user_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_variants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LangId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    PageId = table.Column<int>(type: "int", nullable: false),
                    PageIdItem = table.Column<int>(type: "int", nullable: false),
                    NodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Idproduct = table.Column<int>(type: "int", nullable: true),
                    VariantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_variants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_variantvalues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LangId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    PageId = table.Column<int>(type: "int", nullable: false),
                    PageIdItem = table.Column<int>(type: "int", nullable: false),
                    NodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Idproduct = table.Column<int>(type: "int", nullable: true),
                    VariantID = table.Column<int>(type: "int", nullable: false),
                    ValueName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_variantvalues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pp_visit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Date = table.Column<int>(type: "int", nullable: false),
                    LastUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Referer = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Device = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Browser = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Ip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ClickCount = table.Column<int>(type: "int", nullable: false),
                    MakeOrder = table.Column<bool>(type: "bit", nullable: false),
                    StayTime = table.Column<int>(type: "int", nullable: false),
                    JsonDetails = table.Column<string>(type: "ntext", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pp_visit", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pp_category_LangId_CategoryPath",
                table: "pp_category",
                columns: new[] { "LangId", "CategoryPath" });

            migrationBuilder.CreateIndex(
                name: "IX_pp_config_LangId_PageId_ConfigKey",
                table: "pp_config",
                columns: new[] { "LangId", "PageId", "ConfigKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pp_page_LangId_PathPattern",
                table: "pp_page",
                columns: new[] { "LangId", "PathPattern" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pp_advise");

            migrationBuilder.DropTable(
                name: "pp_category");

            migrationBuilder.DropTable(
                name: "pp_category_details");

            migrationBuilder.DropTable(
                name: "pp_comment");

            migrationBuilder.DropTable(
                name: "pp_compt");

            migrationBuilder.DropTable(
                name: "pp_config");

            migrationBuilder.DropTable(
                name: "pp_contact");

            migrationBuilder.DropTable(
                name: "pp_evaluation");

            migrationBuilder.DropTable(
                name: "pp_feedback");

            migrationBuilder.DropTable(
                name: "pp_json");

            migrationBuilder.DropTable(
                name: "pp_lang");

            migrationBuilder.DropTable(
                name: "pp_node");

            migrationBuilder.DropTable(
                name: "pp_order");

            migrationBuilder.DropTable(
                name: "pp_page");

            migrationBuilder.DropTable(
                name: "pp_product");

            migrationBuilder.DropTable(
                name: "pp_productvariants");

            migrationBuilder.DropTable(
                name: "pp_productvariantvalues");

            migrationBuilder.DropTable(
                name: "pp_register");

            migrationBuilder.DropTable(
                name: "pp_role_claims");

            migrationBuilder.DropTable(
                name: "pp_roles");

            migrationBuilder.DropTable(
                name: "pp_stats_daily");

            migrationBuilder.DropTable(
                name: "pp_stats_monthly");

            migrationBuilder.DropTable(
                name: "pp_stats_url");

            migrationBuilder.DropTable(
                name: "pp_subscribe");

            migrationBuilder.DropTable(
                name: "pp_user");

            migrationBuilder.DropTable(
                name: "pp_user_log");

            migrationBuilder.DropTable(
                name: "pp_user_roles");

            migrationBuilder.DropTable(
                name: "pp_variants");

            migrationBuilder.DropTable(
                name: "pp_variantvalues");

            migrationBuilder.DropTable(
                name: "pp_visit");
        }
    }
}
