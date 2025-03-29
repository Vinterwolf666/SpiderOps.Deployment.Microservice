using Deployment.Microservice.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.APP
{
    public class CustomPipelinesServices : ICustomPipelinesServices
    {
        private readonly ICustomerPipelinesRepository _repository;

        public CustomPipelinesServices(ICustomerPipelinesRepository repository)
        {
            _repository = repository;
        }

        public async Task<FileContentResult> UpdatePipeline(int customer_id, int template_id, string cluster_name, string artifactRegistry, string region, string appname)
        {
            
            if (string.IsNullOrEmpty(cluster_name) || string.IsNullOrEmpty(appname))
                throw new ArgumentException("El nombre del clúster y el nombre de la aplicación no pueden estar vacíos.");

           
            var fileContentResult = await _repository.GetPipelineTemplate(template_id, "build");

            if (fileContentResult?.FileContents == null || fileContentResult.FileContents.Length == 0)
                throw new FileNotFoundException("Archivo YAML inválido.");

            string content = Encoding.UTF8.GetString(fileContentResult.FileContents);
            content = content.Replace("{{CUSTOMER_ID}}", customer_id.ToString())
                             .Replace("{{CLUSTER_NAME}}", cluster_name)
                             .Replace("{{ARTIFACT_REGISTRY}}", artifactRegistry)
                             .Replace("{{REGION}}", region)
                             .Replace("{{APP_NAME}}", appname);

            byte[] fileBytes = Encoding.UTF8.GetBytes(content);

           
            await _repository.SavePipeline(customer_id, template_id, cluster_name, artifactRegistry, region, appname);

            return new FileContentResult(fileBytes, "text/yaml")
            {
                FileDownloadName = "build.yaml"
            };
        }

        public async Task<List<object>> dropGitHub()
        {
            return await _repository.dropGitHub();
        }

        public async Task<List<object>> dropSecrets()
        {
            return await _repository.dropSecrets();
        }





    }


    public class ApiResponse
    {
        public string type { get; set; }
        public string project_id { get; set; }
        public string private_key_id { get; set; }
        public string private_key { get; set; }
        public string client_email { get; set; }
        public string client_id { get; set; }

        public string auth_uri { get; set; }

        public string token_uri { get; set; }


        public string auth_provider_x509_cert_url { get; set; }

        public string client_x509_cert_url { get; set; }

        public string universe_domain { get; set; }
    }

    public class creds
    {

        public string _githubUsername { get; set; }
        public string _githubToken { get; set; }
    }

}






