using Microsoft.EntityFrameworkCore;
using RI.AppFramework.EntityModel;
using RI.AppFramework.Models;
using System;

namespace RI.AppFramework
{
    public class RechargeDbContext : DbContext
    {
        public RechargeDbContext(DbContextOptions<RechargeDbContext> options)
        : base(options)
        {

        }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<PosAssignment> PosAssignments { get; set; }
        public DbSet<PosUnits> PosUnits { get; set; }
        public DbSet<PosUser> PosUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductAssignment> ProductAssignments { get; set; }
    }
}
