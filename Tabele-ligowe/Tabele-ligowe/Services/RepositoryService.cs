using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tabele_ligowe.Data;
using Tabele_ligowe.Extensions;
using Tabele_ligowe.Models;

namespace Tabele_ligowe.Services
{
    public class RepositoryService<T> : IRepositoryService<T> where T : class, IEntity
    {
        protected ApplicationDbContext _context;
        protected DbSet<T> _set;

        public RepositoryService(ApplicationDbContext context)
        {
            _context = context;
            _set = _context.Set<T>();
        }

        public virtual ServiceResult Add(T entity)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                _set.Add(entity);
                result = Save(entity);
            }
            catch (Exception e)
            {
                result.Result = ServiceResultStatus.Error;
                result.Messages.Add(e.Message);
            }

            return result;
        }

        public ServiceResult Delete(T entity)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                _set.Remove(entity);
                result = Save(entity);
            }
            catch (Exception e)
            {
                result.Result = ServiceResultStatus.Error;
                result.Messages.Add(e.Message);
            }

            return result;
        }

        public ServiceResult Edit(T entity)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                _context.Entry(entity).State = EntityState.Modified;

                result = Save(entity);
            }
            catch (Exception e)
            {
                result.Result = ServiceResultStatus.Error;
                result.Messages.Add(e.Message);
            }

            return result;
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _set.Where(predicate);

            return query;
        }

        public IQueryable<T> GetAllRecords()
        {
            return _set;
        }

        public T GetSingle(Guid id)
        {
            var result = _set.FirstOrDefault(r => r.Id == id);

            return result;
        }

        public ServiceResult Save(T entity)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                result.Result = ServiceResultStatus.Error;
                result.Messages.Add(e.Message);
            }

            return result;
        }
    }
}
