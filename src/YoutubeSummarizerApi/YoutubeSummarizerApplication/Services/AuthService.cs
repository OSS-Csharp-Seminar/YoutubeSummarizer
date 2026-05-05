using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using YoutubeSummarizer.Application.DTOs;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;
        private readonly IJwtService _jwtService;

        public AuthService(
            IAuthRepository authRepo,
            IJwtService jwtService)
        {
            _authRepo = authRepo;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {

            var isValid = await _authRepo.CheckPasswordAsync(
                dto.Email, dto.Password);

            if (!isValid)
                throw new UnauthorizedAccessException("Invalid credentials");

            var user = await _authRepo.FindByEmailAsync(dto.Email)
                ?? throw new Exception("User not found");

            var token = _jwtService.GenerateToken(user);

            return new LoginResponseDto
            {
                FullName = $"{user.FirstName} {user.LastName}",
            };
        }
    }

}
