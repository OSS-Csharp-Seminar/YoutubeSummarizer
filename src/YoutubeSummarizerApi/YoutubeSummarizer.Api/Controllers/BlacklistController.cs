using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YoutubeSummarizer.Application.Features.Blacklist.Dtos;
using YoutubeSummarizer.Application.Features.Blacklist.Interfaces;

namespace YoutubeSummarizer.Api.Controllers
{
    [ApiController]
    [Route("api/blacklist")]
    [Authorize]
    public class BlacklistController : ControllerBase
    {
        private readonly IBlacklistService _blacklistService;

        public BlacklistController(IBlacklistService blacklistService)
        {
            _blacklistService = blacklistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetKeywords(CancellationToken cancellationToken)
        {
            var result = await _blacklistService.GetKeywordsAsync(cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddKeyword([FromBody] AddKeywordRequest request, CancellationToken cancellationToken)
        {
            var result = await _blacklistService.AddKeywordAsync(request, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoveKeyword(Guid id, CancellationToken cancellationToken)
        {
            var result = await _blacklistService.RemoveKeywordAsync(id, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }
    }
}