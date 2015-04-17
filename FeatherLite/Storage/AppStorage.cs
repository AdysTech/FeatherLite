using AdysTech.FeatherLite.Loggers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace AdysTech.FeatherLite.Storage
{
    public static class AppStorage
    {

        #region WP8 specific code
        /***********************************************************************************************/



        public static async Task<StorageFolder> TraverseDirectoryStructureAsync(string fileName)
        {

            try
            {
                var dirsPath = GetDirectoryName (fileName).Split (Path.DirectorySeparatorChar);
                StorageFolder folder = ApplicationData.Current.LocalFolder;


                if ( dirsPath.Length == 1 )
                    return folder;

                //Recreate the directory structure
                foreach ( var part in dirsPath )
                {
                    if ( part == "." ) continue;
                    folder = await folder.CreateFolderAsync (part, CreationCollisionOption.OpenIfExists);
                }
                return folder;
            }
            catch ( Exception e )
            {
                throw new OperationCanceledException ("Failed to create Structure!" + fileName, e);
            }
            finally
            {

            }
        }

        public static string GetFileName(string fileName)
        {
            return Path.GetFileName (fileName);
        }

        public static string GetDirectoryName(string fileName)
        {
            return Path.GetDirectoryName (fileName);
        }


        public static async Task<bool> SaveToStorageAsync(string fileName, byte[] data)
        {
            try
            {
                var folder = await TraverseDirectoryStructureAsync (fileName);

                StorageFile textFile = await folder.CreateFileAsync (GetFileName (fileName), CreationCollisionOption.ReplaceExisting);

                using ( var textStream = await textFile.OpenStreamForWriteAsync () )
                {
                    using ( var binaryWriter = new BinaryWriter (textStream) )
                    {
                        binaryWriter.Write (data);
                        binaryWriter.Close ();
                        return true;
                    }
                }

            }
            catch
            {
                return false;
            }
            finally
            {

            }
        }

        public static async Task<bool> WriteToFileAsync(string fileName, string data)
        {
            try
            {
                StorageFile textFile = null;
                var folder = await TraverseDirectoryStructureAsync (fileName);
                try
                {
                    textFile = await folder.CreateFileAsync (GetFileName (fileName), CreationCollisionOption.ReplaceExisting);
                }
                catch ( System.Reflection.TargetInvocationException ex )
                {
                    return false;
                }
                using ( IRandomAccessStream textStream = await textFile.OpenAsync (FileAccessMode.ReadWrite) )
                {
                    using ( DataWriter textWriter = new DataWriter (textStream) )
                    {
                        textWriter.WriteString (data);
                        var res = await textWriter.StoreAsync ();
                        textWriter.DetachStream ();
                    }
                    return await textStream.FlushAsync ();
                }

            }
            catch ( System.Reflection.TargetInvocationException ex )
            {
                return false;
            }
            catch ( Exception e )
            {
                return false;
            }

        }


        public static async Task<bool> AppendToFileAsync(string fileName, string data)
        {
            try
            {
                var folder = await TraverseDirectoryStructureAsync (fileName);

                StorageFile textFile = await folder.CreateFileAsync (GetFileName (fileName), CreationCollisionOption.OpenIfExists);

                using ( var fileTransaction = await textFile.OpenTransactedWriteAsync () )
                {
                    using ( var outputStream = fileTransaction.Stream.GetOutputStreamAt (fileTransaction.Stream.Size) )
                    {
                        using ( DataWriter textWriter = new DataWriter (outputStream) )
                        {
                            textWriter.WriteString (data);
                            await textWriter.StoreAsync ();
                            await fileTransaction.CommitAsync ();
                            return true;
                        }
                    }
                }
            }
            catch ( Exception e )
            {
                return false;
            }
            finally
            {

            }
        }


        public static async Task<ulong> GetFileSizeAsync(string fileName)
        {
            try
            {

                StorageFolder localFolder = await TraverseDirectoryStructureAsync (fileName);
                StorageFile file = await localFolder.GetFileAsync (GetFileName (fileName));
                BasicProperties basicProperties = await file.GetBasicPropertiesAsync ();
                return basicProperties.Size;
            }
            catch
            {
                return 0;
            }
            finally
            {

            }
        }


        public static async Task<Stream> StreamFileFromStorageAsync(string fileName)
        {
            try
            {
                {

                    StorageFolder localFolder = await TraverseDirectoryStructureAsync (fileName);
                    StorageFile textFile = await localFolder.GetFileAsync (GetFileName (fileName));
                    using ( var fileStream = await textFile.OpenStreamForReadAsync () )
                    {
                        var memoryStream = new MemoryStream ();
                        fileStream.CopyTo (memoryStream);
                        fileStream.Close ();
                        memoryStream.Position = 0;
                        return memoryStream;
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {

            }
        }

        public static async Task<string> FileFromStorageAsync(string fileName)
        {
            try
            {

                StorageFolder localFolder = await TraverseDirectoryStructureAsync (fileName);
                StorageFile textFile = await localFolder.GetFileAsync (GetFileName (fileName));

                using ( IRandomAccessStream textStream = await textFile.OpenReadAsync () )
                {
                    using ( DataReader textReader = new DataReader (textStream) )
                    {
                        uint textLength = (uint) textStream.Size;
                        await textReader.LoadAsync (textLength);
                        var contents = textReader.ReadString (textLength);
                        return contents;
                    }
                }
            }

            catch ( Exception e )
            {
                return "";
            }
            finally
            {

            }
        }

        public static async Task<bool> CheckFileExistsAsync(string fileName)
        {
            try
            {
                StorageFolder localFolder = await TraverseDirectoryStructureAsync (fileName);
                StorageFile localFile = await localFolder.GetFileAsync (GetFileName (fileName));
                return true;
            }
            catch ( FileNotFoundException )
            {
                return false;
            }

        }


        public static string GetLogicalPath(string fileName)
        {
            try
            {

                var dirName = GetDirectoryName (fileName);
                if ( dirName.StartsWith ("." + Path.DirectorySeparatorChar) )
                    dirName = dirName.Substring (2);

                StorageFolder folder = ApplicationData.Current.LocalFolder;
                return Path.Combine (folder.Path, dirName, GetFileName (fileName));
            }
            catch ( FileNotFoundException )
            {
                return "";
            }

        }

        public static async Task<string> GetPhysicalPathAsync(string fileName)
        {
            try
            {
                StorageFolder localFolder = await TraverseDirectoryStructureAsync (fileName);
                StorageFile localFile = await localFolder.GetFileAsync (GetFileName (fileName));
                return localFile.Path;
            }
            catch ( FileNotFoundException )
            {
                return "";
            }

        }

        public static async Task<bool> CreateStorageFileAsync(string fileName)
        {

            try
            {
                var dirName = await TraverseDirectoryStructureAsync (fileName);
                var stream = dirName.CreateFileAsync (GetFileName (fileName), CreationCollisionOption.OpenIfExists);
                return true;

            }
            catch
            {
                return false;
            }
            finally
            {

            }
        }

        public static async Task<bool> CopyFromInstallFolderToLocalAsync(string RelativePath)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                // Create a stream for the file in the installation folder.
                using ( Stream input = System.Windows.Application.GetResourceStream (new Uri (RelativePath, UriKind.Relative)).Stream )
                {
                    var targetFile = await localFolder.CreateFileAsync (GetFileName (RelativePath), CreationCollisionOption.ReplaceExisting);
                    // Create a stream for the new file in the local folder.
                    using ( var output = await targetFile.OpenStreamForWriteAsync () )
                    {
                        // Initialize the buffer.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;

                        // Copy the file from the installation folder to the local folder. 
                        while ( ( bytesRead = input.Read (readBuffer, 0, readBuffer.Length) ) > 0 )
                        {
                            await output.WriteAsync (readBuffer, 0, bytesRead);
                        }
                        return true;
                    }
                }
            }
            catch ( Exception e )
            {
                return false;
            }
        }

        #endregion



        internal static async Task<bool> DeleteFile(string FileName)
        {
            StorageFolder localFolder = await TraverseDirectoryStructureAsync (FileName);
            StorageFile textFile = await localFolder.GetFileAsync (GetFileName (FileName));
            await textFile.DeleteAsync ();
            return true;
        }
    }
}
