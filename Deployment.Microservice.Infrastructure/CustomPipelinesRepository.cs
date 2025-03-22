using Deployment.Microservice.APP;
using Deployment.Microservice.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


using Google.Api.Gax.Grpc;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Container.V1;
using Grpc.Auth;
using k8s;
using LibGit2Sharp;

using Sodium;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;

using System.Threading.Tasks;


namespace Deployment.Microservice.Infrastructure
{
    public class CustomPipelinesRepository : ICustomerPipelinesRepository
    {
        private readonly CustomPipelinesDBContext _dbContext;
        private readonly IPipelinesRepository pipelinesRepository;
        public CustomPipelinesRepository(CustomPipelinesDBContext dbContext, IPipelinesRepository p)
        {

            _dbContext = dbContext;
            pipelinesRepository = p;
        }


        public async Task<FileContentResult> UpdatePipeline(int customer_id, int template_id, string cluster_name, string ArtifactRegistry, string REGION, string appname)
        {
            var pipe = new CustomPipelines()
            {
                CUSTOMER_ID = customer_id,
                TEMPLATE_ID = template_id,
                CLUSTER_NAME = cluster_name,
                ArtifactRegistry = ArtifactRegistry,
                REGION = REGION,
                APPNAME = appname,
                CREATED_AT = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"))
            };

         
            var fileContentResult = await pipelinesRepository.DownloadPipeline(template_id, "build");

            if (fileContentResult?.FileContents == null || fileContentResult.FileContents.Length == 0)
            {
                throw new FileNotFoundException("Invalid YAML file.");
            }

            string content = Encoding.UTF8.GetString(fileContentResult.FileContents);
            content = content.Replace("{{CUSTOMER_ID}}", customer_id.ToString())
                             .Replace("{{CLUSTER_NAME}}", cluster_name)
                             .Replace("{{ARTIFACT_REGISTRY}}", ArtifactRegistry)
                             .Replace("{{REGION}}", REGION)
                             .Replace("{{APP_NAME}}", appname);

            byte[] fileBytes = Encoding.UTF8.GetBytes(content);

            _dbContext.CustomPipelinesDomain.Add(pipe);
            await _dbContext.SaveChangesAsync();

            return new FileContentResult(fileBytes, "text/yaml")
            {
                FileDownloadName = "build.yaml"
            };
        }




        public async Task<List<object>> dropGitHub()
        {
            var result = await DropFiles();
            return result ?? new List<object>(); 
        }



        static async Task<List<object>> DropFiles()
        {
            string dropboxUrl = "https://www.dropbox.com/scl/fi/fgil2iq71q9hzdzaf9f4y/output.json?rlkey=3b8yb18ml5rk8kucf5bwuhit1&st=nbnhbo7v&dl=1";

            try
            {
                using (HttpClient httpClient = new HttpClient()) // Instancia local
                {
                    HttpResponseMessage response = await httpClient.GetAsync(dropboxUrl);
                    response.EnsureSuccessStatusCode();
                    string jsonContent = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"📝 JSON original: {jsonContent}"); // Debug

                    var parsedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);

                    if (parsedData != null)
                    {
                        return new List<object> { parsedData };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }

            return new List<object>();
        }




        public async Task<List<object>> dropSecrets()
        {
            var result = await DropFile();
            return result ?? new List<object>(); 
        }

        private static readonly HttpClient _httpClient = new HttpClient();

        static async Task<List<object>> DropFile()
        {
            string dropboxUrl = "https://www.dropbox.com/scl/fi/9hm7gqwaaymm1uo1u5n5n/spiderops-fcc51c76307a.json?rlkey=4yvewm2zsdk9kjki0p6dqsu4y&st=hl2ul8z9&dl=1";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(dropboxUrl);
                response.EnsureSuccessStatusCode();
                string jsonContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📜 JSON original: {jsonContent}"); 

                var parsedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);

                if (parsedData != null)
                {
                    return new List<object> { parsedData };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }

            return new List<object>();
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
