namespace SurveyBasket.Api.Services;

public interface IPollService
{


    IEnumerable<Poll> GetALl();

    Poll? Get(int id);

    Poll Add(Poll request);

    bool Update(int id, Poll poll);

    bool Delete(int id);
}
