﻿namespace VFS.Tests.Common
{
    using System.Threading.Tasks;

    using VFS.Interfaces.Service;

    public class VFSSingleUserServiceTestDouble : IVFSSingleUserService
    {
        public StandardOperationResult Connect(string userName)
        {
            return new StandardOperationResult(
                "Подключено. Имя пользователя: " + userName,
                null);
        }

        public StandardOperationResult Quit()
        {
            return new StandardOperationResult(
                "Выход произведен.",
                null);
        }

        public void SubscribeForNotifications()
        {
        }

        public StandardOperationResult MakeDirectory(string path)
        {
            return new StandardOperationResult(
                "Папка " + path + " создана.",
                null);
        }

        public StandardOperationResult GetCurrentWorkingDirectoryPath()
        {
            return new StandardOperationResult(
                "C:/Test/Test",
                null);
        }

        public StandardOperationResult SetCurrentWorkingDirectory(string path)
        {
            return new StandardOperationResult(
                "Теперь текущая директория - " + path,
                null);
        }

        public StandardOperationResult RemoveDirectory(string path, bool recursive)
        {
            string message = "Директория - " + path + " удалена";

            if (recursive)
            {
                message += " вместе со всеми подпапками.";
            }

            return new StandardOperationResult(message, null);
        }

        public StandardOperationResult MakeFile(string path)
        {
            return new StandardOperationResult(
                "Создан новый файл - " + path,
                null);
        }

        public StandardOperationResult DeleteFile(string path)
        {
            return new StandardOperationResult(
                "Файл " + path + " удален",
                null);
        }

        public StandardOperationResult LockFile(string path)
        {
            return new StandardOperationResult(
                "Файл " + path + " теперь нельзя удалить",
                null);
        }

        public StandardOperationResult UnlockFile(string path)
        {
            return new StandardOperationResult(
                "Файл " + path + " теперь можно удалить",
                null);
        }

        public StandardOperationResult Copy(string sourcePath, string destinationPath)
        {
            return new StandardOperationResult(
                "Скопировано из " + sourcePath + " в " + destinationPath,
                null);
        }

        public StandardOperationResult Move(string sourcePath, string destinationPath)
        {
            return new StandardOperationResult(
                "Перемещено из " + sourcePath + " в " + destinationPath,
                null);
        }

        public StandardOperationResult GetDriveStructure(string drive)
        {
            var message = @"<?xml version=""1.0"" encoding=""utf-8"" ?> 
<DriveStructure driveName=""C:"">
  <Directories>
    <Directory name=""Dir1"">
      <Directories>
        <Directory name=""Dir3"" />
      </Directories>
      <Files />
    </Directory>
    <Directory name=""Dir2"" />
  </Directories>
  <Files>
    <File name=""File1"" />
  </Files>
</DriveStructure>";

            return new StandardOperationResult(
                message,
                null);
        }

        public async Task<StandardOperationResult> UploadFileAsync(
            string filePath,
            string fileData)
        {
            return await Task.FromResult(new StandardOperationResult(
                "Данные успешно загружены в файл " + filePath,
                null));
        }

        public async Task<StandardOperationResult> DownloadFileAsync(string filePath)
        {
            return await Task.FromResult(new StandardOperationResult(
                "Данные успешно загружены из файла файл " + filePath,
                null));
        }
    }
}