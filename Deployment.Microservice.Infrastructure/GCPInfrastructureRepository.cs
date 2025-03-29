using Infrastructure.Microservice.APP;
using Infrastructure.Microservice.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Microservice.Infrastructure
{
    public class GCPInfrastructureRepository : IGCPInfrastructureRepository
    {
        private readonly GCPInfrastructureDBContext _dbContext;
        private readonly GCPTemplatesDBContext _dbtemplates;

        public GCPInfrastructureRepository(GCPInfrastructureDBContext dbContext, GCPTemplatesDBContext t)
        {
            _dbContext = dbContext;
            _dbtemplates = t;
        }

        public List<GCPInfrastructure> AllInfrastructureByID(int customer_id)
        {
            return _dbContext.GCPInfrastructureDomain.Where(a => a.CUSTOMER_ID == customer_id).ToList();
        }

        public async Task<GCPInfrastructure> AddInfrastructure(GCPInfrastructure infras)
        {
            _dbContext.GCPInfrastructureDomain.Add(infras);
            await _dbContext.SaveChangesAsync();
            return infras;
        }

        public async Task<GCPTemplates> GetTemplateById(int id)
        {
            return await _dbtemplates.GCPTemplatesDomain.FirstOrDefaultAsync(a => a.ID == id);
        }

        public async Task<string> RemoveInfrastructureByID(int customer_id)
        {
            var infras = await _dbContext.GCPInfrastructureDomain.FirstOrDefaultAsync(a => a.CUSTOMER_ID == customer_id);
            if (infras != null)
            {
                _dbContext.GCPInfrastructureDomain.Remove(infras);
                await _dbContext.SaveChangesAsync();
                return "Infrastructure removed successfully";
            }
            return "Infrastructure not found";
        }
    }
}
