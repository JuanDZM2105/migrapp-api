using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace migrapp_api.Migrations
{
    /// <inheritdoc />
    public partial class RemodelateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LegalProcessDocuments");

            migrationBuilder.RenameColumn(
                name: "UserType",
                table: "Users",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UserLogId",
                table: "UserLogs",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ProcessType",
                table: "LegalProcesses",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "ProcessStatus",
                table: "LegalProcesses",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "LegalProcessId",
                table: "LegalProcesses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "DocumentType",
                table: "Documents",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "DocumentId",
                table: "Documents",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "AssignedUserId",
                table: "AssignedUsers",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "OtpSecretKey",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "LegalProcesses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Procedures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LegalProcessId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Procedures_LegalProcesses_LegalProcessId",
                        column: x => x.LegalProcessId,
                        principalTable: "LegalProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcedureDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsUploaded = table.Column<bool>(type: "bit", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: true),
                    ProcedureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedureDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcedureDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProcedureDocuments_Procedures_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "Procedures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureDocuments_DocumentId",
                table: "ProcedureDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureDocuments_ProcedureId",
                table: "ProcedureDocuments",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_LegalProcessId",
                table: "Procedures",
                column: "LegalProcessId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcedureDocuments");

            migrationBuilder.DropTable(
                name: "Procedures");

            migrationBuilder.DropColumn(
                name: "OtpSecretKey",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "LegalProcesses");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Users",
                newName: "UserType");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserLogs",
                newName: "UserLogId");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "LegalProcesses",
                newName: "ProcessType");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "LegalProcesses",
                newName: "ProcessStatus");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "LegalProcesses",
                newName: "LegalProcessId");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Documents",
                newName: "DocumentType");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Documents",
                newName: "DocumentId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AssignedUsers",
                newName: "AssignedUserId");

            migrationBuilder.CreateTable(
                name: "LegalProcessDocuments",
                columns: table => new
                {
                    LegalProcessDocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: true),
                    LegalProcessId = table.Column<int>(type: "int", nullable: false),
                    IsUploaded = table.Column<bool>(type: "bit", nullable: false),
                    RequiredDocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalProcessDocuments", x => x.LegalProcessDocumentId);
                    table.ForeignKey(
                        name: "FK_LegalProcessDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "DocumentId");
                    table.ForeignKey(
                        name: "FK_LegalProcessDocuments_LegalProcesses_LegalProcessId",
                        column: x => x.LegalProcessId,
                        principalTable: "LegalProcesses",
                        principalColumn: "LegalProcessId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LegalProcessDocuments_DocumentId",
                table: "LegalProcessDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalProcessDocuments_LegalProcessId",
                table: "LegalProcessDocuments",
                column: "LegalProcessId");
        }
    }
}
