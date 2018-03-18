using System;
using System.IO;
using System.Net;
using RestFiles.ServiceInterface.Support;
using RestFiles.ServiceModel;
using RestFiles.ServiceModel.Types;
using ServiceStack;
using File = System.IO.File;

namespace RestFiles.ServiceInterface
{
    /// <summary>
    /// Define your ServiceStack web service request (i.e. Request DTO).
    /// </summary>
    public class FilesService : Service
    {
        /// <summary>
        /// Gets or sets the AppConfig. The built-in IoC used with ServiceStack autowires this property.
        /// </summary>
        public AppConfig Config { get; set; }

        public object Get(Files request)
        {
            var targetFile = GetAndValidateExistingPath(request);

            var isDirectory = Directory.Exists(targetFile.FullName);

            if (!isDirectory && request.ForDownload)
                return new HttpResult(targetFile, asAttachment: true);

            var response = isDirectory
                ? new FilesResponse { Directory = GetFolderResult(targetFile.FullName) }
                : new FilesResponse { File = GetFileResult(targetFile) };

            return response;
        }

        public void Post(Files request)
        {
            var targetDir = GetPath(request);

            var isExistingFile = targetDir.Exists
                && (targetDir.Attributes & FileAttributes.Directory) != FileAttributes.Directory;

            if (isExistingFile)
                throw new NotSupportedException(
                "POST only supports uploading new files. Use PUT to replace contents of an existing file");

            if (!Directory.Exists(targetDir.FullName))
                Directory.CreateDirectory(targetDir.FullName);

            foreach (var uploadedFile in base.Request.Files)
            {
                var newFilePath = Path.Combine(targetDir.FullName, uploadedFile.FileName);
                uploadedFile.SaveTo(newFilePath);
            }
        }

        public void Put(Files request)
        {
            var targetFile = GetAndValidateExistingPath(request);

            if (!this.Config.TextFileExtensions.Contains(targetFile.Extension))
                throw new NotSupportedException("PUT Can only update text files, not: " + targetFile.Extension);

            if (request.TextContents == null)
                throw new ArgumentNullException("TextContents");

            File.WriteAllText(targetFile.FullName, request.TextContents);
        }

        public void Delete(Files request)
        {
            var targetFile = GetAndValidateExistingPath(request);
            File.Delete(targetFile.FullName);
        }

        private FolderResult GetFolderResult(string targetPath)
        {
            var result = new FolderResult();

            foreach (var dirPath in Directory.GetDirectories(targetPath))
            {
                var dirInfo = new DirectoryInfo(dirPath);

                if (this.Config.ExcludeDirectories.Contains(dirInfo.Name)) continue;

                result.Folders.Add(new Folder
                {
                    Name = dirInfo.Name,
                    ModifiedDate = dirInfo.LastWriteTimeUtc,
                    FileCount = dirInfo.GetFiles().Length
                });
            }

            foreach (var filePath in Directory.GetFiles(targetPath))
            {
                var fileInfo = new FileInfo(filePath);

                result.Files.Add(new ServiceModel.Types.File
                {
                    Name = fileInfo.Name,
                    Extension = fileInfo.Extension,
                    FileSizeBytes = fileInfo.Length,
                    ModifiedDate = fileInfo.LastWriteTimeUtc,
                    IsTextFile = Config.TextFileExtensions.Contains(fileInfo.Extension),
                });
            }

            return result;
        }

        private FileInfo GetPath(Files request)
        {
            return new FileInfo(Path.Combine(this.Config.RootDirectory, request.Path.GetSafePath()));
        }

        private FileInfo GetAndValidateExistingPath(Files request)
        {
            var targetFile = GetPath(request);
            if (!targetFile.Exists && !Directory.Exists(targetFile.FullName))
                throw new HttpError(HttpStatusCode.NotFound, new FileNotFoundException("Could not find: " + request.Path));

            return targetFile;
        }

        private FileResult GetFileResult(FileInfo fileInfo)
        {
            var isTextFile = this.Config.TextFileExtensions.Contains(fileInfo.Extension);

            return new FileResult
            {
                Name = fileInfo.Name,
                Extension = fileInfo.Extension,
                FileSizeBytes = fileInfo.Length,
                IsTextFile = isTextFile,
                Contents = isTextFile ? File.ReadAllText(fileInfo.FullName) : null,
                ModifiedDate = fileInfo.LastWriteTimeUtc,
            };
        }
    }
}