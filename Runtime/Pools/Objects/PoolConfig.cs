using System;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Pools.Objects
{
    [PublicAPI]
    [Serializable]
    public sealed class PoolConfig<TEntity>
    {
        [field: SerializeField] public TEntity Prefab { get; private set; }
        [field: SerializeField] public int DefaultPoolSize { get; private set; }
        [field: SerializeField] public int MaxPoolSize { get; private set; }
        [field: SerializeField] public Transform Parent { get; private set; }
    }
}