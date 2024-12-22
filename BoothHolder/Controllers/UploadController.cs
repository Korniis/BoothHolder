using BoothHolder.Common.Configration;
using COSXML;
using COSXML.Model.Object;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly CosXml _cosXml;
    private readonly string _bucket;
    private readonly string _region;
    private readonly string[] validImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
    // 通过构造函数初始化 COS 客户端
    public UploadController(CosXml cosXml)
    {
        _cosXml = cosXml;
        _region = AppSettings.app("TencentCloud:Region");
        _bucket = AppSettings.app("TencentCloud:Bucket");
    }
    // 上传文件接口
    [HttpPost]
    [Authorize]

    public async Task<IActionResult> UploadUserImg(IFormFile file)
    {  // 检查文件的扩展名是否是图片格式
        if (file == null || file.Length == 0)
        {
            return BadRequest("空文件.");
        }

        string fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (!validImageExtensions.Contains(fileExtension))
        {
            // 如果不是图片格式，返回 null
            return BadRequest("图片格式错误"); ;
        }
        try
        {
            var result = await SetFilePathsAync(file, "/userava/");
            if (!string.IsNullOrEmpty(result))
            {
                return Ok(new { message = "File uploaded successfully", fileUrl = result });
            }
            return BadRequest("File upload failed.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while uploading the file.");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost]
    [Authorize]

    public async Task<IActionResult> UploadBoothImg(IFormFile file)
    {
        string fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (!validImageExtensions.Contains(fileExtension))
        {
            // 如果不是图片格式，返回 null
            return BadRequest("图片格式错误"); ;
        }
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }
        try
        {
            var result = await SetFilePathsAync(file, "/booth/");
            if (!string.IsNullOrEmpty(result))
            {
                return Ok(new { message = "File uploaded successfully", fileUrl = result });
            }
            return BadRequest("File upload failed.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while uploading the file.");
            return StatusCode(500, "Internal server error.");
        }
    }


    private async Task<string> SetFilePathsAync(IFormFile file, string path)
    {


        // 获取文件的流
        using (var fileStream = file.OpenReadStream())
        {
            // 文件名
            string fileName = file.FileName;
            // 创建文件存储路径（可以根据需要调整文件存储路径）
            string cosPath = path;
            // 调用异步上传方法
            var result = await UploadFileToCosAsync(fileStream, cosPath, fileName);
            return result;
        }
    }
    // 异步上传文件到腾讯云 COS
    private async Task<string> UploadFileToCosAsync(Stream fileStream, string cosPath, string fileName)
    {
        try
        {
            // 为避免文件名冲突，使用 GUID 生成唯一的文件名
            string uniqueFileName = cosPath + "_" + Guid.NewGuid() + Path.GetExtension(fileName);
            // 设置对象键（即上传路径）
            string key = uniqueFileName;
            long offset = 0L;
            long sendLength = fileStream.Length;
            PutObjectRequest request = new PutObjectRequest(_bucket, key, fileStream, offset, sendLength);
            // 设置进度回调
            request.SetCosProgressCallback(delegate (long completed, long total)
            {
                Console.WriteLine($"progress = {completed * 100.0 / total:##.##}%");
            });
            // 执行上传请求
            PutObjectResult result = await Task.Run(() => _cosXml.PutObject(request));
            // 返回 COS 文件的访问路径或者其他信息
            if (result != null)
            {
                string fileUrl = $"https://{_bucket}.cos.{_region}.myqcloud.com/{key}";
                return fileUrl;
            }
            return null;
        }
        catch (COSXML.CosException.CosClientException clientEx)
        {
            Log.Error(clientEx, "CosClientException occurred during file upload.");
            return null;
        }
        catch (COSXML.CosException.CosServerException serverEx)
        {
            Log.Error(serverEx, "CosServerException occurred during file upload.");
            return null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred during file upload.");
            return null;
        }
    }
}
