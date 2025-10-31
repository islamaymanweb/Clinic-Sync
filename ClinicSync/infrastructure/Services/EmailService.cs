using Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailVerificationAsync(string email, string name, string token)
        {
            // ✅ استخدام Frontend URL بدلاً من Backend URL
            var verificationUrl = $"{_smtpSettings.FrontendBaseUrl}/auth/verify-email?token={token}&email={email}";

            var subject = "Verify Your Email - HealthSync";
            var body = $"""
                <h2>Welcome to HealthSync, {name}!</h2>
                <p>Please verify your email address by clicking the link below:</p>
                <p><a href="{verificationUrl}" style="background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;">Verify Email</a></p>
                <p>Or copy this link: {verificationUrl}</p>
                <p>This link will expire in 24 hours.</p>
                <br>
                <p>Best regards,<br>HealthSync Team</p>
                """;

            await SendEmailAsync(email, name, subject, body);
        }

        public async Task SendPasswordResetAsync(string email, string name, string token)
        {
            // ✅ استخدام Frontend URL
            var resetUrl = $"{_smtpSettings.FrontendBaseUrl}/auth/reset-password?token={token}&email={email}";

            var subject = "Reset Your Password - HealthSync";
            var body = $"""
                <h2>Password Reset Request</h2>
                <p>Hello {name},</p>
                <p>You requested to reset your password. Click the link below to proceed:</p>
                <p><a href="{resetUrl}" style="background-color: #dc3545; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;">Reset Password</a></p>
                <p>Or copy this link: {resetUrl}</p>
                <p>This link will expire in 24 hours.</p>
                <br>
                <p>Best regards,<br>HealthSync Team</p>
                """;

            await SendEmailAsync(email, name, subject, body);
        }

        public async Task SendDoctorCredentialsAsync(string email, string name, string username, string password)
        {
            var subject = "Your HealthSync Doctor Account";
            var body = $"""
                <h2>Welcome to HealthSync, Dr. {name}!</h2>
                <p>Your doctor account has been created successfully.</p>
                <p><strong>Login Credentials:</strong></p>
                <ul>
                    <li><strong>Email/Username:</strong> {username}</li>
                    <li><strong>Temporary Password:</strong> {password}</li>
                </ul>
                <p>Please login and change your password immediately.</p>
                <p><a href="{_smtpSettings.FrontendBaseUrl}/auth/login" style="background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;">Login to HealthSync</a></p>
                <br>
                <p>Best regards,<br>HealthSync Team</p>
                """;

            await SendEmailAsync(email, name, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string toName, string subject, string body)
        {
            try
            {
                using var smtpClient = new SmtpClient(_smtpSettings.SmtpServer, _smtpSettings.Port)
                {
                    EnableSsl = _smtpSettings.EnableSsl,
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(new MailAddress(toEmail, toName));

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw new Exception($"Failed to send email: {ex.Message}");
            }
        }
    }


    public class SmtpSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public bool EnableSsl { get; set; }
        public string FrontendBaseUrl { get; set; } = string.Empty; // ✅ جديد
    }
}
