using Deployment.Microservice.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return _repository.AllPipelinesByID(customer_id);
        }

        public async Task<FileContentResult> DownloadPipeline(int id, string file_name)
        {
            try
            {
                return await _repository.DownloadPipeline(id, file_name);
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Pipeline file not found.");
            }
        }

        public async Task<string> RemovePipelinesByID(int id)
        {
            return await _repository.RemovePipelinesByID(id);
        }

        public async Task<string> UploadPipeline(int customer_id, IFormFile yaml_file, string cloud, string project_language, string descriptions, string appname, string cluster_name, string ArtifactRegistryName, string REGION)
        {
            if (yaml_file == null || yaml_file.Length == 0)
            {
                return "Invalid YAML file.";
            }

            return await _repository.UploadPipeline(customer_id, yaml_file, cloud, project_language, descriptions, appname, cluster_name, ArtifactRegistryName, REGION);
        }
    }
}
