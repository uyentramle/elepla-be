﻿using Elepla.Domain.Entities;
using Elepla.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<Pagination<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null, // Optional parameter for pagination (page number)
            int? pageSize = null);  // Optional parameter for pagination (number of records per page)
        Task<TEntity?> GetByIdAsync(object id);
        Task<TEntity?> GetByIdAsync(object id, Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(List<TEntity> entities);
        void Update(TEntity entity);
        void UpdateRange(List<TEntity> entities);
        void SoftRemove(TEntity entity);
        void SoftRemoveRange(List<TEntity> entities);
        void Delete(object id);
        void Delete(TEntity entity);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null);
        //findAsync
        Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> filter = null);
    }
}
