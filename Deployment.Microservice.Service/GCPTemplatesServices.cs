using Infrastructure.Microservice.APP;
using Infrastructure.Microservice.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.Service
{
    public class GCPTemplatesServices : IGCPTemplatesServices
    {
        private readonly IGCPTemplatesRepository _repository;

        public GCPTemplatesServices(IGCPTemplatesRepository repository)
        {
            _repository = repository;
        }

        public List<GCPTemplates> AllTemplates()
        {
            return _repository.AllTemplates();
        }

        public async Task<string> SaveNewTemplate(string template_name, byte[] terraform_file, string des)
        {
            var template = new GCPTemplates
            {
                TEMPLATE_NAME = template_name,
                CREATED_AT = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time")),
                TERRAFORM_FILE = terraform_file,
                DESCRIPTIONS = des
            };

            return await _repository.AddTemplate(template);
        }

        public async Task<string> RemoveTemplate(int id)
        {
            var template = await _repository.GetTemplateById(id);

            if (template == null)
            {
                return "Template not found";
            }

            return await _repository.RemoveTemplate(template);
        }
    }
}
