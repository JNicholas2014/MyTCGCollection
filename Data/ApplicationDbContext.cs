using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyTCGCollection.Data.Models;

namespace MyTCGCollection.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base()
        {
        }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Game> Games { get; set; }
   
    }
}
