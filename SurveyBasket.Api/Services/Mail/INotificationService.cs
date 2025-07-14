namespace SurveyBasket.Api.Services.Mail;

public interface INotificationService
{
    Task SendPollsNotification(int? pollId);
}
