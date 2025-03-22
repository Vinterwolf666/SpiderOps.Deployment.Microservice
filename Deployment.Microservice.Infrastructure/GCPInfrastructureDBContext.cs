using Infrastructure.Microservice.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Microservice.Infrastructure
{
    public class GCPInfrastructureDBContext : DbContext
    {
        public GCPInfrastructureDBContext(DbContextOptions<GCPInfrastructureDBContext> options)
            :base(options)
        {
            
        }


        public DbSet<GCPInfrastructure> GCPInfrastructureDomain { get; set; }
    }
}
