using System;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme
{
    [Serializable]
    public sealed class ThemeStateMappingComponent<TEnum> where TEnum : unmanaged, Enum
    {
        [field: SerializeField] public ThemeStateMappingGeneric<TEnum> Mapping { get; private set; }
        [field: SerializeField] public ThemeComponent TargetComponent { get; private set; }

        public void SetComponentForState(TEnum state)
        {
            Mapping.SetComponentForState(state, TargetComponent);
        }
    }
}