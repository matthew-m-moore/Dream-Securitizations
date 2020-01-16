using System;

namespace Dream.IO.Database.Entities
{
    public class VectorParentEntity
    {
        public int VectorParentId { get; set; }
        public DateTime? CutOffDate { get; set; }
        public bool IsFlatVector { get; set; }
        public string VectorParentDescription { get; set; }
    }
}
