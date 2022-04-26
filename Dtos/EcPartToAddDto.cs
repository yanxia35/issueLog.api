using System;

namespace IssueLog.API.Models
{
    public class EcPartToAddDto
    {

        public string PartNo { get; set; }
        public double Quantity { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string SupplierName { get; set; }
        public string IssueNo { get; set; }
        public string ProjectNo { get; set; }

    }
}