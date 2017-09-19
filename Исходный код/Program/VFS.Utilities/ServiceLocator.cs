namespace VFS.Utilities
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Предоставляет доступ к зарегистрированным экземплярам сервисов
    /// из любого места приложения.
    /// </summary>
    public class ServiceLocator
    {
        private static volatile ServiceLocator instance;

        private static readonly object lockObject = new object();

        private readonly Dictionary<Type, object> sharedInstances =
            new Dictionary<Type, object>();

        private ServiceLocator()
        {
        }

        /// <summary>
        /// Получить единственный экземпляр <see cref="ServiceLocator" />'а.
        /// </summary>
        public static ServiceLocator Instance
        {
            get
            {
                if (ServiceLocator.instance != null)
                {
                    return ServiceLocator.instance;
                }

                lock (ServiceLocator.lockObject)
                {
                    if (ServiceLocator.instance == null)
                    {
                        ServiceLocator.instance = new ServiceLocator();
                    }
                }

                return ServiceLocator.instance;
            }
        }

        /// <summary>
        /// Зарегистрировать один экземпляр для общего использования.
        /// </summary>
        /// <typeparam name="T">Тип (как правило, тип интерфейса).</typeparam>
        /// <param name="sharedInstance">Экземпляр.</param>
        /// <exception cref="InvalidOperationException">
        /// Попытка зарегистрировать более одного экземпляра для одного и того же типа.
        /// </exception>
        public void RegisterService<T>(T sharedInstance)
        {
            if (sharedInstance == null)
            {
                throw new ArgumentNullException(
                    nameof(sharedInstance),
                    "Невозможно зарегистрировать сервис. Экземпляр сервиса не должен быть null.");
            }

            if (this.sharedInstances.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException(
                    $"Для типа {typeof(T).Name} уже зарегистрирован экземпляр.");
            }

            this.sharedInstances[typeof(T)] = sharedInstance;
        }

        /// <summary>
        /// Получить зарегистрированный для общего использования экземпляр ("сервис").
        /// </summary>
        /// <typeparam name="T">Тип, экземпляр которого запрашивается.</typeparam>
        /// <returns>Зарегистрированный для общего использования экземпляр</returns>
        public T GetService<T>()
        {
            if (!this.sharedInstances.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException(
                    $"Для типа {typeof(T).Name} не зарегистрирован общий экземпляр.");
            }

            return (T) this.sharedInstances[typeof(T)];
        }
    }
}