using Infrastructure.Microservice.APP;
using Infrastructure.Microservice.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Microservice.Infrastructure
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
            var result = _dbContext.GCPTemplatesDomain.ToList();

            return result;
        }

        public async Task<string> RemoveTemplate(int id)
        {
            var temp = await _dbContext.GCPTemplatesDomain.FirstOrDefaultAsync(a => a.ID == id);

            if(temp != null)
            {
                _dbContext.GCPTemplatesDomain.Remove(temp);
                await _dbContext.SaveChangesAsync();

                return "Template removed successfully";
            }
            else
            {
                return "Template not found";
            }
        }

        public async Task<string> SaveNewTemplate(string template_name, byte[] terraform_file, string des)
        {
            var template = new GCPTemplates();

            template.TEMPLATE_NAME = template_name;
            template.CREATED_AT = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"));
            template.TERRAFORM_FILE = terraform_file;
            template.DESCRIPTIONS = des;

            _dbContext.GCPTemplatesDomain.Add(template);
            await _dbContext.SaveChangesAsync();

            return $"Template saved with the following ID: {template_name}";
        }
    }
}
