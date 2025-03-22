using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Microservice.Domain
{
    [Table("CustomerDeploymentInfrastructure")]
    public class GCPInfrastructure
    {
        [Key]
        public int DEPLOYMENT_ID {  get; set; }
        public int CUSTOMER_ID { get; set; }

        public string PROJECT_LANGUAJE { get; set; }

        public string TEMPLATE_USED { get; set; }

        public DateTime CREATED_AT { get; set; }
    }
}
