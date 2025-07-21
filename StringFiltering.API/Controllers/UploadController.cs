using Microsoft.AspNetCore.Mvc;
using StringFiltering.Application.Dtos;
using StringFiltering.Application.Services;
using StringFiltering.Application.Utilities;

namespace StringFiltering.API.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _uploadService;
        private readonly IUploadStorage _uploadStorage;

        public UploadController(
            IUploadService uploadService,
            IUploadStorage uploadStorage)
        {
            _uploadService = uploadService;
            _uploadStorage = uploadStorage;
        }

        [HttpPost]
        public async Task<IActionResult> UploadString([FromBody] UploadRequestDto requestDto)
        {
            var operationResult = await _uploadService.UploadString(requestDto);

            if (operationResult.IsSuccess)
                return Accepted(new { Status = "Accepted" });

            return BadRequest(new { operationResult.ErrorMessage });
        }

        [HttpGet("{uploadId}")]
        public IActionResult GetFinalText([FromRoute] string uploadId)
        {
            var result = _uploadStorage.FetchFinalText(uploadId);
            return Ok(result);
        }
    }
}