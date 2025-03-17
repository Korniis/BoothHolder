using System;
using System.IO;
using Aliyun.OSS;
using Microsoft.Extensions.Logging;
using BoothHolder.Common.Configration;

namespace BoothHolder.Common.Oss
{
    public class OssService
    {
        private readonly OssClient _ossClient;
        private readonly ILogger<OssService> _logger;
        public string BucketName;
        public string Endpoint;

        public OssService(ILogger<OssService> logger)
        {
            _logger = logger;
            Endpoint = AppSettings.app("Aliyun:EndPoint");
            var accessKeyId = AppSettings.app("Aliyun:AccessKeyId");
            var accessKeySecret = AppSettings.app("Aliyun:AccessKeySecret");
            BucketName = AppSettings.app("Aliyun:BucketName");

            _ossClient = new OssClient(Endpoint, accessKeyId, accessKeySecret);
        }

        /// <summary>
        /// 上传文件到 OSS
        /// </summary>
        public bool UploadFile(string objectName, Stream fileStream)
        {
            try
            {
                var putObjectResult = _ossClient.PutObject(BucketName, objectName, fileStream);
                _logger.LogInformation($"文件 {objectName} 上传成功, ETag: {putObjectResult.ETag}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"文件 {objectName} 上传失败");
                return false;
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        public MemoryStream DownloadFile(string objectName)
        {
            try
            {
                var obj = _ossClient.GetObject(BucketName, objectName);
                using var requestStream = obj.Content;
                var memoryStream = new MemoryStream();
                requestStream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                _logger.LogInformation($"文件 {objectName} 下载成功");
                return memoryStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"文件 {objectName} 下载失败");
                return null;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public bool DeleteFile(string objectName)
        {
            try
            {
                _ossClient.DeleteObject(BucketName, objectName);
                _logger.LogInformation($"文件 {objectName} 删除成功");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"文件 {objectName} 删除失败");
                return false;
            }
        }
    }
}
