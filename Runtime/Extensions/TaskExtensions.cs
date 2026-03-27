using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CustomUtils.Runtime.ResponseTypes;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="Task"/>.
    /// </summary>
    [PublicAPI]
    public static class TaskExtensions
    {
        /// <summary>
        /// Converts a Task to UniTask with external cancellation token support.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="task">The Task to convert.</param>
        /// <param name="token">The cancellation token to attach.</param>
        /// <returns>A UniTask that can be canceled via the provided token.</returns>
        public static UniTask<T> AsUniTask<T>(this Task<T> task, CancellationToken token) =>
            task.AsUniTask().AttachExternalCancellation(token);

        /// <summary>
        /// Converts a Task to UniTask with external cancellation token support.
        /// </summary>
        /// <param name="task">The Task to convert.</param>
        /// <param name="token">The cancellation token to attach.</param>
        /// <returns>A UniTask that can be canceled via the provided token.</returns>
        public static UniTask AsUniTask(this Task task, CancellationToken token) =>
            task.AsUniTask().AttachExternalCancellation(token);

        /// <summary>
        /// Executes the task and wraps the result in a <see cref="Result{TResult}"/>, suppressing any exceptions.
        /// Logs the exception and caller context on failure.
        /// </summary>
        /// <typeparam name="TResult">The type of the task result.</typeparam>
        /// <param name="task">The task to execute.</param>
        /// <param name="methodName">Automatically captured caller method name.</param>
        /// <param name="filePath">Automatically captured caller file path.</param>
        /// <returns>
        /// <see cref="Result{TResult}.Valid"/> with the result on success;
        /// <see cref="Result{TResult}.Invalid"/> on failure.
        /// </returns>
        public static async UniTask<Result<TResult>> SuppressAsync<TResult>(
            this Task<TResult> task,
            [CallerMemberName] string methodName = "",
            [CallerFilePath] string filePath = "")
        {
            try
            {
                return Result<TResult>.Valid(await task);
            }
            catch (Exception exception)
            {
                LogError(exception, methodName, filePath);
                return Result<TResult>.Invalid();
            }
        }

        /// <summary>
        /// Executes the task, suppressing any exceptions.
        /// Logs the exception and caller context on failure.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="methodName">Automatically captured caller method name.</param>
        /// <param name="filePath">Automatically captured caller file path.</param>
        /// <returns><see langword="true"/> on success; <see langword="false"/> on failure.</returns>
        public static async UniTask<bool> SuppressAsync(
            this Task task,
            [CallerMemberName] string methodName = "",
            [CallerFilePath] string filePath = "")
        {
            try
            {
                await task;
                return true;
            }
            catch (Exception exception)
            {
                LogError(exception, methodName, filePath);
                return false;
            }
        }

        private static void LogError(Exception exception, string methodName, string filePath)
        {
            var className = Path.GetFileNameWithoutExtension(filePath);
            Debug.LogException(exception);
            Debug.LogError($"[{className}::{methodName}] Operation failed with: {exception.Message}");
        }
    }
}