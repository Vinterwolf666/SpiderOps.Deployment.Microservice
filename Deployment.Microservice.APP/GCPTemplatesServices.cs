using Infrastructure.Microservice.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Microservice.APP
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
            var result = _repository.AllTemplates();

            return result;
        }

        public async Task<string> RemoveTemplate(int id)
        {
            var result = await _repository.RemoveTemplate(id);
            return result;
        }

        public async Task<string> SaveNewTemplate(string template_name, byte[] terraform_file, string des)
        {
            var result = await _repository.SaveNewTemplate(template_name, terraform_file,des);
            return result;
        }
    }
}
