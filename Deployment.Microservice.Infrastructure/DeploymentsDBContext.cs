using Deployment.Microservice.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.Infrastructure
{
    public class DeploymentsDBContext : DbContext
    {
        
        public DeploymentsDBContext(DbContextOptions<DeploymentsDBContext> options)
            :base(options)
        {
            
        }



        public DbSet<Deployments> DeploymentsDomain { get; set; }
    }
}
