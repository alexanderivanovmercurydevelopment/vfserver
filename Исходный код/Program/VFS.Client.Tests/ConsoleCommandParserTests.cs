namespace VFS.Client.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.Client.Commands;

    /// <summary>
    /// Тестирование разбора строковой команды.
    /// </summary>
    [TestClass]
    public class ConsoleCommandParserTests
    {
        /// <summary>
        /// Тест получения названия команды из пустой строки команды.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetCommandNameFromEmptyCommand()
        {
            ConsoleCommandParser.GetCommandName("   ");
        }

        /// <summary>
        /// Тест получения параметров из пустой строки команды.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetCommandParamsFromEmptyCommand()
        {
            ConsoleCommandParser.GetCommandParams("    ");
        }

        /// <summary>
        /// Тест получения имени команд.
        /// </summary>
        [TestMethod]
        public void GetCommandNameTests()
        {
            Assert.AreEqual(
                ConsoleCommandParser.GetCommandName("    md  par md"),
                "md");
            Assert.AreEqual(
                ConsoleCommandParser.GetCommandName("md"),
                "md");
            Assert.AreEqual(
                ConsoleCommandParser.GetCommandName("русская_команда авав"),
                "русская_команда");
        }

        /// <summary>
        /// Тест получения параметров команд.
        /// </summary>
        [TestMethod]
        public void GetCommandParamsTests()
        {
            string[] commandParams =
                ConsoleCommandParser.GetCommandParams("  mjj 1    2   3,_4");

            Assert.AreEqual(commandParams[0], "1");
            Assert.AreEqual(commandParams[1], "2");
            Assert.AreEqual(commandParams[2], "3,_4");

            string[] emptyParams =
                ConsoleCommandParser.GetCommandParams(" md ");

            Assert.IsNotNull(emptyParams);
            Assert.AreEqual(emptyParams.Length, 0);
        }

        /// <summary>
        /// Проверка получения имени команды, содержащей
        /// более одной строки.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MultilineCommandNameParseTest()
        {
            ConsoleCommandParser.GetCommandName("dsfsdf \n fdfsdfs");
        }

        /// <summary>
        /// Проверка получения параметров команды, содержащей
        /// более одной строки.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MultilineCommandParamsParseTest()
        {
            ConsoleCommandParser.GetCommandParams("fsdfsdf \n fsdfsdf");
        }
    }
}