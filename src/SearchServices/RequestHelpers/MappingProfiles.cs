using System.Reflection.Emit;
using AutoMapper;
using Contracts;
using SearchServices.Models;

namespace SearchServices;

public class MappingProfiles : Profile
{
    public MappingProfiles ()
    {
        CreateMap<AuctionCreated, Item>();
        CreateMap<AuctionUpdated, Item>();
    }
}
