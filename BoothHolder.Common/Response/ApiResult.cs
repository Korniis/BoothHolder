namespace BoothHolder.Common.Response
{
    public record ApiResult
    {
        public int Code { get; set; }
        public string? Message { get; set; }
        public dynamic? Data { get; set; }


        /// <summary>
        /// 成功后返回数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ApiResult Success(dynamic data)
        {
            return new ApiResult
            {
                Code = 200,
                Data = data,
                Message = "操作成功",
            };
        }
        /// <summary>
        /// 失败后返回数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ApiResult Error(string msg)
        {
            return new ApiResult
            {
                Code = 500,
                Data = null,
                Message = msg,
            };
        }
        public static ApiResult NotFound(string msg)
        {
            return new ApiResult
            {
                Code = 404,
                Data = null,
                Message = msg,
            };
        }
        public static ApiResult AuthError(string msg)
        {
            return new ApiResult
            {
                Code = 401,
                Data = null,
                Message = msg,
            };
        }
    }
}
