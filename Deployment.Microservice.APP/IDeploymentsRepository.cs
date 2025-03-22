using Deployment.Microservice.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.APP
{
    public interface IDeploymentsRepository
    {
        Task<string> NewDeployment(string BASE_GITHUB_URL, int CUSTOMER_ID, string REGION, int template_id, string cluster_name, string ArtifactRegistryName, string appname, string language, string CLUSTER_NAME, string ARTIFACT_REGISTRY_REGION, string ARTIFACT_REGISTRY, int customer_id);

      //  Task<string> RemoveDeployment(int id, string deploymentName, string cluster, string zone, string project);

        Task<string> RemoveDeploymentID(int id);

        List<Deployments> AllDeploymentsById(int id);

        Task<string> TriggerPipelineCommit(string repoUrl);

        

       


    }
}
