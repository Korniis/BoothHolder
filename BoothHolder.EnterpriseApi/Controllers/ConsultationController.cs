using BoothHolder.Common.Response;
using BoothHolder.Model.Entity;
using BoothHolder.Model.Status;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Claims;

namespace BoothHolder.EnterpriseApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {

        private     readonly  ISqlSugarClient _db;

        public ConsultationController(ISqlSugarClient db)
        {
            _db = db;
        }

        //[HttpPost]
        //public  async Task<ApiResult>  Recall(long Id,string recall)
        //{
        //    //
        //     await _db.Updateable<Consultation>.

        //}
        [HttpPost]
        [Authorize(Roles = "Enterprise")] // 限制只有企业用户可以回复
        [HttpPut("Recall")]
        public async Task<ApiResult> Recall([FromBody] RecallDto dto)
        {
            try
            {
                var enterpriseId = Convert.ToInt64(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

                if (string.IsNullOrWhiteSpace(dto.Recall))
                {
                    return ApiResult.Error("回复原因不能为空");
                }

                if (dto.Recall.Length > 500)
                {
                    return ApiResult.Error("回复原因长度不能超过500个字符");
                }

                var consultation = await _db.Queryable<Consultation>()
                    .FirstAsync(c => c.Id == dto.Id && c.ReceiveId == enterpriseId);

                if (consultation == null)
                {
                    return ApiResult.Error("咨询记录不存在或无权操作");
                }

                if (consultation.Status != (int)ConsultationStatus.Pending)
                {
                    return ApiResult.Error("只有已完成的咨询才能回复");
                }

                var updateResult = await _db.Updateable<Consultation>()
                    .SetColumns(c => new Consultation()
                    {
                        Recall = dto.Recall,
                        Status = (int)ConsultationStatus.Confirmed,
                        UpdateTime = DateTime.Now
                    })
                    .Where(c => c.Id == dto.Id && c.ReceiveId == enterpriseId)
                    .ExecuteCommandAsync();

                return updateResult > 0
                    ? ApiResult.Success("咨询已成功回复")
                    : ApiResult.Error("回复操作失败");
            }
            catch (Exception ex)
            {
                return ApiResult.Error("系统错误，请稍后重试");
            }
        }

        [HttpGet]
        [Authorize] // 需要用户登录
        public async Task<ApiResult> GetConsultationByUserId()
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var consultations = await _db.Queryable<Consultation>().Where(x => x.ReceiveId == userId).ToListAsync();
            return ApiResult.Success(consultations);

        }
        [HttpGet]
        //  [Authorize]
        public async Task<ApiResult> GetConsultationInfo(long conId)
        {
            try
            {
                var consultation = await _db.Queryable<Consultation>().Where(x => x.Id == conId).FirstAsync();

                var enterprise = await _db.Queryable<EnterpriseApplication>().Where(x => x.UserId == consultation.ReceiveId).FirstAsync();
                var booth = await _db.Queryable<Booth>().Where(x => x.UserId == enterprise.UserId).FirstAsync();
                return ApiResult.Success(new { consultation, enterprise.Id, enterprise.EnterpriseName, booth.BoothName });
            }
            catch (Exception ex)
            {
                return ApiResult.Error("无此数据");
            }


        }
      

    }
    public class RecallDto
    {
        public long Id { get; set; }
        public string Recall { get; set; }
    }
}
