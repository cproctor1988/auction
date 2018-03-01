using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Auction.Models{
    public class User : BaseEntity{
        [Key]
        public int userId {get;set;}
        public string FirstName {get; set;}
        public string LastName {get; set;}
        
        public string UserName {get; set;}
        
        [DataType(DataType.Password)]
        public string Password {get; set;}

        public int Wallet{get;set;}

        
    public User()
    {
        this.Wallet = 1000;
    }
}
}