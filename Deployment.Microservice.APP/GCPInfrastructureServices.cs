using Infrastructure.Microservice.APP;
using Infrastructure.Microservice.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.Service
{
    public class GCPInfrastructureServices : IGCPInfrastructureServices
    {
        private readonly IGCPInfrastructureRepository _repository;

        public GCPInfrastructureServices(IGCPInfrastructureRepository repository)
        {
            _repository = repository;
        }

        public List<GCPInfrastructure> AllInfrastructureByID(int customer_id)
        {
            return _repository.AllInfrastructureByID(customer_id);
        }

        public async Task<FileContentResult> CreateNewInfrastructure(int customer_id, string language, string template, int id, string REGION, string CLUSTER_NAME, string ARTIFACT_REGISTRY_REGION, string ARTIFACT_REGISTRY, string APP_NAME)
        {
            // Crear la infraestructura
            var infras = new GCPInfrastructure
            {
                CUSTOMER_ID = customer_id,
                PROJECT_LANGUAJE = language,
                CREATED_AT = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time")),
                TEMPLATE_USED = template
            };
            await _repository.AddInfrastructure(infras);

            // Obtener la plantilla
            var tem = await _repository.GetTemplateById(id);
            if (tem == null)
            {
                return null;
            }

            // Sustituir los valores en la plantilla
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

        public async Task<string> RemoveNewInfrastructure(int customer_id)
        {
            return await _repository.RemoveInfrastructureByID(customer_id);
        }
    }
}
