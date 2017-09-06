namespace VFS.Interfaces.Service
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Результат выполнения операции.
    /// </summary>
    [DataContract]
    public class StandardOperationResult
    {
        /// <summary>
        /// Создать стандартный результат выполнения операции.
        /// </summary>
        /// <param name="resultMessage">Сообщение о результате, если операция выполнена.</param>
        /// <param name="errorMessage">Сообщение об ошибке, если операция не выполнена.</param>
        public StandardOperationResult(
            string resultMessage,
            string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(resultMessage)
                && string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new InvalidOperationException(
                    "Должно быть передано одно из значений: результат операции или сообщение об ошибке.");
            }

            if (!string.IsNullOrWhiteSpace(resultMessage)
                && !string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new InvalidOperationException(
                    "Результат операции и сообщение об ошибке не могут быть переданы одновременно."
                    + " Операция либо выполнена, либо не выполнена.");
            }

            this.ResultMessage =
                string.IsNullOrWhiteSpace(resultMessage)
                    ? null
                    : resultMessage;

            this.ErrorMessage =
                string.IsNullOrWhiteSpace(errorMessage)
                    ? null
                    : errorMessage;
        }

        /// <summary>
        /// Сообщение о результате операции.
        /// </summary>
        [DataMember]
        public string ResultMessage { get; private set; }

        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Операция выполнена успешно.
        /// </summary>
        public bool Succeed => !string.IsNullOrWhiteSpace(this.ResultMessage);
    }
}