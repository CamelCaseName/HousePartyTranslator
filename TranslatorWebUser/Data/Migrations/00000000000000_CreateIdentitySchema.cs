using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TranslatorWebUser.Data.Migrations
{
	public partial class CreateIdentitySchema : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.CreateTable(
				name: "AspNetRoles",
				columns: table => new
				{
					Id = table.Column<string>(nullable: false),
					Name = table.Column<string>(maxLength: 256, nullable: true),
					NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
					ConcurrencyStamp = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_AspNetRoles", x => x.Id);
				});

			_ = migrationBuilder.CreateTable(
				name: "AspNetUsers",
				columns: table => new
				{
					Id = table.Column<string>(nullable: false),
					UserName = table.Column<string>(maxLength: 256, nullable: true),
					NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
					Email = table.Column<string>(maxLength: 256, nullable: true),
					NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
					EmailConfirmed = table.Column<bool>(nullable: false),
					PasswordHash = table.Column<string>(nullable: true),
					SecurityStamp = table.Column<string>(nullable: true),
					ConcurrencyStamp = table.Column<string>(nullable: true),
					PhoneNumber = table.Column<string>(nullable: true),
					PhoneNumberConfirmed = table.Column<bool>(nullable: false),
					TwoFactorEnabled = table.Column<bool>(nullable: false),
					LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
					LockoutEnabled = table.Column<bool>(nullable: false),
					AccessFailedCount = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_AspNetUsers", x => x.Id);
				});

			_ = migrationBuilder.CreateTable(
				name: "AspNetRoleClaims",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					RoleId = table.Column<string>(nullable: false),
					ClaimType = table.Column<string>(nullable: true),
					ClaimValue = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "AspNetUserClaims",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					UserId = table.Column<string>(nullable: false),
					ClaimType = table.Column<string>(nullable: true),
					ClaimValue = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_AspNetUserClaims_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "AspNetUserLogins",
				columns: table => new
				{
					LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
					ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
					ProviderDisplayName = table.Column<string>(nullable: true),
					UserId = table.Column<string>(nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
					_ = table.ForeignKey(
						name: "FK_AspNetUserLogins_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "AspNetUserRoles",
				columns: table => new
				{
					UserId = table.Column<string>(nullable: false),
					RoleId = table.Column<string>(nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
					_ = table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					_ = table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "AspNetUserTokens",
				columns: table => new
				{
					UserId = table.Column<string>(nullable: false),
					LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
					Name = table.Column<string>(maxLength: 128, nullable: false),
					Value = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
					_ = table.ForeignKey(
						name: "FK_AspNetUserTokens_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateIndex(
				name: "IX_AspNetRoleClaims_RoleId",
				table: "AspNetRoleClaims",
				column: "RoleId");

			_ = migrationBuilder.CreateIndex(
				name: "RoleNameIndex",
				table: "AspNetRoles",
				column: "NormalizedName",
				unique: true,
				filter: "[NormalizedName] IS NOT NULL");

			_ = migrationBuilder.CreateIndex(
				name: "IX_AspNetUserClaims_UserId",
				table: "AspNetUserClaims",
				column: "UserId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_AspNetUserLogins_UserId",
				table: "AspNetUserLogins",
				column: "UserId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_AspNetUserRoles_RoleId",
				table: "AspNetUserRoles",
				column: "RoleId");

			_ = migrationBuilder.CreateIndex(
				name: "EmailIndex",
				table: "AspNetUsers",
				column: "NormalizedEmail");

			_ = migrationBuilder.CreateIndex(
				name: "UserNameIndex",
				table: "AspNetUsers",
				column: "NormalizedUserName",
				unique: true,
				filter: "[NormalizedUserName] IS NOT NULL");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropTable(
				name: "AspNetRoleClaims");

			_ = migrationBuilder.DropTable(
				name: "AspNetUserClaims");

			_ = migrationBuilder.DropTable(
				name: "AspNetUserLogins");

			_ = migrationBuilder.DropTable(
				name: "AspNetUserRoles");

			_ = migrationBuilder.DropTable(
				name: "AspNetUserTokens");

			_ = migrationBuilder.DropTable(
				name: "AspNetRoles");

			_ = migrationBuilder.DropTable(
				name: "AspNetUsers");
		}
	}
}
