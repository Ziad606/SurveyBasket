using Moq;
using SurveyBasket.Api.Contracts.Polls;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Presistenace;
using SurveyBasket.Api.Services.Mail;
using SurveyBasket.Api.Services.Polls;
using SurveyBasket.Tests.Shared;

namespace SurveyBasket.Tests;

public class PollServiceTests
{
    private readonly PollService _pollService;
    private readonly ApplicationDbContext _context;
    private readonly Mock<INotificationService> _notificationService;
    public PollServiceTests()
    {
        _context = ApplicationDbContextFactory.CreateContext();
        _notificationService = new Mock<INotificationService>();
        _pollService = new PollService(_context, _notificationService.Object);
    }

    [Fact]
    public async Task GetAsync_WithNonExistingId_ReturnsNotFound()
    {
        //Arrange 
        var pollId = 1;

        //Act 
        var result = await _pollService.GetAsync(pollId, default);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(PollErrors.PollNotFound, result.Error);
    }

    [Fact]
    public async Task GetAsync_WithExistingId_ReturnsPoll()
    {
        //Arrange 
        var poll = await AddPoll();

        //Act 
        var result = await _pollService.GetAsync(poll.Id, default);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Test Poll", poll.Title);
    }

    [Fact]
    public async Task GetCurrentAsync_WithNoActivePolls_ReturnsEmptyList()
    {
        var result = await _pollService.GetCurrentAsync(default);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }
    [Fact]
    public async Task GetCurrentAsync_WithActivePolls_ReturnsPollsList()
    {
        //Arrange
        AddListOfPolls();

        //Act
        var result = await _pollService.GetCurrentAsync(default);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
    }

    [Fact]
    public async Task GetAllAsync_WithNoPolls_ReturnsEmptyList()
    {
        //Act
        var result = await _pollService.GetAllAsync(default);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetAllAsync_WithPolls_ReturnsAllPolls()
    {
        //Arrange
        await AddPoll();

        //Act
        var result = await _pollService.GetAllAsync(default);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task AddAsync_WithUniqueTitle_ReturnsSuccess()
    {
        //Arrange
        var request = new PollRequest(
            "New Poll",
            "New Poll Summary",
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
        );

        //Act
        var result = await _pollService.AddAsync(request, default);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("New Poll", result.Value.Title);
    }

    [Fact]
    public async Task AddAsync_WithDuplicateTitle_ReturnsFailure()
    {
        //Arrange
        var poll = await AddPoll();
        var request = new PollRequest(
            poll.Title,
            "Another Summary",
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
        );

        //Act
        var result = await _pollService.AddAsync(request, default);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(PollErrors.DuplicatePollTitle, result.Error);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingId_ReturnsNotFound()
    {
        //Arrange
        var request = new PollRequest(
            "Updated Poll",
            "Updated Summary",
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
        );
        var pollId = 999;

        //Act
        var result = await _pollService.UpdateAsync(pollId, request, default);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(PollErrors.PollNotFound, result.Error);
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateTitle_ReturnsFailure()
    {
        //Arrange
        var poll1 = await AddPoll();
        var poll2 = new Poll
        {
            Title = "Existing Poll",
            Summary = "Summary",
            StartsAt = DateOnly.FromDateTime(DateTime.UtcNow),
            EndsAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
        };
        await _context.AddAsync(poll2, default);
        await _context.SaveChangesAsync(default);

        var request = new PollRequest(
            poll1.Title,
            "Updated Summary",
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
        );

        //Act
        var result = await _pollService.UpdateAsync(poll2.Id, request, default);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(PollErrors.DuplicatePollTitle, result.Error);
    }

    [Fact]
    public async Task UpdateAsync_WithValidRequest_ReturnsSuccess()
    {
        //Arrange
        var poll = await AddPoll();
        var request = new PollRequest(
            "Updated Poll",
            "Updated Summary",
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
        );

        //Act
        var result = await _pollService.UpdateAsync(poll.Id, request, default);

        //Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ReturnsNotFound()
    {
        //Arrange
        var pollId = 999;

        //Act
        var result = await _pollService.DeleteAsync(pollId, default);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(PollErrors.PollNotFound, result.Error);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ReturnsSuccess()
    {
        //Arrange
        var poll = await AddPoll();

        //Act
        var result = await _pollService.DeleteAsync(poll.Id, default);

        //Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task TogglePublishStatusAsync_WithNonExistingId_ReturnsNotFound()
    {
        //Arrange
        var pollId = 999;

        //Act
        var result = await _pollService.TogglePublishStatusAsync(pollId, default);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(PollErrors.PollNotFound, result.Error);
    }

    [Fact]
    public async Task TogglePublishStatusAsync_FromUnpublishedToPublished_ReturnsSuccess()
    {
        //Arrange
        var poll = new Poll
        {
            Title = "Test Poll",
            Summary = "Summary",
            StartsAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), // Start date in future to avoid triggering notification
            EndsAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            IsPublished = false
        };
        await _context.AddAsync(poll, default);
        await _context.SaveChangesAsync(default);

        //Act
        var result = await _pollService.TogglePublishStatusAsync(poll.Id, default);

        //Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task TogglePublishStatusAsync_FromPublishedToUnpublished_ReturnsSuccess()
    {
        //Arrange
        var poll = new Poll
        {
            Title = "Published Poll",
            Summary = "Summary",
            StartsAt = DateOnly.FromDateTime(DateTime.UtcNow),
            EndsAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            IsPublished = true
        };
        await _context.AddAsync(poll, default);
        await _context.SaveChangesAsync(default);

        //Act
        var result = await _pollService.TogglePublishStatusAsync(poll.Id, default);

        //Assert
        Assert.True(result.IsSuccess);
    }


    private async Task AddListOfPolls()
    {
        var polls = new List<Poll>();
        for (int i = 1; i < 5; i++)
        {
            polls.Add(new Poll
            {
                Title = $"Test Poll {i}",
                Summary = $"Summary {i}",
                StartsAt = DateOnly.FromDateTime(DateTime.UtcNow),
                EndsAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                IsPublished = true
            });
        }
        await _context.AddRangeAsync(polls, default);
        await _context.SaveChangesAsync(default);
    }

    private async Task<Poll> AddPoll()
    {
        var poll = new Poll
        {
            Title = "Test Poll",
            Summary = "Summary",
            StartsAt = DateOnly.FromDateTime(DateTime.UtcNow),
            EndsAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
        };
        await _context.AddAsync(poll, default);
        await _context.SaveChangesAsync(default);

        return poll;
    }
}
