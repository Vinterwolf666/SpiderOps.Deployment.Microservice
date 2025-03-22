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
        Task<FileContentResult> CreateNewInfrastructure(int customer_id, string language, string template, int id, string REGION, string CLUSTER_NAME, string ARTIFACT_REGISTRY_REGION, string ARTIFACT_REGISTRY, string APP_NAME);
        Task<string> RemoveNewInfrastructure(int customer_id);

        List<GCPInfrastructure> AllInfrastructureByID(int customer_id);
    }
}
