using System;
using System.Collections.Generic;

namespace Dream.WebApp.Models
{
    public class SecuritizationDataSetModel
    {
        public int SecuritizationDataSetId { get; set; }
        public string SecuritizationDataSetDescription { get; set; }
        public string SecuritizationDataSetComment { get; set; }
        public string SecuritizationOwner { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRescuritization { get; set; }
        public bool IsTemplate { get; set; }
        public bool IsReadOnly { get; set; }
        public List<SecuritizationVersionModel> SecuritizationVersions { get; set; }
    }
}