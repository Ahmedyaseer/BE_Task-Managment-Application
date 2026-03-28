using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Managment_Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Column and index may already exist from a partial prior run
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Tasks') AND name = 'DepartmentId')
                BEGIN
                    ALTER TABLE [Tasks] ADD [DepartmentId] int NOT NULL DEFAULT 0;
                END
            ");

            // Set existing tasks' DepartmentId to their assigned employee's department
            migrationBuilder.Sql(@"
                UPDATE t
                SET t.DepartmentId = e.DepartmentId
                FROM [Tasks] t
                INNER JOIN [Employees] e ON t.AssignedToId = e.Id
                WHERE t.DepartmentId = 0;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Tasks') AND name = 'IX_Tasks_DepartmentId')
                BEGIN
                    CREATE INDEX [IX_Tasks_DepartmentId] ON [Tasks] ([DepartmentId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Tasks_Departments_DepartmentId')
                BEGIN
                    ALTER TABLE [Tasks] ADD CONSTRAINT [FK_Tasks_Departments_DepartmentId]
                        FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Departments_DepartmentId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DepartmentId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Tasks");
        }
    }
}
