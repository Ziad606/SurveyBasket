
namespace SurveyBasket.Api.Services;

public class PollService : IPollService
{
    private static List<Poll> _polls = [
        new Poll{
            Id = 1,
            Title = "Poll 1",
            Description = "My First Pool"
        }
    ];

    public IEnumerable<Poll> GetALl()
    {
        return _polls;
    }

    public Poll? Get(int id)
    {
        return _polls.SingleOrDefault(p => p.Id == id);
    }

    public Poll Add(Poll poll)
    {
        poll.Id = _polls.Count + 1;
        _polls.Add(poll);
        return poll;
    }

    public bool Update(int id, Poll poll)
    {
        var existingPoll = Get(id);

        if (existingPoll is null)
            return false;

        existingPoll.Title = poll.Title;
        existingPoll.Description = poll.Description;

        return true;
    }

    public bool Delete(int id)
    {
        var existingPoll = Get(id);

        if (existingPoll is null)
            return false;

        _polls.Remove(existingPoll);
        return true;
    }
}
