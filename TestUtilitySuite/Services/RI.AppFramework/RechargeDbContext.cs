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
        //public DbSet<TestUtilityLog> TestUtilityLog { get; set; }
        public DbSet<Agent> Agents { get; set; }
    }
}
