using BoothHolder.Repository;
using SqlSugar;
using System.Linq.Expressions;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
{
    private readonly ISqlSugarClient _db;

    public BaseRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    // 插入
    public async Task<long> CreateAsync(TEntity entity)
    {
        return await _db.Insertable(entity).ExecuteReturnBigIdentityAsync();
    }
    public async Task<int> CreateAsync(List<TEntity> entities)
    {
        return await _db.Insertable(entities).ExecuteCommandAsync();
    }

    // 删除
    public async Task<bool> DeleteAsync(TEntity entity)
    {

        var result = await _db.Deleteable(entity).ExecuteCommandAsync();
        return result > 0;
    }

    // 更新
    public async Task<bool> UpdateAsync(TEntity entity)
    {
        var result = await _db.Updateable(entity).ExecuteCommandAsync();
        return result > 0;
    }

    // 查询所有
    public async Task<List<TEntity>> SelectAllAsync()
    {
        return await _db.Queryable<TEntity>().ToListAsync();
    }
    public  List<TEntity> SelectAllObj()
    {
        return  _db.Queryable<TEntity>().ToList();
    }
    // 根据 ID 查询单条记录
    public async Task<TEntity> SelectOneByIdAsync(long id)
    {
        return await _db.Queryable<TEntity>().InSingleAsync(id);
    }

    // 根据条件查询单条记录
    public async Task<TEntity> SelectOneAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await _db.Queryable<TEntity>().FirstAsync(expression);
    }

    // 根据条件查询所有记录
    public async Task<List<TEntity>> SelectAllAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await _db.Queryable<TEntity>().Where(expression).ToListAsync();
    }
}