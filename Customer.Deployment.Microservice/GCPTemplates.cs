using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Microservice.Domain
{
    [Table("Templates")]
    public class GCPTemplates
    {
        [Key]
        public int ID { get; set; }

        public string TEMPLATE_NAME { get; set; }

        public byte[] TERRAFORM_FILE { get; set; }

        public DateTime CREATED_AT { get; set; }

        public string DESCRIPTIONS { get; set; }
    }
}
