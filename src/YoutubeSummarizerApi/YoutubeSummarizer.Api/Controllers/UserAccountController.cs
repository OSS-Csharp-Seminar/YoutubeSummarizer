using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YoutubeSummarizer.Application.Features.UserAccount.Dtos;
using YoutubeSummarizer.Application.Features.UserAccount.Interfaces;

namespace YoutubeSummarizer.Api.Controllers
{
    [ApiController]
    [Route("api/account")]
    [Authorize]
    public class UserAccountController : ControllerBase
    {
        private readonly IUserAccountService _userAccountService;

        public UserAccountController(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        [HttpGet("blacklist")]
        public async Task<IActionResult> GetBlacklistEntries(CancellationToken cancellationToken)
        {
            var result = await _userAccountService.GetBlacklistEntriesAsync(cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("blacklist")]
        public async Task<IActionResult> AddBlacklistEntry(
            [FromBody] AddBlacklistEntryRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _userAccountService.AddBlacklistEntryAsync(request, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("blacklist")]
        public async Task<IActionResult> RemoveBlacklistEntry([FromQuery] string keyword, CancellationToken cancellationToken)
        {
            var result = await _userAccountService.RemoveBlacklistEntryAsync(keyword, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount(
            [FromBody] DeleteAccountRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _userAccountService.DeleteAccountAsync(request, cancellationToken);
            if (!result.Status)
                return BadRequest(result);

            Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
            Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/api/auth" });

            return Ok(result);
        }
    }
}
