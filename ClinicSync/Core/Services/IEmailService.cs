using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IEmailService
    {
        //Task SendEmailVerificationAsync(string email, string name, string token);
        Task SendPasswordResetAsync(string email, string name, string token);
        Task SendDoctorCredentialsAsync(string email, string name, string username, string password);
    }
}
