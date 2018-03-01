using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;


namespace Auction.Models{
    public class AuctionItem : BaseEntity{
        [Key]
        public int AuctionId {get;set;}
        [Required]
        [MinLength(3)]
        public string ProductName{get;set;}
        [Required]
        [MinLength(10)]
        public String Description{get;set;}
        //Set min of 1 somehow
        [Required]
        public DateTime EndDate{get;set;}
        [Required]
        public int UserId{get;set;}
        public User Auctioner{get;set;}
        public int Bid{get;set;}
        
        public String HighestBidder{get;set;}
        public Double Remains{get;set;}
        
        

        

    public AuctionItem()
    {
        
    }
    }
}