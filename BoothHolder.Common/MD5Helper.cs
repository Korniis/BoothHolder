using System.Security.Cryptography;
using System.Text;

namespace BoothHolder.Common
{
    public class MD5Helper
    {  // 计算字符串的 MD5 值
        public static string GetMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                // 将输入字符串转换为字节数组并计算哈希值
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // 将字节数组转换为十六进制字符串
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        // 计算文件的 MD5 值
        public static string CalculateFileMD5(string filePath)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hashBytes)
                    {
                        sb.Append(b.ToString("x2"));
                    }
                    return sb.ToString();
                }
            }
        }
    }
}
