namespace MyVeggieStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblLocation")]
    public partial class tblLocation
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Longitude { get; set; }
    }
}
