using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.Domain
{
    [Table("Deployments")]
    public class Deployments
    {
        [Key]
        public int ID { get; set; }

        public string DEPLOYMENT_NAME {  get; set; }   
        public int CUSTOMER_ID { get; set; }
        public string BASE_GITHUB_URL { get; set; }
        public string DEPLOYMENT_URL { get; set; }

        public string DEPLOYMENT_STATUS { get; set; }

        public string REGION { get; set; }
        public DateTime CREATED_AT { get; set; }
    }
}
