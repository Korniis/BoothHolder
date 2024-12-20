using BoothHolder.Common.Configration;
using COSXML;
using COSXML.Auth;

namespace BoothHolder.Common.Oss
{
    public static class UploadObject
    {


        public static CosXml InitCosXml()
        {
            CosXml cosXml;
            string region = AppSettings.app("TencentCloud:Region");
            CosXmlConfig config = new CosXmlConfig.Builder()
                .SetRegion(region) // 设置默认的地域, COS 地域的简称请参照 https://cloud.tencent.com/document/product/436/6224
                .Build();
            string secretId = AppSettings.app("TencentCloud:SecretId"); // 云 API 密钥 SecretId, 获取 API 密钥请参照 https://console.cloud.tencent.com/cam/capi
            string secretKey = AppSettings.app("TencentCloud:SecretKey"); // 云 API 密钥 SecretKey, 获取 API 密钥请参照 https://console.cloud.tencent.com/cam/capi
            long durationSecond = 600; //每次请求签名有效时长，单位为秒
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, durationSecond);
            cosXml = new CosXmlServer(config, qCloudCredentialProvider);
            return cosXml;
        }
    }
}
