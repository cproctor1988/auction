using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Auction.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace Auction.Controllers
{
    public class HomeController : Controller
    {
        private AuctionContext _context;
        public HomeController(AuctionContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }
        [HttpPost]
        [Route("Register")]
        public IActionResult Register(RegisterViewModel model){
            if(ModelState.IsValid)
            {
                if(_context.users.SingleOrDefault(user=>user.UserName==model.UserName)==null){
                    
                User NewUser = new User{
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    Password = model.Password
                };
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                NewUser.Password = Hasher.HashPassword(NewUser, NewUser.Password);
                _context.Add(NewUser);
                _context.SaveChanges();
                User CurrentUser  =  _context.users.SingleOrDefault(user => user.UserName == NewUser.UserName);
                HttpContext.Session.SetString("loggedin","yes");
                HttpContext.Session.SetString("username",NewUser.UserName);
                return RedirectToAction("Home");
                }else{
                    ViewBag.error = "UserName Already in use";
                   return View("Index"); 
                } 
            }else{
                return View("Index");
            }
            
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(string UserName,string Password){
        User CurrentUser  =  _context.users.SingleOrDefault(user => user.UserName == UserName);
        var Hasher = new PasswordHasher<User>(); 
        if(CurrentUser == null){
            ViewBag.error = "No user with that Username!";
            return View("Index");
        }else if(0 != Hasher.VerifyHashedPassword(CurrentUser,CurrentUser.Password, Password)){
            HttpContext.Session.SetString("loggedin","yes");
            HttpContext.Session.SetString("username",CurrentUser.UserName);
            return RedirectToAction("Home");
            
        }else{
            ViewBag.error = "Password does not match!";
            return View("Index");
        }
        }
        [HttpGetAttribute]
        [Route("Home")]
        public IActionResult Home(){
        if(HttpContext.Session.GetString("loggedin") == "yes"){    
            User user =  _context.users.SingleOrDefault(u => u.UserName == HttpContext.Session.GetString("username"));
            List<AuctionItem> allAuctions = _context.auctions.Include(w=> w.Auctioner).OrderBy(a => a.EndDate).ToList();
            foreach(var item in allAuctions){
              item.Remains = Math.Round((item.EndDate - item.CreatedAt).TotalDays, 2);
              _context.SaveChanges();
          
            }
            
            ViewBag.FirstName = user.FirstName;
            ViewBag.userId = user.userId;
            ViewBag.error = TempData["error"];
            ViewBag.Wallet = user.Wallet;
            return View("Home",allAuctions);}// pass all auctions through
            else{
                return RedirectToAction("index");
            }
        }
        [HttpGetAttribute]
        [Route("New/{userId}")]
        public IActionResult PlanActivity(int userId){
            
            ViewBag.userId = userId;
            return View("PlanAuction");
        }
        [HttpPostAttribute]
        [Route("CreateAuction")]
        public IActionResult CreateAuction(AuctionItem model){
            User CurrentUser = _context.users.SingleOrDefault(user => user.userId == model.UserId);
            model.Remains = 1;
            model.Auctioner = CurrentUser;
            if(ModelState.IsValid){
            _context.auctions.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Home");}
            else{
                ViewBag.userId = CurrentUser.userId;
                return View("PlanAuction");
            }
        }
        [HttpGetAttribute]
        [Route("Auction/{AuctionId}")]
        public IActionResult Auction(int AuctionId){
            AuctionItem ViewAuction = _context.auctions.Include(u=> u.Auctioner).SingleOrDefault(activity=> activity.AuctionId==AuctionId);
            ViewAuction.Remains = Math.Round((ViewAuction.EndDate - ViewAuction.CreatedAt).TotalDays, 2);;
            //ViewBag.user = find from session username;
            
            return View("Auction",ViewAuction);
        }
        [HttpPost]
        [Route("Auction/Bid")]
        public IActionResult AuctionBid(int Auction1,int BidAmount){
            User User =  _context.users.SingleOrDefault(u => u.UserName == HttpContext.Session.GetString("username"));
            AuctionItem ViewAuction = _context.auctions.SingleOrDefault(activity=> activity.AuctionId==Auction1);
            User LastBidder = _context.users.SingleOrDefault(u => u.UserName == ViewAuction.HighestBidder);
            if(BidAmount> ViewAuction.Bid){
                if(User.Wallet > BidAmount){
                User.Wallet -= BidAmount;
                if(ViewAuction.HighestBidder != null){ 
                LastBidder.Wallet += ViewAuction.Bid;
                }
                ViewAuction.HighestBidder = User.UserName;
                ViewAuction.Bid = BidAmount;
                _context.SaveChanges();
                return RedirectToAction("Home");
                }else{
                    ViewBag.error = "You dont have enough Money!";
                    return View("Auction",ViewAuction);
                }

            }else{
                ViewBag.error = "Need to bid more than current highest bid";
                return View("Auction",ViewAuction);
            }

        
        }
        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(int auction1, int user1){
            User CurrentUser = _context.users.SingleOrDefault(user => user.userId == user1);
            AuctionItem RetrievedWedding = _context.auctions.SingleOrDefault(wedding => wedding.AuctionId == auction1);
            _context.auctions.Remove(RetrievedWedding);
            _context.SaveChanges();
            return RedirectToAction("Home");
        }

        
    }
}
