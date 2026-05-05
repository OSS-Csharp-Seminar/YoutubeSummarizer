using FluentValidation;
using YoutubeSummarizer.Application.DTOs;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.Application.Mappings;

namespace YoutubeSummarizer.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;
        private readonly IJwtService _jwtService;
        private readonly UserMapper _mapper;
        private readonly IValidator<RegisterRequestDto> _registerValidator;

        public AuthService(
            IAuthRepository authRepo,
            IJwtService jwtService,
            UserMapper mapper,
            IValidator<RegisterRequestDto> registerValidator)
        {
            _authRepo = authRepo;
            _jwtService = jwtService;
            _mapper = mapper;
            _registerValidator = registerValidator;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var email = dto.Email.Trim();
            var isValid = await _authRepo.CheckPasswordAsync(
                email, dto.Password);

            if (!isValid)
                throw new UnauthorizedAccessException("Invalid credentials");

            var user = await _authRepo.FindByEmailAsync(email)
                ?? throw new Exception("User not found");

            var token = _jwtService.GenerateToken(user);

            return _mapper.MapToLoginResponseDto(user, token);
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            var validationResult = await _registerValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var firstName = dto.FirstName.Trim();
            var lastName = dto.LastName.Trim();
            var email = dto.Email.Trim();

            var existingUser = await _authRepo.FindByEmailAsync(email);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists");

            var user = await _authRepo.CreateUserAsync(
                firstName,
                lastName,
                email,
                dto.Password);

            var token = _jwtService.GenerateToken(user);

            return _mapper.MapToRegisterResponseDto(user, token);
        }
    }
}
