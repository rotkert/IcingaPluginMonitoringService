using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringService
{
    [Table ("CheckResult")]
    public class CheckResult
    {
        [Key]
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public double Result { get; set; }
        public long Timestamp { get; set; }
        public double Crtitical { get; set; }
        public bool HasExceeded { get; set; } 
    }
}
