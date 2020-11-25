using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskChecker.Interface
{
    public interface IDispatcherService
    {
        /// <summary>
        /// Dispatches the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        void Dispatch(Action action);

        /// <summary>
        /// Dispatches the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="argument">The argument.</param>
        void Dispatch(Action<object> action, object argument);

        /// <summary>
        /// Dispatches the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="arguments">The arguments.</param>
        void Dispatch(Delegate action, object[] arguments);

        /// <summary>
        /// Dispatches the specified action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="param">The parameter.</param>
        void Dispatch<T>(Action<T> action, T param);

        /// <summary>
        /// Invokes the specified function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        T Invoke<T>(Func<T> func);

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="argument">The argument.</param>
        /// <returns></returns>
        T Invoke<T>(Func<object, T> action, object argument);

        /// <summary>
        /// Invokes the specified function.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        object Invoke(Delegate func, object[] arguments);

        /// <summary>
        /// Invokes the specified function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="func">The function.</param>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        R Invoke<T, R>(Func<T, R> func, T param);

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        void Invoke(Action action);

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="argument">The argument.</param>
        void Invoke(Action<object> action, object argument);

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="param">The parameter.</param>
        void Invoke<T>(Action<T> action, T param);
    }
}
