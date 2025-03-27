using System.Linq.Expressions;

namespace BoothHolder.IService
{


    public interface IBaseService<TEntity, TDto> where TEntity : class, new() where TDto : class, new()
    {
        // 插入
        Task<long> CreateAsync(TDto dto);

        // 删除
        Task<bool> DeleteAsync(TDto dto);

        // 更新
        Task<bool> UpdateAsync(TDto dto);

        // 查询所有
        Task<List<TEntity>> SelectAllAsync();

        // 根据 ID 查询单条记录
        Task<TEntity> SelectOneByIdAsync(long id);

        // 根据条件查询单条记录
        Task<TEntity> SelectOneAsync(Expression<Func<TEntity, bool>> expression);

        // 根据条件查询所有记录
        Task<List<TEntity>> SelectAllAsync(Expression<Func<TEntity, bool>> expression);
    }

}
