using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment.Microservice.Domain
{
    [Table("AI_REFINEMENT")]
    public class AIRefinement
    {
        [Key]
        public int ID { get; set; }

        public int PIPELINE_ID { get; set; }

        public byte[] REFINED_YAML_FILE { get; set; }

        public DateTime CREATED_AT { get; set; }
    }
}
