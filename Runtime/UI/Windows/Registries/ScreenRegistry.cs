using System;
using System.Threading;
using CustomUtils.Runtime.AddressableSystem;
using CustomUtils.Runtime.UI.Windows.Windows.Base;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using R3;
using UnityEngine;
using VContainer;

namespace CustomUtils.Runtime.UI.Windows.Registries
{
    [PublicAPI]
    internal sealed class ScreenRegistry : WindowRegistry<SharedScreenBase>
    {
        private readonly ReactiveProperty<Type> _currentScreenType;

        internal ScreenRegistry(
            ReactiveProperty<Type> currentScreenType,
            Transform container,
            IObjectResolver objectResolver,
            IAddressablesLoader addressablesLoader)
            : base(container, objectResolver, addressablesLoader)
        {
            _currentScreenType = currentScreenType;
        }

        protected override void OnRegistered(SharedScreenBase sharedScreenBase)
        {
            if (sharedScreenBase.InitialWindow)
            {
                _currentScreenType.Value = sharedScreenBase.GetType();
                return;
            }

            sharedScreenBase.HideImmediately();
        }

        protected override async UniTask<SharedScreenBase> OpenWindow(
            SharedScreenBase sharedScreenBase,
            CancellationToken token)
        {
            if (currentWindow)
                await currentWindow.HideAsync(token);

            currentWindow = sharedScreenBase;
            _currentScreenType.Value = sharedScreenBase.GetType();
            await sharedScreenBase.ShowAsync(token);
            return sharedScreenBase;
        }
    }
}