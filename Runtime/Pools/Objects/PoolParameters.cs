using System;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;

namespace CustomUtils.Runtime.Pools.Objects
{
    [PublicAPI]
    public struct PoolParameters<TEntity>
    {
        public TEntity Prefab { get; }
        public int DefaultPoolSize { get; }
        public int MaxPoolSize { get; }
        public Action<TEntity> OnCreateCallback { get; }
        public Action<TEntity> OnGetCallback { get; }
        public Action<TEntity> OnReleaseCallback { get; }
        public Action<TEntity> OnDestroyCallback { get; }
        public Transform Parent { get; }
        public IObjectResolver ObjectResolver { get; }

        public PoolParameters(
            TEntity prefab,
            int defaultPoolSize = 10,
            int maxPoolSize = 100,
            Action<TEntity> onCreateCallback = null,
            Action<TEntity> onGetCallback = null,
            Action<TEntity> onReleaseCallback = null,
            Action<TEntity> onDestroyCallback = null,
            Transform parent = null,
            IObjectResolver objectResolver = null)
        {
            Prefab = prefab;
            DefaultPoolSize = defaultPoolSize;
            MaxPoolSize = maxPoolSize;
            OnCreateCallback = onCreateCallback;
            OnGetCallback = onGetCallback;
            OnReleaseCallback = onReleaseCallback;
            OnDestroyCallback = onDestroyCallback;
            Parent = parent;
            ObjectResolver = objectResolver;
        }

        public PoolParameters(PoolConfig<TEntity> poolConfig,
            Action<TEntity> onCreateCallback = null,
            Action<TEntity> onGetCallback = null,
            Action<TEntity> onReleaseCallback = null,
            Action<TEntity> onDestroyCallback = null,
            IObjectResolver objectResolver = null)
        {
            Prefab = poolConfig.Prefab;
            DefaultPoolSize = poolConfig.DefaultPoolSize;
            MaxPoolSize = poolConfig.MaxPoolSize;
            OnCreateCallback = onCreateCallback;
            OnGetCallback = onGetCallback;
            OnReleaseCallback = onReleaseCallback;
            OnDestroyCallback = onDestroyCallback;
            Parent = poolConfig.Parent;
            ObjectResolver = objectResolver;
        }
    }
}