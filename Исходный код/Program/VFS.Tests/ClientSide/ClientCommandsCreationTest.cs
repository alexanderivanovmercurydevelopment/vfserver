namespace VFS.Tests.ClientSide
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.Client.Commands;
    using VFS.Tests.TestDoubles;

    [TestClass]
    public class ClientCommandsCreationTest
    {
        /// <summary>
        /// Создание несуществующей команды.
        /// </summary>
        [TestMethod]
        public void NonexistentCommandCreationTest()
        {
            VFSClientCommand command = CommandCreator.CreateCommand(
                "fakecommand", 
                new VFSSingleUserServiceTestDouble());

            Assert.IsNull(command);
        }

        /// <summary>
        /// Создание пустой команды.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(ArgumentNullException))]
        public void EmptyCommandCreationTest()
        {
            CommandCreator.CreateCommand(
                "   ",
                new VFSSingleUserServiceTestDouble());
        }

        /// <summary>
        /// Создание команды без передачи интерфейса доступа
        /// к виртуальному файловому серверу.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(ArgumentNullException))]
        public void CreateCommandWithoutVFSService()
        {
            CommandCreator.CreateCommand("md", null);
        }

        /// <summary>
        /// Создания всех возможных команд.
        /// </summary>
        [TestMethod]
        public void StandardCommandsCreationTest()
        {
            foreach (var value in Enum.GetValues(typeof(ClientCommands)))
            {
                VFSClientCommand command = 
                    CommandCreator.CreateCommand(
                         Enum.GetName(typeof(ClientCommands), value), 
                         new VFSSingleUserServiceTestDouble());

                Assert.IsNotNull(command);
            }
        }
    }
}
