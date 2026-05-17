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
                    return ServiceResponse<LoginResponseDto>.Failure("Neispravni korisnički podaci.");

                var user = await _authRepo.FindByEmailAsync(email);
                if (user == null)
                    return ServiceResponse<LoginResponseDto>.Failure("Neispravni korisnički podaci.");

                if (!user.IsActive)
                    return ServiceResponse<LoginResponseDto>.Failure("Korisnički račun je deaktiviran.");

                var (accessToken, _) = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken(ipAddress);
                refreshToken.UserId = user.Id.ToString();

                await _refreshTokenRepo.AddAsync(refreshToken, cancellationToken);

                var data = _mapper.MapToLoginResponseDto(user, accessToken, refreshToken.Token);
                return ServiceResponse<LoginResponseDto>.Success(data, "Uspješna prijava.");
            }
            catch
            {
                return ServiceResponse<LoginResponseDto>.Failure("Došlo je do greške.");
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
                    return ServiceResponse<RegisterResponseDto>.Failure("Korisnik već postoji.");

                var user = await _authRepo.CreateUserAsync(dto.FirstName.Trim(), dto.LastName.Trim(), email, dto.Password);

                var (accessToken, _) = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken(ipAddress);
                refreshToken.UserId = user.Id.ToString();

                await _refreshTokenRepo.AddAsync(refreshToken, cancellationToken);

                var data = _mapper.MapToRegisterResponseDto(user, accessToken, refreshToken.Token);
                return ServiceResponse<RegisterResponseDto>.Success(data, "Registracija uspješna.");
            }
            catch
            {
                return ServiceResponse<RegisterResponseDto>.Failure("Došlo je do greške.");
            }
        }

        public async Task<ServiceResponse<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto, string ipAddress, CancellationToken cancellationToken = default)
        {
            try
            {
                var principal = _jwtService.ValidateTokenWithoutLifetime(dto.AccessToken);
                if (principal == null)
                    return ServiceResponse<RefreshTokenResponseDto>.Failure("Refresh token nije valjan.");

                var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return ServiceResponse<RefreshTokenResponseDto>.Failure("Refresh token nije valjan.");

                var token = await _refreshTokenRepo.GetByTokenAsync(dto.RefreshToken, cancellationToken);
                if (token == null || !token.IsActive || token.UserId != userId)
                    return ServiceResponse<RefreshTokenResponseDto>.Failure("Refresh token nije valjan.");

                var user = await _userRepo.GetByIdAsync(Guid.Parse(userId));
                if (user == null)
                    return ServiceResponse<RefreshTokenResponseDto>.Failure("Refresh token nije valjan.");

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
                    "Token uspješno obnovljen.");
            }
            catch
            {
                return ServiceResponse<RefreshTokenResponseDto>.Failure("Došlo je do greške.");
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
