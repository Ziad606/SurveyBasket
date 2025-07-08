using SurveyBasket.Api.Contracts.Votes;
using SurveyBasket.Api.Extensions;
using SurveyBasket.Api.Services.Votes;

namespace SurveyBasket.Api.Controllers;
[Route("api/polls/{pollId}/vote")]
[ApiController]
public class VotesController(IVoteService voteService) : ControllerBase
{
    private readonly IVoteService _voteService = voteService;


    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
    {
        //var result = await _voteService.GetAsync(pollId, id, cancellationToken);
        //return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        return Ok();
    }


    [HttpPost("")]
    public async Task<IActionResult> Add([FromRoute] int pollId, [FromBody] VoteRequest request, CancellationToken cancellationToken)
    {
        var result = await _voteService.AddAsync(pollId, User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess
            ? Created()
            : result.ToProblem();
    }
}
