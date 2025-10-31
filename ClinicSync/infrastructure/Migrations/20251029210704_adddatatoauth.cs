using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adddatatoauth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("0be02a18-14b8-4f60-99c3-4b10e3eecf0d"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("5788f514-c919-4a29-b82d-bd7bd0a1df13"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("cc66e659-3a16-4b70-b33d-b885688c921b"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("dd88de84-8c45-4f8c-a5ee-87689bf5aa4a"));

            migrationBuilder.InsertData(
                table: "Specialties",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("0ed03a9c-066d-4728-a751-2c4dabeb88a4"), "Bones and muscles", true, "Orthopedics" },
                    { new Guid("483f923d-ec60-4025-a073-49ea8ff95d52"), "Skin, hair, and nails", true, "Dermatology" },
                    { new Guid("bdefcb91-4f4e-457f-804d-e6741e8e3e07"), "Children's health", true, "Pediatrics" },
                    { new Guid("f0a75e0b-3e1c-4bf1-ab18-56af0b617558"), "Heart and cardiovascular system", true, "Cardiology" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("0ed03a9c-066d-4728-a751-2c4dabeb88a4"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("483f923d-ec60-4025-a073-49ea8ff95d52"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("bdefcb91-4f4e-457f-804d-e6741e8e3e07"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("f0a75e0b-3e1c-4bf1-ab18-56af0b617558"));

            migrationBuilder.InsertData(
                table: "Specialties",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("0be02a18-14b8-4f60-99c3-4b10e3eecf0d"), "Heart and cardiovascular system", true, "Cardiology" },
                    { new Guid("5788f514-c919-4a29-b82d-bd7bd0a1df13"), "Bones and muscles", true, "Orthopedics" },
                    { new Guid("cc66e659-3a16-4b70-b33d-b885688c921b"), "Children's health", true, "Pediatrics" },
                    { new Guid("dd88de84-8c45-4f8c-a5ee-87689bf5aa4a"), "Skin, hair, and nails", true, "Dermatology" }
                });
        }
    }
}
