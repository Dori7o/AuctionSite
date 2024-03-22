using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using AuctionService.Entities;

namespace AuctionService;

[Table("Items")]
public class Item
{
    public Guid Id { set; get; }

    public string Make { set; get; }

    public string Model { set; get; }

    public int Year { set; get; }

    public string Color { set; get; }

    public int Mileage {set;get;}

    public string ImageUrl {set;get;}

    // nav properties

    public Auction Auction {set;get;}
    public Guid AuctionId {set; get;}
}
