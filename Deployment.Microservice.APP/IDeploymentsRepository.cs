using Deployment.Microservice.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.APP
{
    public interface IDeploymentsRepository
    {
        List<Deployments> AllDeploymentsById(int id);

        Task<string> SaveDeployment(Deployments d);

        Task<string> DeleteDeployment(int id);


        List<Deployments> GetDeploymentById(int id);



    }
}
