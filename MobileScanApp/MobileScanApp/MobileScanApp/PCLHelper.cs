using PCLStorage;  
using System;  
using System.Threading.Tasks;


    namespace MobileScanApp
    {

    /*
     *  @author Jess Merolla
     *  @date 9/25/2020
     *  @source:  https://www.c-sharpcorner.com/article/local-file-storage-using-xamarin-form/
     *  @summary:
     *  
     *  Uses the PCLStorage package to generate a set of helpers to work
     *  with cross platform file systems
     *  
     *  !!!!!!!!!!!TO-DO figure out if any of this works without crashing
     *  
     */
    public static class PCLHelper
        {

            public async static Task<bool> IsFileExistAsync(this string fileName, IFolder rootFolder = null)
            {
                // get hold of the file system  
                IFolder folder = rootFolder ?? FileSystem.Current.LocalStorage;
                ExistenceCheckResult folderexist = await folder.CheckExistsAsync(fileName);
                // already run at least once, don't overwrite what's there  
                if (folderexist == ExistenceCheckResult.FileExists)
                {
                    return true;

                }
                return false;
            }

            public async static Task<bool> IsFolderExistAsync(this string folderName, IFolder rootFolder = null)
            {
                // get hold of the file system  
                IFolder folder = rootFolder ?? FileSystem.Current.LocalStorage;
                ExistenceCheckResult folderexist = await folder.CheckExistsAsync(folderName);
                // already run at least once, don't overwrite what's there  
                if (folderexist == ExistenceCheckResult.FolderExists)
                {
                    return true;

                }
                return false;
            }

            public async static Task<IFolder> CreateFolder(this string folderName, IFolder rootFolder = null)
            {
                IFolder folder = rootFolder ?? FileSystem.Current.LocalStorage;
                folder = await folder.CreateFolderAsync(folderName, CreationCollisionOption.FailIfExists);
                return folder;
            }

            public async static Task<IFile> CreateFile(this string filename, IFolder rootFolder = null)
            {
                IFolder folder = rootFolder ?? FileSystem.Current.LocalStorage;
                IFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                return file;
            }
            public async static Task<bool> WriteTextAllAsync(this string filename, string content = "", IFolder rootFolder = null)
            {
                IFile file = await filename.CreateFile(rootFolder);
                await file.WriteAllTextAsync(content);
                return true;
            }

            public async static Task<string> ReadAllTextAsync(this string fileName, IFolder rootFolder = null)
            {
                string content = "";
                IFolder folder = rootFolder ?? FileSystem.Current.LocalStorage;
                bool exist = await fileName.IsFileExistAsync(folder);
                if (exist == true)
                {
                    IFile file = await folder.GetFileAsync(fileName);
                    content = await file.ReadAllTextAsync();
                }
                return content;
            }
        }
    }
