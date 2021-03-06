﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EmployeeRegister.Core.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EmployeeRegister.Core.Repositories
{
    public abstract class GenericRepository<TContext, TEntity> : IGenericRepository<TContext, TEntity> where TContext : DbContext where TEntity : class
    {
        public TContext Context { get; set; }
        public DbSet<TEntity> DbSet { get; set; }

        protected GenericRepository(TContext dbContext)
        {
            Context = dbContext;
            DbSet = Context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> GetAll(bool noTracking = true)
        {
            return noTracking
                ? DbSet.AsNoTracking()
                : DbSet.AsTracking();
        }

        public virtual IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate, bool noTracking = true)
        {
            return noTracking
                ? DbSet.AsNoTracking().Where(predicate)
                : DbSet.AsTracking().Where(predicate);
        }

        public virtual async Task<TEntity> GetAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<EntityEntry<TEntity>> CreateAsync(TEntity entity)
        {
            return await DbSet.AddAsync(entity);
        }

        public virtual void Update(TEntity entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual async Task Delete(int id)
        {
            var entityToDelete = await DbSet.FindAsync(id);
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }

            DbSet.Remove(entityToDelete);
        }

        public virtual void Detach(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Detached;
        }

        public virtual async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}
