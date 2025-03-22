using Infrastructure.Microservice.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Microservice.APP
{
    public class GCPInfrastructureServices : IGCPInfrastructureServices
    {
        private readonly IGCPInfrastructureRepository _repository;

        public GCPInfrastructureServices(IGCPInfrastructureRepository r)
        {
            _repository = r;
        }
        public List<GCPInfrastructure> AllInfrastructureByID(int customer_id)
        {
            var result = _repository.AllInfrastructureByID(customer_id);
            return result;
        }

        public async Task<FileContentResult> CreateNewInfrastructure(int customer_id, string language, string template, int id, string REGION, string CLUSTER_NAME, string ARTIFACT_REGISTRY_REGION, string ARTIFACT_REGISTRY, string APP_NAME)
        {
            var result = await _repository.CreateNewInfrastructure(customer_id, language,template,  id,  REGION,  CLUSTER_NAME,  ARTIFACT_REGISTRY_REGION, ARTIFACT_REGISTRY, APP_NAME);
            return result;
        }

        public async Task<string> RemoveNewInfrastructure(int customer_id)
        {
            var result = await _repository.RemoveNewInfrastructure(customer_id);

            return result;
        }
    }
}
