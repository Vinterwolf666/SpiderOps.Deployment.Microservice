using Infrastructure.Microservice.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Microservice.Infrastructure
{
    public class GCPTemplatesDBContext : DbContext
    {
        public GCPTemplatesDBContext(DbContextOptions<GCPTemplatesDBContext> options)
            :base(options)
        {
            
        }



        public DbSet<GCPTemplates> GCPTemplatesDomain { get; set; }
    }
}
