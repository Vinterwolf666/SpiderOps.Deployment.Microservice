using Deployment.Microservice.APP;
using Deployment.Microservice.Domain;
using Google.Api.Gax.Grpc;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Container.V1;
using Grpc.Auth;
using k8s;
using LibGit2Sharp;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sodium;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
using Infrastructure.Microservice.Infrastructure;
using Infrastructure.Microservice.APP;

namespace Deployment.Microservice.Service
{
    public class DeploymentsServices : IDeploymentsServices
    {   
        
        private readonly IDeploymentsRepository _dr;
        private readonly IPipelinesRepository _pipe;
        private readonly IGCPInfrastructureServices _cpRepository;
        private readonly ICustomPipelinesServices _pipelines;

        public DeploymentsServices(IPipelinesRepository pipe, IGCPInfrastructureServices gc, ICustomPipelinesServices pipelines, IDeploymentsRepository dr)
        {

            
            _pipe = pipe;
            _cpRepository = gc;
            _pipelines = pipelines;
            _dr = dr;
        }

        public List<Deployments> AllDeploymentsById(int id)
        {
            var result = _dr.AllDeploymentsById(id);

            return result;
        }

        public async Task<string> NewDeployment(string BASE_GITHUB_URL, int CUSTOMER_ID, string REGION, int template_id, string cluster_name, string ArtifactRegistry, string appname, string language, string CLUSTER_NAME, string ARTIFACT_REGISTRY_REGION, string ARTIFACT_REGISTRY, int customer_id)
        {
            var repoName = $"repo-{Guid.NewGuid()}";
            var repo = new Deployments();


            var responseBodys = string.Empty;


            var responses = await _pipelines.dropGitHub();


            Console.WriteLine("[SUCCESS] GitHub drop operation completed.");


            responseBodys = System.Text.Json.JsonSerializer.Serialize(responses);
            Console.WriteLine($"Response Data: {responseBodys}");


            var jsonDataLists = JsonConvert.DeserializeObject<List<creds>>(responseBodys);


            var jsonDatas = jsonDataLists?.FirstOrDefault();

            Console.WriteLine(jsonDataLists?.FirstOrDefault());


            string _githubUsername = jsonDatas?._githubUsername ?? "";
            string _githubToken = jsonDatas?._githubToken ?? "";


            Console.WriteLine(responseBodys);
            System.Text.Json.JsonSerializer.Serialize(responses);

            repo.CUSTOMER_ID = CUSTOMER_ID;
            repo.REGION = REGION;
            repo.BASE_GITHUB_URL = BASE_GITHUB_URL;
            repo.DEPLOYMENT_URL = $"https://github.com/Vinterwolf666/{repoName}.git";
            repo.DEPLOYMENT_NAME = repoName;
            repo.CREATED_AT = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"));
            repo.DEPLOYMENT_STATUS = "Active";
            repo.APPNAME = appname;
            repo.CLUSTER_NAME = cluster_name;
            string localPath = Path.Combine(Path.GetTempPath(), repo.DEPLOYMENT_NAME);
            Console.WriteLine($"[INFO] Local Path: {localPath}");

            try
            {
                if (string.IsNullOrEmpty(BASE_GITHUB_URL))
                    return "Error: BASE_GITHUB_URL es null o vacío.";

                var cloneOptions = new CloneOptions
                {
                    FetchOptions =
            {
                CredentialsProvider = (_url, _user, _cred) =>
                    new UsernamePasswordCredentials { Username = _githubUsername, Password = _githubToken }
            }
                };

                Console.WriteLine("[INFO] Clonning repository...");
                Repository.Clone(BASE_GITHUB_URL, localPath, cloneOptions);
                Console.WriteLine("[SUCCESS] Repository cloned.");

                using (var repository = new Repository(localPath))
                {

                    repository.Network.Remotes.Remove("origin");
                    Console.WriteLine("[INFO] Removing the original remote.");


                    var newRepoUrl = await CreateNewGitHubRepository(repoName);
                    Console.WriteLine($"[SUCCESS] New repository create: {newRepoUrl}");


                    repository.Network.Remotes.Add("origin", newRepoUrl);
                    Console.WriteLine("[SUCCESS] New remote add.");


                    var currentBranch = repository.Head.FriendlyName;
                    Console.WriteLine($"[INFO] The actual branch is '{currentBranch}'.");

                    if (currentBranch != "main")
                    {
                        var mainBranch = repository.CreateBranch("main", repository.Head.Tip);
                        Commands.Checkout(repository, mainBranch);
                        Console.WriteLine("[INFO] Branch 'main' created and selected.");
                    }


                    string infrasPath = Path.Combine(localPath, "infras");
                    string githubPath = Path.Combine(localPath, ".github");
                    string workflowsPath = Path.Combine(githubPath, "workflows");

                    Directory.CreateDirectory(infrasPath);
                    Directory.CreateDirectory(workflowsPath);
                    Console.WriteLine("[SUCCESS] Directories created.");


                    var fileContentResult = await _pipe.DownloadPipeline(10, "resources");

                    if (fileContentResult?.FileContents != null)
                    {
                        string resourcesPath = Path.Combine(localPath, "resources.yaml");


                        await File.WriteAllBytesAsync(resourcesPath, fileContentResult.FileContents);

                        Console.WriteLine("[SUCCESS] resources.yaml file saved locally.");
                    }
                    else
                    {
                        Console.WriteLine("[ERROR] Failed to retrieve YAML file.");
                    }



                    var fileContentResult1 = await _cpRepository.CreateNewInfrastructure(
                        CUSTOMER_ID, language, "main-template", 1, REGION, CLUSTER_NAME,
                        ARTIFACT_REGISTRY_REGION, ARTIFACT_REGISTRY, appname
                    );

                    if (fileContentResult1?.FileContents != null)
                    {
                        string mainTfPath = Path.Combine(infrasPath, "main.tf");


                        await File.WriteAllBytesAsync(mainTfPath, fileContentResult1.FileContents);

                        Console.WriteLine("[SUCCESS] main.tf file saved locally.");
                    }
                    else
                    {
                        Console.WriteLine("[ERROR] Failed to create main.tf file.");
                    }



                    var fileContentResult2 = await _cpRepository.CreateNewInfrastructure(
                        CUSTOMER_ID, language, "variables-template", 2, REGION, CLUSTER_NAME,
                        ARTIFACT_REGISTRY_REGION, ARTIFACT_REGISTRY, appname
                    );

                    if (fileContentResult2?.FileContents != null)
                    {
                        string variablesTfPath = Path.Combine(infrasPath, "variables.tf");


                        await File.WriteAllBytesAsync(variablesTfPath, fileContentResult2.FileContents);

                        Console.WriteLine("[SUCCESS] variables.tf file saved locally.");
                    }
                    else
                    {
                        Console.WriteLine("[ERROR] Failed to create variables.tf file.");
                    }



                    var fileContentResult3 = await _pipelines.UpdatePipeline(
                        CUSTOMER_ID, template_id, CLUSTER_NAME, ArtifactRegistry, REGION, appname
                    );

                    if (fileContentResult3?.FileContents != null)
                    {
                        string buildPath = Path.Combine(workflowsPath, "build.yaml");


                        await File.WriteAllBytesAsync(buildPath, fileContentResult3.FileContents);

                        Console.WriteLine("[SUCCESS] build.yaml file saved locally.");
                    }
                    else
                    {
                        Console.WriteLine("[ERROR] Failed to create build.yaml file.");
                    }



                    Commands.Stage(repository, "*");
                    var author = new Signature("your name", "vinterwolf666@gmailcom", DateTimeOffset.Now);
                    repository.Commit("Adding the new archives", author, author);
                    Console.WriteLine("[SUCCESS] Changes done.");


                    var pushOptions = new PushOptions
                    {
                        CredentialsProvider = (_url, _user, _cred) =>
                            new UsernamePasswordCredentials { Username = _githubUsername, Password = _githubToken }
                    };
                    repository.Network.Push(repository.Network.Remotes["origin"], @"refs/heads/main", pushOptions);
                    Console.WriteLine("[SUCCESS] Changes ready in the new repo.");
                }


                Console.WriteLine("[INFO] Waiting 5 seconds before adding the secrets...");
                await Task.Delay(5000);




                string owner = "Vinterwolf666";


                var response1 = await _pipelines.dropSecrets();
                var response2 = await _pipelines.dropSonar();

                Console.WriteLine($"[DEBUG] Raw response1: {System.Text.Json.JsonSerializer.Serialize(response1)}");

                Console.WriteLine($"Type of response1: {response1.GetType()}");


                Console.WriteLine("[SUCCESS] Secrets retrieved successfully.");


                var responseBody = System.Text.Json.JsonSerializer.Serialize(response1);
                var responseBody2 = System.Text.Json.JsonSerializer.Serialize(response2);
                Console.WriteLine($"Response Data: {responseBody}");






                var jsonDataList = JsonConvert.DeserializeObject<List<ApiResponse>>(responseBody);
                var jsonDataList2 = JsonConvert.DeserializeObject<List<ApiResponse2>>(responseBody2);
                var jsonData = jsonDataList?.FirstOrDefault();
                var jsonData2 = jsonDataList2?.FirstOrDefault();

                Console.WriteLine(jsonData);



                Console.WriteLine(jsonDataList?.FirstOrDefault());


                await AddRepoSecretAsync(owner, repoName, "GOOGLE_CREDENTIALS",
         $@"{{
  ""type"": ""{jsonData?.type}"",
  ""project_id"": ""{jsonData?.project_id}"",
  ""private_key_id"": ""{jsonData?.private_key_id}"",
  ""private_key"": ""{jsonData?.private_key?.Replace("\n", "\\n")}"",
  ""client_email"": ""{jsonData?.client_email}"",
  ""client_id"": ""{jsonData?.client_id}"",
  ""auth_uri"": ""{jsonData?.auth_uri}"",
  ""token_uri"": ""{jsonData?.token_uri}"",
  ""auth_provider_x509_cert_url"": ""{jsonData?.auth_provider_x509_cert_url}"",
  ""client_x509_cert_url"": ""{jsonData?.client_x509_cert_url}"",
  ""universe_domain"": ""{jsonData?.universe_domain}""
}}");
                await AddRepoSecretAsync(owner, repoName, "GOOGLE_PROJECT_ID", "spiderops");
                await AddRepoSecretAsync(owner, repoName, "SONAR_TOKEN", $@"{{""SONAR_TOKEN"":""{ jsonData2?.SONAR_TOKEN}}}");
                Console.WriteLine("[SUCCESS] Secrets saved.");


                var apiResponse = new
                {
                    type = jsonData?.type,
                    project_id = jsonData?.project_id,
                    private_key_id = jsonData?.private_key_id,
                    private_key = jsonData?.private_key,
                    client_email = jsonData?.client_email,
                    client_id = jsonData?.client_id,
                    auth_uri = jsonData?.auth_uri,
                    token_uri = jsonData?.token_uri,
                    auth_provider_x509_cert_url = jsonData?.auth_provider_x509_cert_url,
                    client_x509_cert_url = jsonData?.client_x509_cert_url,
                    universe_domain = jsonData?.universe_domain


                };


                var apiResponse2 = new
                {
                    SONAR_TOKEN = jsonData2?.SONAR_TOKEN,
                    
                };

                Console.WriteLine($"api response: {apiResponse}");
                Console.WriteLine($"api response: {apiResponse2}");

                Console.WriteLine("[INFO] Saving in the database...");

                Console.WriteLine("[SUCCESS] Saving in the database.");



                
                await _dr.SaveDeployment(repo);
                

                return $"Repository successfully created at: {repo.DEPLOYMENT_URL}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }





        private async Task<string> CreateNewGitHubRepository(string repoName)
        {

            var responses = await _pipelines.dropGitHub();


            string responseBodys = string.Empty;

            if (responses != null && responses.Count > 0)
            {
                Console.WriteLine("[SUCCESS] GitHub drop operation completed.");


                responseBodys = System.Text.Json.JsonSerializer.Serialize(responses);
                Console.WriteLine($"Response Data: {responseBodys}");
            }
            else
            {
                Console.WriteLine("[ERROR] GitHub drop operation failed or returned no data.");
                return string.Empty;
            }


            var jsonDataLists = JsonConvert.DeserializeObject<List<creds>>(responseBodys);
            var jsonDatas = jsonDataLists?.FirstOrDefault();

            if (jsonDatas == null)
            {
                Console.WriteLine("[ERROR] No valid credentials found.");
                return string.Empty;
            }

            Console.WriteLine(jsonDatas);

            string _githubUsername = jsonDatas?._githubUsername ?? "";
            string _githubToken = jsonDatas?._githubToken ?? "";

            if (string.IsNullOrEmpty(_githubToken))
            {
                Console.WriteLine("[ERROR] GitHub token is missing.");
                return string.Empty;
            }

            var createRepoUrl = "https://api.github.com/user/repos";
            var payload = new { name = repoName, @private = false };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MyApp", "1.0"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _githubToken);

                var response = await client.PostAsJsonAsync(createRepoUrl, payload);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(jsonResponse);
                return jsonObject["html_url"]?.ToString() ?? string.Empty;
            }
        }







        private async Task DownloadAndSaveFile(string url, string filePath)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                await File.WriteAllTextAsync(filePath, content);
            }
        }







        private async Task<(string base64Key, string keyId)> GetRepoPublicKeyAsync(string owner, string repoName)
        {

            var responses = await _pipelines.dropGitHub();


            string responseBodys = string.Empty;

            if (responses != null && responses.Count > 0)
            {
                Console.WriteLine("[SUCCESS] GitHub drop operation completed.");


                responseBodys = System.Text.Json.JsonSerializer.Serialize(responses);
                Console.WriteLine($"Response Data: {responseBodys}");
            }
            else
            {
                Console.WriteLine("[ERROR] GitHub drop operation failed or returned no data.");
                return (string.Empty, string.Empty);
            }


            var jsonDataLists = JsonConvert.DeserializeObject<List<creds>>(responseBodys);
            var jsonDatas = jsonDataLists?.FirstOrDefault();

            if (jsonDatas == null)
            {
                Console.WriteLine("[ERROR] No valid credentials found.");
                return (string.Empty, string.Empty);
            }

            Console.WriteLine(jsonDatas);

            string _githubUsername = jsonDatas?._githubUsername ?? "";
            string _githubToken = jsonDatas?._githubToken ?? "";

            if (string.IsNullOrEmpty(_githubToken))
            {
                Console.WriteLine("[ERROR] GitHub token is missing.");
                return (string.Empty, string.Empty);
            }

            string url = $"https://api.github.com/repos/{owner}/{repoName}/actions/secrets/public-key";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MyApp", "1.0"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _githubToken);

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                dynamic keyInfo = JsonConvert.DeserializeObject(json);
                string base64Key = keyInfo.key;
                string keyId = keyInfo.key_id;

                return (base64Key, keyId);
            }
        }



        private string EncryptSecret(string plainText, string base64PublicKey)
        {

            byte[] publicKey = Convert.FromBase64String(base64PublicKey);


            byte[] secretBytes = Encoding.UTF8.GetBytes(plainText);


            byte[] sealedBox = SealedPublicKeyBox.Create(secretBytes, publicKey);


            return Convert.ToBase64String(sealedBox);
        }


        private async Task AddRepoSecretAsync(string owner, string repoName, string secretName, string secretValue)
        {

            var responses = await _pipelines.dropGitHub();


            if (responses == null || responses.Count == 0)
            {
                Console.WriteLine("[ERROR] GitHub drop operation failed or returned no data.");
                return;
            }

            Console.WriteLine("[SUCCESS] GitHub drop operation completed.");


            string responseBodys = System.Text.Json.JsonSerializer.Serialize(responses);
            Console.WriteLine($"Response Data: {responseBodys}");


            var jsonDataLists = JsonConvert.DeserializeObject<List<creds>>(responseBodys);
            var jsonDatas = jsonDataLists?.FirstOrDefault();

            if (jsonDatas == null)
            {
                Console.WriteLine("[ERROR] No valid credentials found.");
                return;
            }

            Console.WriteLine(jsonDatas);

            string _githubUsername = jsonDatas?._githubUsername ?? "";
            string _githubToken = jsonDatas?._githubToken ?? "";

            if (string.IsNullOrEmpty(_githubToken))
            {
                Console.WriteLine("[ERROR] GitHub token is missing.");
                return;
            }


            var (base64Key, keyId) = await GetRepoPublicKeyAsync(owner, repoName);

            if (string.IsNullOrEmpty(base64Key) || string.IsNullOrEmpty(keyId))
            {
                Console.WriteLine("[ERROR] Failed to retrieve GitHub repository public key.");
                return;
            }


            string encryptedValue = EncryptSecret(secretValue, base64Key);


            string putUrl = $"https://api.github.com/repos/{owner}/{repoName}/actions/secrets/{secretName}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MyApp", "1.0"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _githubToken);

                var payload = new
                {
                    encrypted_value = encryptedValue,
                    key_id = keyId
                };

                var response = await client.PutAsJsonAsync(putUrl, payload);
                response.EnsureSuccessStatusCode();

                Console.WriteLine($"[SUCCESS] Secret '{secretName}' added to the repository.");
            }
        }




        public async Task<string> TriggerPipelineCommit(string repoUrl)
        {

            var responses = await _pipelines.dropGitHub();


            if (responses == null || responses.Count == 0)
            {
                Console.WriteLine("[ERROR] GitHub drop operation failed or returned no data.");
                return "Error: No valid credentials found.";
            }

            Console.WriteLine("[SUCCESS] GitHub drop operation completed.");


            string responseBodys = System.Text.Json.JsonSerializer.Serialize(responses);
            Console.WriteLine($"Response Data: {responseBodys}");


            var jsonDataLists = JsonConvert.DeserializeObject<List<creds>>(responseBodys);
            var jsonDatas = jsonDataLists?.FirstOrDefault();

            if (jsonDatas == null)
            {
                Console.WriteLine("[ERROR] No valid credentials found.");
                return "Error: No valid credentials found.";
            }

            Console.WriteLine(jsonDatas);

            string _githubUsername = jsonDatas?._githubUsername ?? "";
            string _githubToken = jsonDatas?._githubToken ?? "";

            if (string.IsNullOrEmpty(_githubToken))
            {
                Console.WriteLine("[ERROR] GitHub token is missing.");
                return "Error: GitHub token is missing.";
            }

            // Definir la ruta temporal del repositorio
            string localRepoPath = Path.Combine(Path.GetTempPath(), "trigger-" + Guid.NewGuid().ToString());
            Console.WriteLine($"[INFO] Clonando el repositorio de {repoUrl} en {localRepoPath}...");

            try
            {
                var cloneOptions = new CloneOptions
                {
                    FetchOptions =
            {
                CredentialsProvider = (_url, _user, _cred) =>
                    new UsernamePasswordCredentials { Username = _githubUsername, Password = _githubToken }
            }
                };

                // Clonar el repositorio
                Repository.Clone(repoUrl, localRepoPath, cloneOptions);
                Console.WriteLine("[SUCCESS] Repositorio clonado temporalmente.");

                using (var repository = new Repository(localRepoPath))
                {
                    // Verificar la rama actual
                    var currentBranch = repository.Head.FriendlyName;
                    Console.WriteLine($"[INFO] Rama actual: {currentBranch}");

                    if (currentBranch != "main")
                    {
                        var mainBranch = repository.CreateBranch("main", repository.Head.Tip);
                        Commands.Checkout(repository, mainBranch);
                        Console.WriteLine("[INFO] Cambiada a la rama 'main'.");
                    }

                    // Crear un archivo trigger.txt para activar el pipeline
                    string triggerFilePath = Path.Combine(localRepoPath, "trigger.txt");
                    File.WriteAllText(triggerFilePath, $"Pipeline trigger commit at {DateTime.UtcNow}");
                    Console.WriteLine("[INFO] Archivo trigger.txt creado.");

                    // Realizar commit
                    Commands.Stage(repository, "trigger.txt");
                    var author = new Signature("Trigger Bot", "trigger@example.com", DateTimeOffset.Now);
                    repository.Commit("Trigger pipeline commit", author, author);
                    Console.WriteLine("[SUCCESS] Commit realizado.");

                    // Realizar push del commit
                    var pushOptions = new PushOptions
                    {
                        CredentialsProvider = (_url, _user, _cred) =>
                            new UsernamePasswordCredentials { Username = _githubUsername, Password = _githubToken }
                    };
                    repository.Network.Push(repository.Network.Remotes["origin"], "refs/heads/main", pushOptions);
                    Console.WriteLine("[SUCCESS] Push del commit trigger realizado.");
                }

                return "Trigger commit successful.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }



        public async Task<string> RemoveDeploymentID(int id)
        {

            return  await _dr.DeleteDeployment(id);
                    

        }



    }




}

