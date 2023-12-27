using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace P2P_Bitfinex
{
    class AuctionClient : AuctionService.AuctionServiceBase
    {
        private static int auctionIdCounter = 1;
        private static Dictionary<int, AuctionDetails> auctions = new Dictionary<int, AuctionDetails>();

        public override Task<AuctionResponse> CreateAuction(AuctionRequest request, ServerCallContext context)
        {
            int auctionId = auctionIdCounter++;
            auctions[auctionId] = new AuctionDetails
            {
                Seller = request.Seller,
                Item = request.Item,
                StartingPrice = request.StartingPrice
            };

            Console.WriteLine($"Auction created with ID: {auctionId}");
            return Task.FromResult(new AuctionResponse { AuctionId = auctionId });
        }

        public override Task<BidResponse> PlaceBid(BidRequest request, ServerCallContext context)
        {
            int auctionId = request.AuctionId;
            if (auctions.ContainsKey(auctionId))
            {
                AuctionDetails auction = auctions[auctionId];
                if (request.Amount > auction.StartingPrice)
                {
                    Console.WriteLine($"{request.Bidder} placed a bid of {request.Amount} on Auction {auctionId}");
                    return Task.FromResult(new BidResponse { Success = true });
                }
                else
                {
                    Console.WriteLine($"Bid of {request.Amount} is too low for Auction {auctionId}");
                }
            }

            return Task.FromResult(new BidResponse { Success = false });
        }
    }
}
