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
            var result = _dbContext.GCPInfrastructureDomain.Where(a=>a.CUSTOMER_ID==customer_id).ToList();

            return result;
        }

        public async Task<FileContentResult> CreateNewInfrastructure(int customer_id, string language, string template, int id, string REGION, string CLUSTER_NAME, string ARTIFACT_REGISTRY_REGION, string ARTIFACT_REGISTRY, string APP_NAME)
        {
            
            var infras = new GCPInfrastructure
            {
                CUSTOMER_ID = customer_id,
                PROJECT_LANGUAJE = language,
                CREATED_AT = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time")),
                TEMPLATE_USED = template
            };

            _dbContext.GCPInfrastructureDomain.Add(infras);
            await _dbContext.SaveChangesAsync();

            
            var tem = await _dbtemplates.GCPTemplatesDomain.FirstOrDefaultAsync(a => a.ID == id);

            if (tem == null)
            {
                return null;
            }
            else
            {
                
                string content = Encoding.UTF8.GetString(tem.TERRAFORM_FILE);

                
                content = content.Replace("{{REGION}}", REGION)
                                 .Replace("{{CLUSTER_NAME}}", CLUSTER_NAME)
                                 .Replace("{{ARTIFACT_REGISTRY_REGION}}", ARTIFACT_REGISTRY_REGION)
                                 .Replace("{{ARTIFACT_REGISTRY}}", ARTIFACT_REGISTRY)
                                 .Replace("{{APP_NAME}}", APP_NAME);

                var fileName = $"{tem.TEMPLATE_NAME}.tf";
                return new FileContentResult(Encoding.UTF8.GetBytes(content), "application/octet-stream")
                {
                    FileDownloadName = fileName
                };
            }
        }


        public async Task<string> RemoveNewInfrastructure(int customer_id)
        {
            var infras = await _dbContext.GCPInfrastructureDomain.FirstOrDefaultAsync(a => a.CUSTOMER_ID == customer_id);

            if(infras != null)
            {

               _dbContext.GCPInfrastructureDomain.Remove(infras);
                await _dbContext.SaveChangesAsync();

                return "Infrastructure removed successfully";

            }
            else
            {
                return "Infrastructure not found";
            }
        }
    }
}
