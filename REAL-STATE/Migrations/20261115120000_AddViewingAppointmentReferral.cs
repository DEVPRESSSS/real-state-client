using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using REAL_STATE.Data;

namespace REAL_STATE.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261115120000_AddViewingAppointmentReferral")]
public partial class AddViewingAppointmentReferral : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
        => migrationBuilder.AddColumn<string>(
            name: "ReferralAgentId", table: "ViewingAppointments",
            type: "nvarchar(450)", maxLength: 450, nullable: true);

    protected override void Down(MigrationBuilder migrationBuilder)
        => migrationBuilder.DropColumn(name: "ReferralAgentId", table: "ViewingAppointments");
}
