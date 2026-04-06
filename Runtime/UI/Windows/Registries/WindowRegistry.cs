using System;
using System.Collections.Generic;
using System.Threading;
using CustomUtils.Runtime.AddressableSystem;
using CustomUtils.Runtime.UI.Windows.Windows.Base;
using CustomUtils.Runtime.UI.Windows.Windows.Parameterized;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace CustomUtils.Runtime.UI.Windows.Registries
{
    internal abstract class WindowRegistry<TWindow> where TWindow : WindowBase
    {
        protected TWindow currentWindow;

        private readonly Dictionary<Type, TWindow> _windows = new();

        private readonly Transform _container;
        private readonly IObjectResolver _objectResolver;
        private readonly IAddressablesLoader _addressablesLoader;

        internal WindowRegistry(
            Transform container,
            IObjectResolver objectResolver,
            IAddressablesLoader addressablesLoader)
        {
            _container = container;
            _objectResolver = objectResolver;
            _addressablesLoader = addressablesLoader;
        }

        internal async UniTask LoadAsync(List<AssetReferenceT<GameObject>> references, CancellationToken token)
        {
            foreach (var reference in references)
            {
                var loaded = await _addressablesLoader.LoadAsync<GameObject>(reference, token);
                var created = _objectResolver.Instantiate(loaded, _container);

                if (!created.TryGetComponent<TWindow>(out var window))
                {
                    Debug.LogError("[WindowRegistry::LoadAsync] Loaded GameObject does not contain " +
                                   $"{typeof(TWindow).Name} component: {created.name}");
                    continue;
                }

                _windows[window.GetType()] = window;
                window.BaseInitialize();
                window.Initialize();

                OnRegistered(window);
            }
        }

        internal async UniTask<TWindow> Open<TConcreteWindow>() where TConcreteWindow : TWindow
        {
            if (!TryGet<TConcreteWindow>(out var window))
                return null;

            return await OpenWindow(window);
        }

        internal async UniTask<TWindow> Open<TConcreteWindow, TParameters>(TParameters parameters)
            where TConcreteWindow : TWindow, IParameterizedWindow<TParameters>
        {
            if (!TryGet<TConcreteWindow>(out var window))
                return null;

            ((IParameterizedWindow<TParameters>)window).SetParameters(parameters);
            return await OpenWindow(window);
        }

        internal void HideCurrent()
        {
            if (!currentWindow)
                return;

            currentWindow.HideImmediately();
            currentWindow = null;
        }

        protected abstract void OnRegistered(TWindow window);
        protected abstract UniTask<TWindow> OpenWindow(TWindow window);

        private bool TryGet<TConcreteWindow>(out TWindow window) where TConcreteWindow : TWindow
        {
            if (_windows.TryGetValue(typeof(TConcreteWindow), out window))
                return true;

            Debug.LogError($"[WindowRegistry] No window registered for type: {typeof(TConcreteWindow)}");
            return false;
        }
    }
}