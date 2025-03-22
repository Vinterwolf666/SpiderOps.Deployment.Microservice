using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.Domain
{
    [Table("CustomPipelines")]
    public class CustomPipelines
    {
        [Key]
        public int ID { get; set; } 

        public int CUSTOMER_ID { get; set; }

	    public int TEMPLATE_ID {  get; set; }

        public string? CLUSTER_NAME {  get; set; } 

	    public string? ArtifactRegistry {  get; set; } 

	    public string? REGION {  get; set; }

	    public string? APPNAME {  get; set; } 

	    public DateTime CREATED_AT {  get; set; } 
    }
}
