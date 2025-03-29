using Infrastructure.Microservice.APP;
using Infrastructure.Microservice.Domain;
using Infrastructure.Microservice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deployment.Microservice.Infrastructure
{
    public class GCPTemplatesRepository : IGCPTemplatesRepository
    {
        private readonly GCPTemplatesDBContext _dbContext;

        public GCPTemplatesRepository(GCPTemplatesDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<GCPTemplates> AllTemplates()
        {
            return _dbContext.GCPTemplatesDomain.ToList();
        }

        public async Task<GCPTemplates> GetTemplateById(int id)
        {
            return await _dbContext.GCPTemplatesDomain.FirstOrDefaultAsync(a => a.ID == id);
        }

        public async Task<string> AddTemplate(GCPTemplates template)
        {
            _dbContext.GCPTemplatesDomain.Add(template);
            await _dbContext.SaveChangesAsync();
            return $"Template saved with the following name: {template.TEMPLATE_NAME}";
        }

        public async Task<string> RemoveTemplate(GCPTemplates template)
        {
            _dbContext.GCPTemplatesDomain.Remove(template);
            await _dbContext.SaveChangesAsync();
            return "Template removed successfully";
        }
    }
}
