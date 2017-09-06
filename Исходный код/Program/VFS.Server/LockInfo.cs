namespace VFS.Server
{
    using System;

    using VFS.Interfaces.VirtualDrive;

    internal class FileLockInfo
    {
        internal FileLockInfo(VFSUser blockedBy, IVirtualFile file)
        {
            if (blockedBy == null)
            {
                throw new ArgumentNullException(
                    nameof(blockedBy),
                    "Необходимо указать пользователя, который блокирует файл.");
            }

            if (file == null)
            {
                throw new ArgumentNullException(
                    nameof(file),
                    "Необходимо указать блокируемый файл.");
            }

            this.BlockedBy = blockedBy;
            this.File = file;
        }

        internal VFSUser BlockedBy { get; }

        internal IVirtualFile File { get; }
    }
}