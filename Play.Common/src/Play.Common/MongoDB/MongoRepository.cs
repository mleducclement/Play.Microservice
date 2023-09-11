﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Play.Common.MongoDB
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
         private readonly IMongoCollection<T> _dbCollection;
         private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;
         
         public MongoRepository(IMongoDatabase database, string collectionName)
         {
             _dbCollection = database.GetCollection<T>(collectionName);
         }
         
         public async Task AddAsync(T entity)
         {
             if (entity is null)
             {
                 throw new ArgumentNullException(nameof(entity));
             }

             await _dbCollection.InsertOneAsync(entity);
         }
         
         public async Task<IReadOnlyCollection<T>> GetAllAsync()
         {
             return await _dbCollection.Find(_filterBuilder.Empty).ToListAsync();
         }

         public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
         {
             return await _dbCollection.Find(filter).ToListAsync();
         }

         public async Task<T> GetOneAsync(Guid id)
         {
             var filter = _filterBuilder.Eq(entity => entity.Id, id);
             return await _dbCollection.Find(filter).FirstOrDefaultAsync();
         }

         public async Task<T> GetOneAsync(Expression<Func<T, bool>> filter)
         {
             return await _dbCollection.Find(filter).FirstOrDefaultAsync();
         }

         public async Task UpdateAsync(T entity)
         {
             if (entity is null)
             {
                 throw new ArgumentNullException(nameof(entity));
             }
             
             var filter = _filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
             await _dbCollection.ReplaceOneAsync(filter, entity);
         }

         public async Task RemoveAsync(Guid id)
         {
             var filter = _filterBuilder.Eq(entity => entity.Id, id);
             await _dbCollection.DeleteOneAsync(filter);
         }
    }
}