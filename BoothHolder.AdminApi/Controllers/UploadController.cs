using BoothHolder.Common.Oss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoothHolder.AdminApi.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UploadController : ControllerBase
    {
        private readonly OssService _ossService;
        private readonly string[] validImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
        private readonly ILogger<UploadController> _logger;

        public UploadController(OssService ossService, ILogger<UploadController> logger)
        {
            _ossService = ossService;
            _logger = logger;
        }

        /// <summary>
        /// 上传用户头像
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UploadUserImg(IFormFile file)
        {
            return await UploadFileToOss(file, "userava/");
        }

        /// <summary>
        /// 上传展位图片
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UploadBoothImg(IFormFile file)
        {
            return await UploadFileToOss(file, "booth/");
        }

        /// <summary>
        /// 上传文件到阿里云 OSS
        /// </summary>
        private async Task<IActionResult> UploadFileToOss(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("上传的文件不能为空.");
            }

            string fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!validImageExtensions.Contains(fileExtension))
            {
                return BadRequest("图片格式错误.");
            }

            try
            {
                string fileName = $"{folderPath}{Guid.NewGuid()}{fileExtension}"; // 避免文件名冲突
                using var fileStream = file.OpenReadStream();
                bool success = _ossService.UploadFile(fileName, fileStream);

                if (success)
                {
                    string endpoint = _ossService.Endpoint.Replace("https://", "").Replace("http://", "");
                    string fileUrl = $"https://{_ossService.BucketName}.{endpoint}/{fileName}";
                    return Ok(new { message = "文件上传成功", fileUrl });
                }

                return BadRequest("文件上传失败.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "文件上传发生异常");
                return StatusCode(500, "服务器内部错误.");
            }
        }
    }
}
