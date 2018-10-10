using MyVeggieStore.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyVeggieStore.Models.ViewModels.Account
{
    public class UserProfileVM
    {
        public UserProfileVM()
        {

        }

        public UserProfileVM(UserDTO row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            Email = row.Email;
            Username = row.Username;
            Password = row.Password;
            RegisterDate = row.RegisterDate;
            //LocationName = row.LocationName;
            //Latitude = row.Latitude;
            //Longitude = row.Longitude;
        }


        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public DateTime RegisterDate { get; set; }

       // public string LocationName { get; set; }

       // public decimal Latitude { get; set; }

       // public decimal Longitude { get; set; }
    }
}