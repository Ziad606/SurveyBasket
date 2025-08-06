using Microsoft.AspNetCore.Cors;
using SurveyBasket.Api.Services.Polls;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = DefaultRole.Member)]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;



    [HttpGet("")]
    //[HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpGet("current")]
    //[Authorize(Roles = DefaultRole.Member)]
    public async Task<IActionResult> GetCurrunt(CancellationToken cancellationToken)
    {
        var result = await _pollService.GetCurrentAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id}")]
    //[HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    [DisableCors]
    //[HasPermission(Permissions.AddPolls)]
    public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var result = await _pollService.AddAsync(request, cancellationToken);

        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value) : result.ToProblem();
    }

    [HttpPut("{id}")]
    //[HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var result = await _pollService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{id}")]
    [DisableCors]
    //[HasPermission(Permissions.DeletePolls)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{id}/togglePublish")]
    [EnableCors("AllowAll")]
    //[HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishStatusASync(id, cancellationToken);


        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

}
