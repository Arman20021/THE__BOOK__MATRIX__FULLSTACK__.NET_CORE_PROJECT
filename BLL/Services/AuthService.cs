using DAL.EF.Tables;
using DAL.Repositories;
using BLL.DTOs;
using BLL.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BLL.Services
{
    public class AuthService
    {
        UserRepository userRepository;
        EmailVerificationRepository emailVerificationRepository;
        EmailService emailService;
        IConfiguration configuration;

        public AuthService(
            UserRepository userRepository,
            EmailVerificationRepository emailVerificationRepository,
            EmailService emailService,
            IConfiguration configuration)
        {
            this.userRepository = userRepository;
            this.emailVerificationRepository = emailVerificationRepository;
            this.emailService = emailService;
            this.configuration = configuration;
        }

        public AuthResultDto Register(RegisterDto dto)
        {
            var existingUser = userRepository.GetUserByEmail(dto.Email);

            if (existingUser != null)
            {
                return new AuthResultDto
                {
                    IsSuccess = false,
                    Message = "This email is already registered."
                };
            }

            if (!dto.Password.Any(char.IsUpper))
            {
                return new AuthResultDto
                {
                    IsSuccess = false,
                    Message = "Password must contain at least one capital letter."
                };
            }

            User user = new User();

            user.FullName = dto.FullName;
            user.Email = dto.Email;

            // If your Phone column is int
            user.Phone = Convert.ToInt32(dto.Phone);

            user.Password = dto.Password;
            user.RoleId = 2; // 2 = normal user
            user.IsEmailVerified = false;
            user.IsActive = true;
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            userRepository.AddUser(user);

            string tokenValue = Guid.NewGuid().ToString("N");

            EmailVerificationToken token = new EmailVerificationToken();

            token.UserId = user.UserId;
            token.Token = tokenValue;
            token.ExpireAt = DateTime.Now.AddHours(24);
            token.IsUsed = false;
            token.CreatedAt = DateTime.Now;

            emailVerificationRepository.AddToken(token);

            string baseUrl = configuration["AppSettings:BaseUrl"]!;
            string verificationLink = baseUrl + "/Auth/VerifyEmail?token=" + tokenValue;

            string subject = "Verify Your Book Shop Account";

            string body = $@"
                <h2>Hello {user.FullName},</h2>
                <p>Thank you for registering in Book Shop.</p>
                <p>Please click the button below to verify your email.</p>

                <p>
                    <a href='{verificationLink}'
                       style='background-color:#0d6efd;color:white;padding:10px 18px;
                       text-decoration:none;border-radius:5px;'>
                       Verify Email
                    </a>
                </p>

                <p>This link will expire in 24 hours.</p>
            ";

            emailService.SendEmail(user.Email, subject, body);

            return new AuthResultDto
            {
                IsSuccess = true,
                Message = "Registration successful. Please check your email and verify your account."
            };
        }

        public AuthResultDto VerifyEmail(string tokenValue)
        {
            var verificationToken = emailVerificationRepository.GetValidToken(tokenValue);

            if (verificationToken == null)
            {
                return new AuthResultDto
                {
                    IsSuccess = false,
                    Message = "Invalid or expired verification link."
                };
            }

            var user = userRepository.GetUserById(verificationToken.UserId);

            if (user == null)
            {
                return new AuthResultDto
                {
                    IsSuccess = false,
                    Message = "User not found."
                };
            }

            user.IsEmailVerified = true;
            user.UpdatedAt = DateTime.Now;

            verificationToken.IsUsed = true;

            userRepository.UpdateUser(user);
            emailVerificationRepository.UpdateToken(verificationToken);

            return new AuthResultDto
            {
                IsSuccess = true,
                Message = "Email verified successfully. You can now login."
            };
        }

        public AuthResultDto Login(LoginDto dto)
        {
            var user = userRepository.GetUserByEmail(dto.Email);

            if (user == null)
            {
                return new AuthResultDto
                {
                    IsSuccess = false,
                    Message = "Invalid email or password."
                };
            }

            if (user.Password != dto.Password)
            {
                return new AuthResultDto
                {
                    IsSuccess = false,
                    Message = "Invalid email or password."
                };
            }

            if (user.IsEmailVerified == false)
            {
                return new AuthResultDto
                {
                    IsSuccess = false,
                    Message = "Please verify your email before login."
                };
            }

            if (user.IsActive == false)
            {
                return new AuthResultDto
                {
                    IsSuccess = false,
                    Message = "Your account is inactive."
                };
            }

            string roleName = userRepository.GetRoleNameByRoleId(user.RoleId);

            return new AuthResultDto
            {
                IsSuccess = true,
                Message = "Login successful.",
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = roleName
            };
        }
    }
}
