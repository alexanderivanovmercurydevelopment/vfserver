namespace VFS.Server
{
    using System;

    using VFS.Utilities;

    internal class VFSUser
    {
        private string currentWorkingDirectoryPath;

        internal VFSUser(string name, string currentWorkingDirectoryPath)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(
                    nameof(name),
                    "Имя пользователя не должно быть пустым.");
            }

            currentWorkingDirectoryPath.ValidateCorrectPath();

            this.Name = name;
            this.currentWorkingDirectoryPath = currentWorkingDirectoryPath;
        }

        internal string Name { get; }

        internal string CurrentWorkingDirectoryPath
        {
            get { return this.currentWorkingDirectoryPath; }
            set
            {
                value.ValidateCorrectPath();
                this.currentWorkingDirectoryPath = value;
            }
        }
    }
}