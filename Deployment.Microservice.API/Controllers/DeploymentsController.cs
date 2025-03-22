using Deployment.Microservice.APP;
using Deployment.Microservice.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Deployment.Microservice.API.Controllers
{
    [ApiController]
    [Route("Deployment.Microservice.DEP.API")]
    public class DeploymentsController : Controller
    {
        private readonly IDeploymentsServices _services;
        public DeploymentsController(IDeploymentsServices s)
        {
            _services = s;
        }


        [HttpPost]
        [Route("NewDeployment")]
        public async Task<ActionResult<string>> NewDeployment(string BASE_GITHUB_URL, int CUSTOMER_ID, string REGION, int template_id, string cluster_name, string ArtifactRegistry, string appname, string language, string CLUSTER_NAME, string ARTIFACT_REGISTRY_REGION, string ARTIFACT_REGISTRY, int customer_id)
        {
            try
            {
                var result = await _services.NewDeployment( BASE_GITHUB_URL, CUSTOMER_ID,  REGION,  template_id,  cluster_name,  ArtifactRegistry,  appname,  language,  CLUSTER_NAME,  ARTIFACT_REGISTRY_REGION,  ARTIFACT_REGISTRY,  customer_id);

                return Ok(result);

            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("TriggerDeploymentGCPDeploy")]
        public async Task<ActionResult<string>> TriggerDeploymentGCPDeploy(string repoUrl)
        {
            try
            {
                var result = await _services.TriggerPipelineCommit(repoUrl);

                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("AllGCPDeployByID")]
        public ActionResult<List<Deployments>> AllGCPDeployByID(int id)
        {
            try
            {
                var result = _services.AllDeploymentsById(id);

                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

/*
        [HttpDelete]
        [Route("DeleteGCPDeployByID")]
        public async Task<ActionResult<string>> DeleteGCPDeployByID(int id, string deploymentName, string cluster, string zone, string project)
        {
            try
            {
                var result = await _services.RemoveDeployment(id,deploymentName,cluster,zone,project);

                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
*/
        [HttpDelete]
        [Route("DeleteDeploymentByID")]
        public async Task<ActionResult<string>> RemoveDeploymentID(int id)
        {
            try
            {

                var result = await _services.RemoveDeploymentID(id);

                return Ok(result);

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
