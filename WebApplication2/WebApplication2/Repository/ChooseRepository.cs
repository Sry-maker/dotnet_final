using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using WebApplication2.Models;

namespace WebApplication2.Repository
{
    public class ChooseRepository : DbContext
    {
        public DbSet<Choose> Chooses { get; set; }

        public DbSet<Dish>  Dishes{ get; set; }

        private IConfiguration configuration;

        public ChooseRepository()
        {
            configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile("appsettings.json").Build();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder ob)
        {
            string strConn = configuration.GetConnectionString("Default");
            ob.UseMySql(strConn, ServerVersion.AutoDetect(strConn));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Choose>().HasKey(t => new { t.order_id, t.dish_id, t.order_date });
        }

    }
}
