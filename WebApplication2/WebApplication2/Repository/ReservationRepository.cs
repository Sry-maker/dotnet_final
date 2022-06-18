using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using WebApplication2.Models;

namespace WebApplication2.Repository
{
    public class ReservationRepository : DbContext
    {
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<DiningTable> Tables { get; set; }


        private IConfiguration configuration;

        public ReservationRepository()
        {
            configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile("appsettings.json").Build();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder ob)
        {
            string strConn = configuration.GetConnectionString("Default");
            ob.UseMySql(strConn, ServerVersion.AutoDetect(strConn));
        }

    }
}
