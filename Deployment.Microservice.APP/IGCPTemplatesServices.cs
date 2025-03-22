using Infrastructure.Microservice.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Microservice.APP
{
    public interface IGCPTemplatesServices
    {
        Task<string> SaveNewTemplate(string template_name, byte[] terraform_file,string des);

        Task<string> RemoveTemplate(int id);

        List<GCPTemplates> AllTemplates();
    }
}
