using System.Linq.Expressions;
using Tabele_ligowe.Extensions;
using Tabele_ligowe.Models;

namespace Tabele_ligowe.Services
{
    public interface IRepositoryService<T> where T : IEntity
    {
        IQueryable<T> GetAllRecords();

        T GetSingle(Guid id);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        ServiceResult Add(T entity);
        ServiceResult Delete(T entity);
        ServiceResult Edit(T entity);
        ServiceResult Save(T entity);
    }
}
