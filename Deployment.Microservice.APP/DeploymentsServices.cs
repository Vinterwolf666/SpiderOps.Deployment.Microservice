using Deployment.Microservice.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.APP
{
    public class DeploymentsServices : IDeploymentsServices
    {
        private readonly IDeploymentsRepository _repository;
        public DeploymentsServices(IDeploymentsRepository repository)
        {

            _repository = repository;

        }
        public List<Deployments> AllDeploymentsById(int id)
        {
            var result = _repository.AllDeploymentsById(id);

            return result;
        }

        public async Task<string> NewDeployment(string BASE_GITHUB_URL, int CUSTOMER_ID, string REGION, int template_id, string cluster_name, string ArtifactRegistryName, string appname, string language, string CLUSTER_NAME, string ARTIFACT_REGISTRY_REGION, string ARTIFACT_REGISTRY, int customer_id)
        {
            var result = await _repository.NewDeployment( BASE_GITHUB_URL,  CUSTOMER_ID,  REGION,  template_id,  cluster_name,  ArtifactRegistryName,  appname,  language,  CLUSTER_NAME,  ARTIFACT_REGISTRY_REGION,  ARTIFACT_REGISTRY, customer_id);

            return result;
        }
/*
        public async Task<string> RemoveDeployment(int id, string deploymentName, string cluster, string zone, string project)
        {
            var result = await _repository.RemoveDeployment(id, deploymentName,cluster,zone,project);

            return result;
        }
*/
        public async Task<string> RemoveDeploymentID(int id)
        {
            var result = await _repository.RemoveDeploymentID(id);

            return result;
        }

        public async Task<string> TriggerPipelineCommit(string repoUrl)
        {
            var result = await _repository.TriggerPipelineCommit(repoUrl);

            return result;

        }

        
    }
}
