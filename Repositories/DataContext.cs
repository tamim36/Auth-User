using Microsoft.EntityFrameworkCore;
using Models.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
