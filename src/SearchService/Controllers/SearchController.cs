using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHealpers;


namespace SearchService;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItem([FromQuery] SearchPrams searchPrams)

    {
        var query = DB.PagedSearch<Item, Item>();


        if (!String.IsNullOrEmpty(searchPrams.SearchTerm))
        {
            query.Match(Search.Full, searchPrams.SearchTerm).SortByTextScore();
        }

        //sorting
        query = searchPrams.OrderBy switch
        {
            "make" => query.Sort(x => x.Ascending(a => a.Make)),
            "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
            _ => query.Sort(x => x.Ascending(a => a.AuctionEnd)) //default sorting
        };
        query = searchPrams.FilterBy switch
        {
            "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6)
            && x.AuctionEnd > DateTime.UtcNow),
            _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        };

        if (!string.IsNullOrEmpty(searchPrams.Seller))
        {
            query.Match(x => x.Seller == searchPrams.Seller);
        }

        if (!string.IsNullOrEmpty(searchPrams.Winner))
        {
            query.Match(x => x.Winner == searchPrams.Winner);
        }

        query.PageNumber(searchPrams.PageNumber);
        query.PageSize(searchPrams.PageSize);

        var result = await query.ExecuteAsync();

        return Ok(new
        {
            result = result.Results,
            pageCount = result.PageCount,
            totaCount = result.TotalCount
        });
    }

}
