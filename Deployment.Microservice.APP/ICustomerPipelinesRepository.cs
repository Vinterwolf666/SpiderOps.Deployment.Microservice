using Deployment.Microservice.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.APP
{
    public interface ICustomerPipelinesRepository
    {
        Task<FileContentResult> UpdatePipeline(int customer_id, int template_id, string cluster_name, string ArtifactRegistry, string REGION, string appname);

        Task<List<object>> dropGitHub();

        Task<List<object>> dropSecrets();

    }
}
