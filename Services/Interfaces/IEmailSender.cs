using System.Threading.Tasks;

namespace FoodMarket.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}