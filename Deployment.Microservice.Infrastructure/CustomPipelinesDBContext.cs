using Deployment.Microservice.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.Infrastructure
{
    public class CustomPipelinesDBContext : DbContext
    {
        public CustomPipelinesDBContext(DbContextOptions<CustomPipelinesDBContext> options)
            :base(options)
        {
       

        }


        public DbSet<CustomPipelines> CustomPipelinesDomain { get; set; }
    }
}
