using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionController : ControllerBase
{
    private readonly IMapper _autoMapper;
    private readonly AuctionDbContext _context;

    public AuctionController(IMapper autoMapper, AuctionDbContext context)
    {
        _autoMapper = autoMapper;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
    {
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

        if(!string.IsNullOrEmpty(date))
        {
            query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }

        return await query.ProjectTo<AuctionDto>(_autoMapper.ConfigurationProvider).ToListAsync()  ;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
        {
            return NotFound();
        }
        return _autoMapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = _autoMapper.Map<Auction>(auctionDto);
        // TODO : add current user as a seller;

        auction.Seller = "test";

        _context.Auctions.Add(auction);
        var result = await _context.SaveChangesAsync() > 0;

        if (!result)
        {
            return BadRequest("Could not save in the database");
        }
        return CreatedAtAction(nameof(GetAuctionById),
            new { auction.Id }, _autoMapper.Map<AuctionDto>(auction));

    }
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {   
        Console.WriteLine("hey");
        var auction = await _context.Auctions.Include(i => i.Item).FirstOrDefaultAsync(x => x.Id == id);

        if(auction == null){
            return NotFound();
        }
        // TODO : check seller == username;

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        var result = await _context.SaveChangesAsync() > 0;

        if(!result)
        {
            return BadRequest("Problem saving changes");
        }
        return Ok();

    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction (Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);

        if(auction == null)
        {
            return NotFound();
        }
        _context.Auctions.Remove(auction);

        var result = await _context.SaveChangesAsync() > 0;

        if(!result)
        {
            return BadRequest("Changes were not made");
        }
        return Ok();
    }

}
