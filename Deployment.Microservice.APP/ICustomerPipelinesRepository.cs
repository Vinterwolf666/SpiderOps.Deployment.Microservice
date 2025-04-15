using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployment.Microservice.APP
{
    public interface ICustomerPipelinesRepository
    {
        Task<FileContentResult> GetPipelineTemplate(int template_id, string pipelineType);
        Task SavePipeline(int customer_id, int template_id, string cluster_name, string artifactRegistry, string region, string appname);
        Task<List<object>> dropGitHub();
        Task<List<object>> dropSecrets();

        Task<List<object>> dropSonar();
    }
}
