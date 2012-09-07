﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;

namespace WinRTXamlToolkit.IO.Extensions
{
    public static class StorageFolderExtensions
    {
        #region ContainsFileAsync()
        /// <summary>
        /// Returns true if a file exists at a given path relative to the given folder.
        /// </summary>
        /// <param name="folder">Starting folder.</param>
        /// <param name="relativePath">Relative path.</param>
        /// <returns>True if file exists. False - otherwise.</returns>
        /// <remarks>
        /// NOTE: Even if the files with the given name only match the resource key - it will return true.
        /// I.e. if relative path points to Logo.png, but there is only Logo.scale-100.png - the method will still return true.
        /// </remarks>
        public static async Task<bool> ContainsFileAsync(this StorageFolder folder, string relativePath)
        {
            if (folder == Package.Current.InstalledLocation)
            {
                string resourceKey = string.Format("Files/{0}", relativePath);
                var mainResourceMap = ResourceManager.Current.MainResourceMap;

                return (mainResourceMap.ContainsKey(resourceKey));
            }

            var parts = relativePath.Split('\\', '/');

            for (int i = 0; i < parts.Length - 1; i++)
            {
                folder = await folder.GetFolderAsync(parts[i]);
            }

            string fileName = parts[parts.Length - 1];

            return (await folder.GetFilesAsync()).Any(file => file.Name == fileName);
            //try
            //{
            //    await folder.GetFileAsync(fileName);
            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
        } 
        #endregion

        #region GetFileByPathAsync()
        /// <summary>
        /// Returns the StorageFile object for a file at a given path relative to the given folder.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static async Task<StorageFile> GetFileByPathAsync(this StorageFolder folder, string relativePath)
        {
            if (folder == Package.Current.InstalledLocation)
            {
                string resourceKey = string.Format("Files/{0}", relativePath);
                var mainResourceMap = ResourceManager.Current.MainResourceMap;

                if (mainResourceMap.ContainsKey(resourceKey))
                {
                    return await mainResourceMap[resourceKey].Resolve().GetValueAsFileAsync();
                }
            }
            
            var parts = relativePath.Split('\\', '/');

            for (int i = 0; i < parts.Length - 1; i++ )
            {
                folder = await folder.GetFolderAsync(parts[i]);
            }

            string fileName = parts[parts.Length - 1];

            return await folder.GetFileAsync(fileName);
        } 
        #endregion

        #region CreateTempFileNameAsync()
        /// <summary>
        /// Creates a unique file name given the parameters.
        /// Note the theoretical race condition since
        /// the file name is only guaranteed to be unique at the point it is generated.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="extension"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static async Task<string> CreateTempFileNameAsync(
            this StorageFolder folder,
            string extension = ".tmp",
            string prefix = "",
            string suffix = "")
        {
            string fileName;

            if (folder == null)
            {
                folder = ApplicationData.Current.TemporaryFolder;
            }

            if (string.IsNullOrEmpty(extension))
            {
                extension = ".tmp";
            }
            else if (extension[0] != '.')
            {
                extension = string.Format(".{0}", extension);
            }

            // Try no random numbers
            if (!string.IsNullOrEmpty(prefix) &&
                !string.IsNullOrEmpty(prefix))
            {
                fileName = string.Format(
                    "{0}{1}{2}",
                    prefix,
                    suffix,
                    extension);
                if (!await folder.ContainsFileAsync(fileName))
                {
                    return fileName;
                }
            }

            do
            {
                fileName = string.Format(
                    "{0}{1}{2}{3}",
                    prefix,
                    Guid.NewGuid(),
                    suffix,
                    extension);
            } while (await folder.ContainsFileAsync(fileName));

            return fileName;
        } 
        #endregion

        #region CreateTempFileAsync()
        /// <summary>
        /// Creates a temporary file.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static async Task<StorageFile> CreateTempFileAsync(this StorageFolder folder, string extension = ".tmp")
        {
            if (folder == null)
            {
                folder = ApplicationData.Current.TemporaryFolder;
            }

            var fileName = await folder.CreateTempFileNameAsync(
                extension,
                DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss.ffffff"));

            var file = await folder.CreateFileAsync(
                fileName,
                CreationCollisionOption.GenerateUniqueName);

            return file;
        } 
        #endregion

        #region DeleteFilesAsync()
        /// <summary>
        /// Deletes all files from the given folder.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="ignoreExceptions">If true - all exceptions will be swallowed. Otherwise - they will be rethrown.</param>
        /// <returns></returns>
        public static async Task DeleteFilesAsync(this StorageFolder folder, bool ignoreExceptions = false)
        {
            try
            {
                foreach (var file in await folder.GetFilesAsync())
                {
                    try
                    {
                        await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    catch
                    {
                        if (!ignoreExceptions)
                            throw;
                    }
                }
            }
            catch
            {
                if (!ignoreExceptions)
                    throw;
            }
        }
        #endregion
    }
}
