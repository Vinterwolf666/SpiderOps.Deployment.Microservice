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

namespace Deployment.Microservice.Infrastructure
{
    public class DeploymentsRepository : IDeploymentsRepository
    {
        private readonly DeploymentsDBContext _dbContext;
        

        public DeploymentsRepository(DeploymentsDBContext dbContext)
        {

            _dbContext = dbContext;
  
        }

        public List<Deployments> AllDeploymentsById(int id)
        {
            var result = _dbContext.DeploymentsDomain.ToList();

            return result;
        }

        public async Task<string> DeleteDeployment(int id)
        {
            var _id = await _dbContext.DeploymentsDomain.FindAsync(id);

            if (_id != null)
            {

                _dbContext.DeploymentsDomain.Remove(_id);
                await _dbContext.SaveChangesAsync();

                return "Deployment removed successfully";
            }
            else
            {
                return "Deployment not";
            }
        }

        public List<Deployments> GetDeploymentById(int id)
        {
            var result = _dbContext.DeploymentsDomain.Where(a => a.ID == id).ToList();

            return result;
        }

        public async Task<string> SaveDeployment(Deployments d)
        {
            _dbContext.DeploymentsDomain.Add(d);
            await _dbContext.SaveChangesAsync();

            return "saved";
        }
    }




}

