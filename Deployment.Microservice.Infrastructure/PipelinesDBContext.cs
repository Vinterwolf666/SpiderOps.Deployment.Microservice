using Deployment.Microservice.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.Infrastructure
{
    public class PipelinesDBContext : DbContext
    {
        public PipelinesDBContext(DbContextOptions<PipelinesDBContext> options)
            :base(options)
        {
            
        }



       public DbSet<Pipelines> PipelinesDomain { get; set; }


    }
}
