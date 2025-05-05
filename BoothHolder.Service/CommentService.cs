using BoothHolder.IService;
using BoothHolder.Model.Entity;
using BoothHolder.Repository;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Service
{
    public class CommentService : BaseService<Comment, Comment>, ICommentService 

    {
        IBaseRepository<Comment> _repository;
        ICommentRepository  _commentRepository;
        IMapper _mapper;


        public CommentService(IBaseRepository<Comment> repository, IMapper mapper, ICommentRepository commentRepository) : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _commentRepository = commentRepository;
        }

        public async Task<List<Comment>> SelectAllWithUserAsync(long boothId, long enterpriseId)
        {
            var Comments = await _commentRepository.SelectAllWithUserAsync( boothId,  enterpriseId);
            return Comments;
        }
    }
}
