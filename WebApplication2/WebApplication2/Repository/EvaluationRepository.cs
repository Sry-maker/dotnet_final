﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using WebApplication2.Models;

namespace WebApplication2.Repository
{
    public class EvaluationRepository : DbContext
    {
        public DbSet<Evaluation> Evaluations { get; set; }

        private IConfiguration configuration;

        public EvaluationRepository()
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
