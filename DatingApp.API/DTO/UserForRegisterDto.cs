using System.ComponentModel.DataAnnotations;
using System;

namespace DatingApp.API.DTO
{
    public class UserForRegisterDto
    {
        [Required]
        public string UserName {get;set;}

        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="Password should have between 4 and 8 characters")]
        public string Password {get;set;}
        
        [Required]
        public string Gender {get;set;}
        
        [Required]
        public string KnownAs {get;set;}
        
        [Required]
        public DateTime DateOfBirth {get;set;}
        
        [Required]
        public string City {get;set;}
        
        [Required]
        public string Country {get;set;}
        
        public DateTime CreatedDate {get;set;}
        
        public DateTime LastActive {get;set;}
        
        public UserForRegisterDto()
        {
            CreatedDate = DateTime.Now;
            LastActive = DateTime.Now;
        }

    }
}