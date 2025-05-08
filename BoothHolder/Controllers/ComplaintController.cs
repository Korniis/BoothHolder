using BoothHolder.Model.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Claims;

namespace BoothHolder.UserApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplaintController : ControllerBase
    {
        private readonly ISqlSugarClient _db;

        public ComplaintController(ISqlSugarClient db)
        {
            _db = db;
        }

        [HttpPost("submit")]
        [Authorize]
        public async Task<IActionResult> SubmitComplaint([FromBody] Complaint complaint)
        {
             complaint.UserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (string.IsNullOrWhiteSpace(complaint.Title) || string.IsNullOrWhiteSpace(complaint.Content))
            {
                return BadRequest(new { code = 400, message = "标题或内容不能为空" });
            }
             
            complaint.CreateTime = DateTime.Now;
            var result = await _db.Insertable(complaint).ExecuteReturnIdentityAsync();

            return Ok(new { code = 200, message = "投诉提交成功", id = result });
        }
    }
}
