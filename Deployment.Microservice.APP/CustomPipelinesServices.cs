using Deployment.Microservice.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.APP
{
    public class CustomPipelinesServices : ICustomPipelinesServices
    {
        private readonly ICustomerPipelinesRepository _r;
        public CustomPipelinesServices(ICustomerPipelinesRepository r)
        {
            _r = r;
        }
        public async Task<FileContentResult> UpdatePipeline(int customer_id, int template_id, string cluster_name, string ArtifactRegistry, string REGION, string appname)
        {
            var result = await _r.UpdatePipeline(customer_id,template_id,cluster_name,ArtifactRegistry,REGION,appname);    

            return result;

        }

        public Task<List<object>> dropGitHub()
        {

            return _r.dropGitHub();
        }

        public Task<List<object>> dropSecrets()
        {
            return _r.dropSecrets();
        }
    }
}
