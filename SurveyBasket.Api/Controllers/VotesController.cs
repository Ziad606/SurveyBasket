using SurveyBasket.Api.Contracts.Votes;
using SurveyBasket.Api.Extensions;
using SurveyBasket.Api.Services.Questions;
using SurveyBasket.Api.Services.Votes;

namespace SurveyBasket.Api.Controllers;
[Route("api/polls/{pollId}/vote")]
[ApiController]
//[Authorize]
public class VotesController(IVoteService voteService, IQuestionService questionService) : ControllerBase
{
    private readonly IVoteService _voteService = voteService;
    private readonly IQuestionService _questionService = questionService;

    [HttpGet("")]
    public async Task<IActionResult> GetAvailable([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        //var userId = User.GetUserId();
        var result = await _questionService.GetAvailableAsync(pollId, cancellationToken); //, userId!
        return result.IsSuccess ? Ok(result) : result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromRoute] int pollId, [FromBody] VoteRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _voteService.AddAsync(pollId, userId!, request, cancellationToken);

        return result.IsSuccess
            ? Created()
            : result.ToProblem();
    }
}
