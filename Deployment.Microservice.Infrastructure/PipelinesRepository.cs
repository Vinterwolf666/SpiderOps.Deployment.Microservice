using Deployment.Microservice.APP;
using Deployment.Microservice.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.Infrastructure
{
    public class PipelinesRepository : IPipelinesRepository
    {
        private readonly PipelinesDBContext _dbContext;
        public PipelinesRepository(PipelinesDBContext dbContext)
        {

            _dbContext = dbContext;

        }

        public List<Pipelines> AllPipelinesByID(int customer_id)
        {
            var result = _dbContext.PipelinesDomain.Where(a => a.CUSTOMER_ID == customer_id).ToList();

            return result;
        }

        public async Task<FileContentResult> DownloadPipeline(int id,string file_name)
        {
            var pipeline = await _dbContext.PipelinesDomain.FindAsync(id);

            if (pipeline == null || pipeline.YAML_FILE == null || pipeline.YAML_FILE.Length == 0)
            {
                throw new FileNotFoundException("Invalid YAML file.");
            }

            return new FileContentResult(pipeline.YAML_FILE, "application/x-yaml")
            {
                FileDownloadName = $"{file_name}.yaml"
            };
        }

        public async Task<string> RemovePipeline(int id)
        {
            var pipe = await _dbContext.PipelinesDomain.FirstOrDefaultAsync(a => a.ID == id);

            if (pipe != null)
            {
                _dbContext.PipelinesDomain.Remove(pipe);
                await _dbContext.SaveChangesAsync();

                return "Pipeline removed successfully";
            }
            else
            {
                return "Pipeline not found";
            }
        }

        public async Task<string> RemovePipelinesByID(int id)
        {
            var result = await _dbContext.PipelinesDomain.FindAsync(id);

            if(result != null)
            {

                _dbContext.PipelinesDomain.Remove(result);
               await _dbContext.SaveChangesAsync();

                return "Pipeline removed successfully";
            }
            else
            {
                return "Invalid pipeline";
            }

           
        }

        public async Task<string> UploadPipeline(int customer_id, IFormFile yamlFile, string cloud, string project_language, string descriptions, string appname, string cluster_name, string ArtifactRegistryName, string REGION)
        {

            try
            {

                var pipeline = new Pipelines();


                if (yamlFile == null || yamlFile.Length == 0)
                {
                    return "Invalid YAML file";
                }

                using (var memoryStream = new MemoryStream())
                {
                    await yamlFile.CopyToAsync(memoryStream);
                    pipeline.YAML_FILE = memoryStream.ToArray();
                }

                pipeline.CREATED_AT = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"));
                pipeline.CUSTOMER_ID = customer_id;
                pipeline.CLOUD = cloud;
                pipeline.PROJECT_LANGUAGE = project_language;
                pipeline.DESCRIPTIONS = descriptions;
                pipeline.APPNAME = appname;
                pipeline.CLUSTER_NAME = cluster_name;
                pipeline.ArtifactRegistryName = ArtifactRegistryName;
                pipeline.REGION = REGION;

                _dbContext.PipelinesDomain.Add(pipeline);
                await _dbContext.SaveChangesAsync();

                return "YAML saved correctly";
            }
            catch (Exception ex)
            {
                return $"An error has occurred while saving the YAML file: {ex.Message}";
            }




        }
    }
}
