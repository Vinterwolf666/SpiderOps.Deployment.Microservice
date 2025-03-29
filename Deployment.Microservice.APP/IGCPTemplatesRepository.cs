using Infrastructure.Microservice.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Microservice.APP
{
    public interface IGCPTemplatesRepository
    {
        List<GCPTemplates> AllTemplates();

        Task<GCPTemplates> GetTemplateById(int id);

        Task<string> AddTemplate(GCPTemplates template);

        Task<string> RemoveTemplate(GCPTemplates template);
    }
}
