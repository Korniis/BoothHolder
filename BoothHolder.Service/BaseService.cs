using BoothHolder.IService;
using BoothHolder.Repository;
using MapsterMapper;
using System.Linq.Expressions;

namespace BoothHolder.Service
{


    public class BaseService<TEntity, TDto> : IBaseService<TEntity, TDto> where TEntity : class, new() where TDto : class, new()
    {
        private readonly IBaseRepository<TEntity> _repository;
        private readonly IMapper _mapper;

        public BaseService(IBaseRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // 插入
        public async Task<long> CreateAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            return await _repository.CreateAsync(entity);
        }

        // 删除
        public async Task<bool> DeleteAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            return await _repository.DeleteAsync(entity);
        }

        // 更新
        public async Task<bool> UpdateAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            return await _repository.UpdateAsync(entity);
        }

        // 查询所有
        public async Task<List<TEntity>> SelectAllAsync()
        {
            return await _repository.SelectAllAsync();

        }

        // 根据 ID 查询单条记录
        public async Task<TEntity> SelectOneByIdAsync(long id)
        {
            return await _repository.SelectOneByIdAsync(id);

        }

        // 根据条件查询单条记录
        public async Task<TEntity> SelectOneAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _repository.SelectOneAsync(expression);

        }

        // 根据条件查询所有记录
        public async Task<List<TEntity>> SelectAllAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _repository.SelectAllAsync(expression);
        }
    }


}


