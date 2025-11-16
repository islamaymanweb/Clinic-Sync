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
     
        //public async Task SendEmailVerificationAsync(string email, string name, string token)
        //{
        //    try
        //    {
        //        var frontendBaseUrl = _smtpSettings.FrontendBaseUrl?.TrimEnd('/') ?? "http://localhost:4200";
 
        //        var encodedToken = WebUtility.UrlEncode(token);
        //        var encodedEmail = WebUtility.UrlEncode(email);

        //        var verificationUrl = $"{frontendBaseUrl}/auth/verify-email?token={encodedToken}&email={encodedEmail}";

        //        Console.WriteLine($"🔗 Verification URL: {verificationUrl}"); 

        //        var subject = "Verify Your Email - HealthSync";
        //        var body = $"""
        //    <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
        //        <h2 style="color: #007bff;">Welcome to HealthSync, {name}!</h2>
        //        <p>Please verify your email address by clicking the button below:</p>
                
        //        <div style="text-align: center; margin: 30px 0;">
        //            <a href="{verificationUrl}" 
        //               style="background-color: #007bff; color: white; padding: 12px 24px; 
        //                      text-decoration: none; border-radius: 5px; display: inline-block; 
        //                      font-weight: bold;">
        //                Verify Email Address
        //            </a>
        //        </div>
                
        //        <p style="color: #666; font-size: 14px;">
        //            <strong>Or copy this link:</strong><br>
        //            <span style="background: #f8f9fa; padding: 10px; border-radius: 5px; 
        //                         word-break: break-all; display: inline-block; margin-top: 5px; 
        //                         font-family: monospace; font-size: 12px;">
        //                {verificationUrl}
        //            </span>
        //        </p>
                
        //        <p style="color: #999; font-size: 12px; margin-top: 20px;">
        //            This link will expire in 24 hours.
        //        </p>
                
        //        <hr style="border: none; border-top: 1px solid #eee; margin: 20px 0;">
                
        //        <p style="color: #777; font-size: 12px;">
        //            If the button doesn't work, please make sure you're using a modern web browser 
        //            and that you copy the entire link into your browser's address bar.
        //        </p>
        //    </div>
        //    """;

        //        await SendEmailAsync(email, name, subject, body);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to send verification email to {Email}", email);
        //        throw;
        //    }
        //}

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
        public string FrontendBaseUrl { get; set; } = string.Empty;  
    }
}
