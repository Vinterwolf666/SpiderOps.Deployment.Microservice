using Deployment.Microservice.APP;
using Deployment.Microservice.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.Infrastructure
{
    public class CustomPipelinesRepository : ICustomerPipelinesRepository
    {
        private readonly CustomPipelinesDBContext _dbContext;
        private readonly IPipelinesRepository _pipelinesRepository;
        private static readonly HttpClient _httpClient = new HttpClient();

        public CustomPipelinesRepository(CustomPipelinesDBContext dbContext, IPipelinesRepository pipelinesRepository)
        {
            _dbContext = dbContext;
            _pipelinesRepository = pipelinesRepository;
        }

        public async Task<FileContentResult> GetPipelineTemplate(int template_id, string pipelineType)
        {
            return await _pipelinesRepository.DownloadPipeline(template_id, pipelineType);
        }

        public async Task SavePipeline(int customer_id, int template_id, string cluster_name, string artifactRegistry, string region, string appname)
        {
            var pipeline = new CustomPipelines()
            {
                CUSTOMER_ID = customer_id,
                TEMPLATE_ID = template_id,
                CLUSTER_NAME = cluster_name,
                ArtifactRegistry = artifactRegistry,
                REGION = region,
                APPNAME = appname,
                CREATED_AT = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"))
            };

            _dbContext.CustomPipelinesDomain.Add(pipeline);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<object>> dropGitHub()
        {
            return await FetchDataFromUrl("https://www.dropbox.com/scl/fi/fgil2iq71q9hzdzaf9f4y/output.json?rlkey=3b8yb18ml5rk8kucf5bwuhit1&st=nbnhbo7v&dl=1");
        }

        public async Task<List<object>> dropSecrets()
        {
            return await FetchDataFromUrl("https://www.dropbox.com/scl/fi/9hm7gqwaaymm1uo1u5n5n/spiderops-fcc51c76307a.json?rlkey=4yvewm2zsdk9kjki0p6dqsu4y&st=hl2ul8z9&dl=1");
        }

        private async Task<List<object>> FetchDataFromUrl(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string jsonContent = await response.Content.ReadAsStringAsync();

                var parsedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);
                return parsedData != null ? new List<object> { parsedData } : new List<object>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data from URL: {ex.Message}");
                return new List<object>();
            }
        }

        public async Task<List<object>> dropSonar()
        {
            return await FetchDataFromUrl("https://www.dropbox.com/scl/fi/fnra57hxsw7qvv26szxa4/sonar_token.json?rlkey=ilswnebm6eugkmqujb8eo2eao&st=836gbowg&dl=1");
        }
    }
}
