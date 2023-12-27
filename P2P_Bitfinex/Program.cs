using Grpc.Core;
using P2P_Bitfinex;

namespace AuctionClient
{
    class Program
    {
        static void Main()
        {
            const int Port = 50051;
            Server server = new Server
            {
                Services = { AuctionService.BindService(new AuctionServer()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine($"TYhe server listening on port {Port}");
            Console.WriteLine("Press any key to create the client and start the bids...");
            Console.ReadKey();

            Channel channel = new Channel("localhost:50051", ChannelCredentials.Insecure);
            var client = new AuctionService.AuctionServiceClient(channel);
                     
            // evaluations example : Create Auction
            var createAuctionResponse = client.CreateAuction(new AuctionRequest
            {
                Seller = "Client#1",
                Item = "Pic#1",
                StartingPrice = 75
            });

            Console.WriteLine($" Auction cerated with ID: {createAuctionResponse.AuctionId}");

            // evaluations example : Place Bid
            var placeBidResponse = client.PlaceBid(new BidRequest
            {
                AuctionId = createAuctionResponse.AuctionId,
                Bidder = "Client#2",
                Amount = 80
            });

            Console.WriteLine($" Bid placement status: {placeBidResponse.Success}");

            channel.ShutdownAsync().Wait();
        }
    }
}