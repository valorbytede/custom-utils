using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableCollections;
using R3;

namespace CustomUtils.Runtime.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IReadOnlyObservableDictionary{TKey,TValue}"/>.
    /// </summary>
    [PublicAPI]
    public static class ReadOnlyObservableDictionaryExtensions
    {
        /// <summary>
        /// Observes all current and future items in the dictionary.
        /// </summary>
        /// <remarks>
        /// Immediately emits all existing key-value pairs upon subscription, then continues to emit
        /// any newly added pairs as they are inserted into the dictionary.
        /// </remarks>
        /// <typeparam name="TKey">The type of the dictionary keys.</typeparam>
        /// <typeparam name="TValue">The type of the dictionary values.</typeparam>
        /// <param name="dictionary">The observable dictionary to observe.</param>
        /// <returns>An observable sequence of key-value pairs, starting with the current state.</returns>
        public static Observable<KeyValuePair<TKey, TValue>> ObserveAddWithCurrentState<TKey, TValue>(
            this IReadOnlyObservableDictionary<TKey, TValue> dictionary) =>
            Observable.Create<KeyValuePair<TKey, TValue>, IReadOnlyObservableDictionary<TKey, TValue>>(
                dictionary,
                static (observer, dictionary) =>
                {
                    foreach (var pair in dictionary)
                        observer.OnNext(pair);

                    return dictionary.ObserveChanged()
                        .Select(static changedEvent => changedEvent.NewItem)
                        .Subscribe(observer);
                });
    }
}