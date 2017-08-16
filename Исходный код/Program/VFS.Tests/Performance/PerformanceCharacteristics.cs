namespace VFS.Tests.Performance
{
    /// <summary>
    /// Набор характеристик производительности.
    /// </summary>
    internal class PerformanceCharacteristics
    {
        /// <summary>
        /// Количество пользователей.
        /// </summary>
        public int UsersCount { get; set; }

        /// <summary>
        /// Количество запросов, выполняемых каждым пользователем.
        /// </summary>
        public int RequestsPerUser { get; set; }

        /// <summary>
        /// Среднее выполнение операции сервером в миллисекундах.
        /// </summary>
        public int ServerOperationTimeInMillisec { get; set; }

        /// <summary>
        /// Максимальное количество допустимых параллельных запросов к серверу.
        /// </summary>
        public int MaxParallelQueries { get; set; }

        /// <summary>
        /// Промежуток времени между запросами каждого отдельного пользователя
        /// (в милисекундах).
        /// </summary>
        public int IntervalBetweenUserRequestsInMillisec { get; set; }
    }
}
