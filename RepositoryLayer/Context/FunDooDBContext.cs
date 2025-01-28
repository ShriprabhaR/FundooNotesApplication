using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Context
{
    public class FunDooDBContext : DbContext
    {
        public FunDooDBContext(DbContextOptions options) : base(options) { }

        public DbSet<Users> Users { get; set; }

    }
}
