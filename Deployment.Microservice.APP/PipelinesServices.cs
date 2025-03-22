using Deployment.Microservice.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Deployment.Microservice.APP
{
    public class PipelinesServices : IPipelinesServices
    {
        private readonly IPipelinesRepository _repository;
        public PipelinesServices(IPipelinesRepository repository)
        {

            _repository = repository;

        }

        public List<Pipelines> AllPipelinesByID(int customer_id)
        {
            var result =  _repository.AllPipelinesByID(customer_id);

            return result;
        }

        public async Task<FileContentResult> DownloadPipeline(int id, string file_name)
        {
            var result = await _repository.DownloadPipeline(id, file_name);

            return result;
        }

        public async Task<string> RemovePipelinesByID(int id)
        {
            var result = await _repository.RemovePipelinesByID(id);

            return result;
        }

        public Task<string> UploadPipeline(int customer_id, IFormFile yaml_file, string cloud, string project_language, string descriptions, string appname, string cluster_name, string ArtifactRegistryName, string REGION)
        {
            var result = _repository.UploadPipeline(customer_id, yaml_file, cloud, project_language, descriptions, appname, cluster_name, ArtifactRegistryName, REGION);

            return result;
        }
    }
}
