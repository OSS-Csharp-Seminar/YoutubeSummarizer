using System.Security.Claims;
using FluentValidation;
using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.DTOs;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.Application.Mappings;

namespace YoutubeSummarizer.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IUserRepository _userRepo;
        private readonly UserMapper _mapper;
        private readonly IValidator<RegisterRequestDto> _registerValidator;

        public AuthService(
            IAuthRepository authRepo,
            IJwtService jwtService,
            IRefreshTokenRepository refreshTokenRepo,
            IUserRepository userRepo,
            UserMapper mapper,
            IValidator<RegisterRequestDto> registerValidator)
        {
            _authRepo = authRepo;
            _jwtService = jwtService;
            _refreshTokenRepo = refreshTokenRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _registerValidator = registerValidator;
        }

        public async Task<ServiceResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto, string ipAddress, CancellationToken cancellationToken = default)
        {
            try
            {
                var email = dto.Email.Trim();
                var isValid = await _authRepo.CheckPasswordAsync(email, dto.Password);
                if (!isValid)
                    return ServiceResponse<LoginResponseDto>.Failure("Invalid credentials.");

                var user = await _authRepo.FindByEmailAsync(email);
                if (user == null)
                    return ServiceResponse<LoginResponseDto>.Failure("Invalid credentials.");

                if (!user.IsActive)
                    return ServiceResponse<LoginResponseDto>.Failure("Account is deactivated.");

                var (accessToken, _) = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken(ipAddress);
                refreshToken.UserId = user.Id.ToString();

                await _refreshTokenRepo.AddAsync(refreshToken, cancellationToken);

                var data = _mapper.MapToLoginResponseDto(user, accessToken, refreshToken.Token);
                return ServiceResponse<LoginResponseDto>.Success(data, "Login successful.");
            }
            catch
            {
                return ServiceResponse<LoginResponseDto>.Failure("An error occurred.");
            }
        }

        public async Task<ServiceResponse<RegisterResponseDto>> RegisterAsync(RegisterRequestDto dto, string ipAddress, CancellationToken cancellationToken = default)
        {
            try
            {
                var validationResult = await _registerValidator.ValidateAsync(dto, cancellationToken);
                if (!validationResult.IsValid)
                    return ServiceResponse<RegisterResponseDto>.Failure(validationResult.Errors.First().ErrorMessage);

                var email = dto.Email.Trim();
                var existingUser = await _authRepo.FindByEmailAsync(email);
                if (existingUser != null)
                    return ServiceResponse<RegisterResponseDto>.Failure("User already exists.");

                var user = await _authRepo.CreateUserAsync(dto.FirstName.Trim(), dto.LastName.Trim(), email, dto.Password);

                var (accessToken, _) = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken(ipAddress);
                refreshToken.UserId = user.Id.ToString();

                await _refreshTokenRepo.AddAsync(refreshToken, cancellationToken);

                var data = _mapper.MapToRegisterResponseDto(user, accessToken, refreshToken.Token);
                return ServiceResponse<RegisterResponseDto>.Success(data, "Registration successful.");
            }
            catch
            {
                return ServiceResponse<RegisterResponseDto>.Failure("An error occurred.");
            }
        }

        public async Task<ServiceResponse<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto, string ipAddress, CancellationToken cancellationToken = default)
        {
            try
            {
                var principal = _jwtService.ValidateTokenWithoutLifetime(dto.AccessToken);
                if (principal == null)
                    return ServiceResponse<RefreshTokenResponseDto>.Failure("Invalid refresh token.");

                var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return ServiceResponse<RefreshTokenResponseDto>.Failure("Invalid refresh token.");

                var token = await _refreshTokenRepo.GetByTokenAsync(dto.RefreshToken, cancellationToken);
                if (token == null || !token.IsActive || token.UserId != userId)
                    return ServiceResponse<RefreshTokenResponseDto>.Failure("Invalid refresh token.");

                var user = await _userRepo.GetByIdAsync(Guid.Parse(userId));
                if (user == null)
                    return ServiceResponse<RefreshTokenResponseDto>.Failure("Invalid refresh token.");

                var (newAccessToken, _) = _jwtService.GenerateAccessToken(user);
                var newRefreshToken = _jwtService.GenerateRefreshToken(ipAddress);
                newRefreshToken.UserId = userId;

                token.UsedAtUtc = DateTime.UtcNow;
                token.ReplacedByToken = newRefreshToken.Token;

                await _refreshTokenRepo.UpdateAsync(token, cancellationToken);
                await _refreshTokenRepo.AddAsync(newRefreshToken, cancellationToken);

                return ServiceResponse<RefreshTokenResponseDto>.Success(
                    new RefreshTokenResponseDto
                    {
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken.Token
                    },
                    "Token refreshed successfully.");
            }
            catch
            {
                return ServiceResponse<RefreshTokenResponseDto>.Failure("An error occurred.");
            }
        }

        public async Task LogoutAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default)
        {
            var token = await _refreshTokenRepo.GetByTokenAsync(refreshToken, cancellationToken);
            if (token != null && token.IsActive)
            {
                token.RevokedAtUtc = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;
                await _refreshTokenRepo.UpdateAsync(token, cancellationToken);
            }
        }
    }
}
