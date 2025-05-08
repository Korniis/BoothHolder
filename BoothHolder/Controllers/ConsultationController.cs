using BoothHolder.Common.Response;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Model.Status;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Claims;

namespace BoothHolder.UserApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {

        private readonly ISqlSugarClient _db;
        private readonly IMapper _mapper;

        public ConsultationController(ISqlSugarClient db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResult> Create([FromBody] ConsultationDTO modelDTO)
        {
            try
            {
                modelDTO.SenderId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

                if (modelDTO.SenderId <= 0 || modelDTO.ReceiveId <= 0)
                    return ApiResult.Error("发送方或接收方ID无效");

                var model = _mapper.Map<Consultation>(modelDTO);

                var result = await _db.Insertable(model).ExecuteReturnIdentityAsync();
                return ApiResult.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResult.Error( "发生错误");
            }
        }

        [HttpPost]
        [Authorize] // 需要用户登录
        public async Task<ApiResult> UserConfirmReceipt(long consultationId)
        {
            try
            {
                // 获取当前用户ID
                var userId = Convert.ToInt64(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

                // 查询咨询记录
                var consultation = await _db.Queryable<Consultation>()
                    .FirstAsync(c => c.Id == consultationId && c.SenderId == userId);

                if (consultation == null)
                {
                    return ApiResult.Error("咨询记录不存在或无权操作");
                }

                // 检查当前状态是否允许确认
                if (consultation.Status != (int)ConsultationStatus.Confirmed)

                {
                    return ApiResult.Error("只有已完成或已召回的咨询才能确认接收");
                }

                // 更新确认状态
                var updateResult = await _db.Updateable<Consultation>()
                    .SetColumns(c => new Consultation()
                    {
                        Status = (int)ConsultationStatus.Completed,
                        UpdateTime = DateTime.Now
                    })
                    .Where(c => c.Id == consultationId && c.SenderId == userId)
                    .ExecuteCommandAsync();

                if (updateResult > 0)
                {
                    // 这里可以添加通知企业的逻辑
                    return ApiResult.Success("咨询接收确认成功");
                }
                return ApiResult.Error("确认操作失败");
            }
            catch (Exception ex)
            {
                return ApiResult.Error("系统错误，请稍后重试");
            }
        }
        [HttpGet]
        [Authorize] // 需要用户登录
        public  async Task<ApiResult> GetConsultationByUserId ()
        {
          var userId=   Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var consultations = await _db.Queryable<Consultation>().Where(x=>x.SenderId ==userId).ToListAsync();
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
                return ApiResult.Success(new { consultation, enterprise.Id, enterprise.EnterpriseName ,booth.BoothName});
            }
            catch (Exception ex) {
                return ApiResult.Error("无此数据");
            }
          

        }



    }
}
