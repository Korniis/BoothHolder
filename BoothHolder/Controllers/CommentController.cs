using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Model.VO;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace BoothHolder.UserApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
      //  private readonly ISqlSugarClient _db;
      private   readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public CommentController(ICommentService commentService, IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
            //  _db = db;
        }



        /// <summary>
        /// 获取指定展位的评论列表（包含评论用户信息）
        /// </summary>
        [HttpGet("{boothId}/{enterpriseId}")]
        public async Task<ApiResult> GetCommentsByBooth(long boothId,long enterpriseId)
        {


            //var comments = await _commentService.SelectAllAsync(c=>c.BoothId==boothId&&c.EnterpriseId == enterpriseId);
            List<Comment> comments = await _commentService.SelectAllWithUserAsync(boothId,  enterpriseId);


            //var comments = await _db.Queryable<Comment>()
            //    .Includes(c => c.CommentUser)
            //    .Where(c => c.BoothId == boothId && !c.IsDeleted)
            //    .OrderByDescending(c => c.CreateTime)
            //    .ToListAsync();
            var res = _mapper.Map<List<CommentVO>>(comments);
            return ApiResult.Success(res);
        }
        
        
        [HttpPost]
        [Authorize]

        public async  Task<ApiResult> AddComment(CommentDTO commentDTO)
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var excomment = await _commentService.SelectOneAsync(c=>c.BoothId==commentDTO.BoothId
                                                                  &&c.EnterpriseId==commentDTO.EnterpriseId 
                                                                  &&c.UserId==userId);

            if (excomment != null)
                return ApiResult.Error("您已经评论过");

            var comment  = _mapper.Map<Comment>(commentDTO);
             comment.UserId = userId;
             var result=  await _commentService.CreateAsync(comment);

            return ApiResult.Success(result);



        }
    }
}
