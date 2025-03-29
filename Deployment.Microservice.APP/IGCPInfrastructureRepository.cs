using Infrastructure.Microservice.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Microservice.APP
{
    public interface IGCPInfrastructureRepository
    {
        List<GCPInfrastructure> AllInfrastructureByID(int customer_id);

        Task<GCPInfrastructure> AddInfrastructure(GCPInfrastructure infras);

        Task<GCPTemplates> GetTemplateById(int id);

        Task<string> RemoveInfrastructureByID(int customer_id);
    }
}
