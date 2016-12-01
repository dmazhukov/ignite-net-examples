using System;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Apache.Ignite.Core.Cache.Store;
using Apache.Ignite.Core.Common;
using Tim.DataAccess;

namespace IgniteEFCacheStore
{

    /// <summary>
    /// Generic EF cache store.
    /// </summary>
    public class EntityFrameworkCacheStore<TEntity, TContext> : CacheStoreAdapter
        where TEntity : class, new() where TContext : Tim_DB_ContextWrapper
    {
        private readonly Func<TContext> _getContext;

        private readonly Func<TContext, IDbSet<TEntity>> _getDbSet;

        private readonly Func<TEntity, object> _getKey;

        private readonly Action<TEntity, object> _setKey;

        public EntityFrameworkCacheStore(Func<TContext> getContext, Func<TContext, IDbSet<TEntity>> getDbSet,
            Func<TEntity, object> getKey, Action<TEntity, object> setKey)
        {
            if (getContext == null)
                throw new ArgumentNullException(nameof(getContext));

            if (getDbSet == null)
                throw new ArgumentNullException(nameof(getDbSet));

            if (getKey == null)
                throw new ArgumentNullException(nameof(getKey));

            if (setKey == null)
                throw new ArgumentNullException(nameof(setKey));

            _getContext = getContext;
            _getDbSet = getDbSet;
            _getKey = getKey;
            _setKey = setKey;
        }

        public override void LoadCache(Action<object, object> act, params object[] args)
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine("{0}.LoadCache() for {1} called in PID {2}.", GetType().Name, typeof(TEntity).Name, Process.GetCurrentProcess().Id);

            // Load everything from DB to Ignite
            using (var ctx = _getContext())
            {
                //foreach (var entity in _getDbSet(ctx).Take(1000))
                Console.WriteLine($"{_getDbSet(ctx).Count()} {typeof(TEntity).Name}s to load");
                foreach (var entity in _getDbSet(ctx).AsNoTracking())
                {
                    act(_getKey(entity), entity);
                }
            }
            Console.WriteLine("{0}.LoadCache() for {1} took {2}.", GetType().Name, typeof(TEntity).Name, sw.Elapsed);
        }

        public override object Load(object key)
        {
            Console.WriteLine("{0}.Load({1}) called.", GetType().Name, key);

            using (var ctx = _getContext())
            {
                return _getDbSet(ctx).Find(key);
            }
        }

        public override IDictionary LoadAll(ICollection keys)
        {
            Console.WriteLine("{0}.LoadAll({1}) called.", GetType().Name, keys);

            // TODO: Load in one SQL query.
            return keys.OfType<object>().ToDictionary(x => x, Load);
        }

        public override void Write(object key, object val)
        {
            Console.WriteLine("{0}.Write({1}, {2}) called.", GetType().Name, key, val);

            using (var ctx = _getContext())
            {
                _getDbSet(ctx).AddOrUpdate((TEntity)val);

                ctx.SaveChanges();
            }
        }

        public override void WriteAll(IDictionary entries)
        {
            Console.WriteLine("{0}.WriteAll({1}) called.", GetType().Name, entries);

            using (var ctx = _getContext())
            {
                foreach (var entity in entries.Values.OfType<TEntity>())
                {
                    _getDbSet(ctx).AddOrUpdate(entity);

                    ctx.SaveChanges();
                }
            }
        }

        public override void Delete(object key)
        {
            Console.WriteLine("{0}.Delete({1}) called.", GetType().Name, key);

            using (var ctx = _getContext())
            {
                var entity = new TEntity();
                _setKey(entity, key);

                var dbSet = _getDbSet(ctx);

                dbSet.Attach(entity);
                dbSet.Remove(entity);

                ctx.SaveChanges();
            }
        }

        public override void DeleteAll(ICollection keys)
        {
            Console.WriteLine("{0}.DeleteAll({1}) called.", GetType().Name, keys);

            using (var ctx = _getContext())
            {
                var dbSet = _getDbSet(ctx);

                foreach (var key in keys)
                {
                    var entity = new TEntity();
                    _setKey(entity, key);

                    dbSet.Attach(entity);
                    dbSet.Remove(entity);
                }

                ctx.SaveChanges();
            }
        }

        public override void SessionEnd(bool commit)
        {
            // No-op.
        }
    }

    [Serializable]
    public class DynamicEntityFrameworkCacheStoreFactory<TContext> : IFactory<ICacheStore>
        where TContext : Tim_DB_ContextWrapper, new()
    {
        private readonly Type _entityType;
        private readonly Func<TContext> _getContext;

        private readonly Delegate _getDbSet;

        private readonly Delegate _getKey;

        private readonly Delegate _setKey;


        public DynamicEntityFrameworkCacheStoreFactory(Type entityType)
        {
            _entityType = entityType;

            _getContext = () => new TContext() { Configuration = { ProxyCreationEnabled = false }};
            var ti = typeof(IDbSet<>).MakeGenericType(entityType);
            var getDbSetMethod = typeof(TimDbContext).GetMethod("GetDbSet").MakeGenericMethod(entityType);
            var getDbSetFunc = typeof(Func<,>).MakeGenericType(typeof(TContext), ti);
            _getDbSet = Delegate.CreateDelegate(getDbSetFunc, getDbSetMethod);

            var getKeyMethod = typeof(TimDbContext).GetMethod("GetKey").MakeGenericMethod(entityType);
            var getKeyFunc = typeof(Func<,>).MakeGenericType(entityType, typeof(object));
            _getKey = Delegate.CreateDelegate(getKeyFunc, getKeyMethod);

            var setKeyMethod = typeof(TimDbContext).GetMethod("SetKey").MakeGenericMethod(entityType);
            var setKeyAction = typeof(Action<,>).MakeGenericType(entityType, typeof(object));
            _setKey = Delegate.CreateDelegate(setKeyAction, setKeyMethod);
        }
        public ICacheStore CreateInstance()
        {
            var gc = typeof(EntityFrameworkCacheStore<,>).MakeGenericType(_entityType, typeof(TContext));
            return (ICacheStore)Activator.CreateInstance(gc, _getContext, _getDbSet, _getKey, _setKey);
        }
    }
    [Serializable]
    public class EntityFrameworkCacheStoreFactory<TEntity, TContext> : IFactory<ICacheStore>
        where TEntity : class, new() where TContext : Tim_DB_ContextWrapper
    {
        private readonly Func<TContext> _getContext;

        private readonly Func<TContext, IDbSet<TEntity>> _getDbSet;

        private readonly Func<TEntity, object> _getKey;

        private readonly Action<TEntity, object> _setKey;

        public EntityFrameworkCacheStoreFactory(Func<TContext> getContext, Func<TContext, IDbSet<TEntity>> getDbSet,
            Func<TEntity, object> getKey, Action<TEntity, object> setKey)
        {
            if (getContext == null)
                throw new ArgumentNullException(nameof(getContext));

            if (getDbSet == null)
                throw new ArgumentNullException(nameof(getDbSet));

            if (getKey == null)
                throw new ArgumentNullException(nameof(getKey));

            if (setKey == null)
                throw new ArgumentNullException(nameof(setKey));

            _getContext = getContext;
            _getDbSet = getDbSet;
            _getKey = getKey;
            _setKey = setKey;
        }
        public ICacheStore CreateInstance()
        {
            return new EntityFrameworkCacheStore<TEntity, TContext>(_getContext, _getDbSet, _getKey, _setKey);
        }
    }
}
