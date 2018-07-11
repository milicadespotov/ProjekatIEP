using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace WebApplication3
{
    public class RefreshHub : Hub
    {
        public void Refresh(Guid AuctionId, int? AuctionPrice, string LastBidder)
        {
            Clients.All.refresh(AuctionId, AuctionPrice, LastBidder);
        }
    }
}