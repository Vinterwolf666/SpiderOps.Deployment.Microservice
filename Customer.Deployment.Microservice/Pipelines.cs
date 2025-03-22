using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.Domain
{
    [Table("Pipelines")]
    public class Pipelines
    {
        [Key]
        public int ID { get; set; }
        public int CUSTOMER_ID { get; set; }        

        public byte[] YAML_FILE { get; set; }

        public string PROJECT_LANGUAGE { get; set; }

        public string CLOUD {  get; set; }

        public string CLUSTER_NAME { get; set; }

        public string ArtifactRegistryName { get; set; }

        public string REGION { get; set; }

        public string APPNAME { get; set; }

        public string DESCRIPTIONS { get; set; }

        public DateTime? CREATED_AT { get; set; }
    }
}
