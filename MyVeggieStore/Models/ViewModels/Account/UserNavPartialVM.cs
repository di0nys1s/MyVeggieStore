using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyVeggieStore.Models.ViewModels.Account
{
    public class UserNavPartialVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegisterDate { get; set; }
        //public string LocationName { get; set; }
        //public decimal Latitude { get; set; }
        //public decimal Longitude { get; set; }
    }
}