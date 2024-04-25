using Microsoft.EntityFrameworkCore;
using TCC_SAMMI.Domain.Entities;
using System.Reflection;

namespace TCC_SAMMI.Data.Context
{
    public class TCC_SAMMIContext : DbContext
    {
        public DbSet<Usuario> UsuariosSet { get; set; }
        public DbSet<Jogo> JogoSet {  get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            configurationBuilder.Properties<Enum>().HaveConversion<string>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());            

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string conexao = "Server=mysql.tccnapratica.com.br;Database=tccnapratica03;Uid=tccnapratica03;Pwd=Ms478t;";
            optionsBuilder.UseMySql(conexao, ServerVersion.AutoDetect(conexao));
            base.OnConfiguring(optionsBuilder);
        }
    }
}
