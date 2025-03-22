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
    public interface IPipelinesRepository
    {        
        Task<FileContentResult> DownloadPipeline(int id, string file_name);


        List<Pipelines> AllPipelinesByID(int customer_id);


        Task<string> RemovePipelinesByID(int id);


        Task<string> UploadPipeline(int customer_id, IFormFile yaml_file, string cloud, string project_language, string descriptions, string appname, string cluster_name, string ArtifactRegistryName, string REGION);

    }
}
