using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishListik.Models;

namespace WishListik.Models
{
    public class FriendRequest
    {
        public int FriendRequestId { get; set; }
        public int RequesterId { get; set; }
        public int RequesteeId { get; set; }

        // Navigation properties
        public User? Requester { get; set; }
        public User? Requestee { get; set; }
    }

}
