using BoothHolder.Common.Response;
using BoothHolder.Model.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Claims;

namespace BoothHolder.AdminApi.Controllers
{
    [ApiController]
    [Route("")]
    public class ComplaintController : ControllerBase
    {
        private readonly ISqlSugarClient _db;

        public ComplaintController(ISqlSugarClient db)
        {
            _db = db;
        }
        [HttpGet("api/admin/complaints")]
        public async Task<ApiResult> GetComplaints()
        {
            var list = await _db.Queryable<Complaint>().Includes(x=>x.Reporter).OrderBy(it => it.CreateTime, OrderByType.Desc).ToListAsync();
            return ApiResult.Success(list);
        }
    }
}