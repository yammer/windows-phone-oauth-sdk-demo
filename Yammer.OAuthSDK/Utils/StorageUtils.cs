using System.IO;
using System.IO.IsolatedStorage;
using System.Text;

namespace Yammer.OAuthSDK.Utils
{
    /// <summary>
    /// Utils class to handle storage related operations.
    /// </summary>
    public static class StorageUtils
    {
        /// <summary>
        /// Deletes file from Isolated Storage if exists.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        public static void DeleteFromIsolatedStorage(string path)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if (file.FileExists(path))
            {
                file.DeleteFile(path);
            }
        }

        /// <summary>
        /// Writes text to an isolated storage file.
        /// </summary>
        /// <param name="data">The text to write to the file.</param>
        /// <param name="path">Path to the file.</param>
        public static void WriteToIsolatedStorage(string data, string path)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            WriteToIsolatedStorage(byteData, path);
        }

        /// <summary>
        /// Writes byte data to an isolated storage file.
        /// </summary>
        /// <param name="data">The bytes to write to the file.</param>
        /// <param name="path">Path to the file.</param>
        public static void WriteToIsolatedStorage(byte[] data, string path)
        {
            // Create a file in the application's isolated storage.
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream writestream = new IsolatedStorageFileStream(path, FileMode.Create, FileAccess.Write, file))
                {
                    writestream.Write(data, 0, data.Length);
                }
            }
        }

        /// <summary>
        /// Reads all text content from an isolated storage file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>The text read from the file.</returns>
        public static string ReadStringFromIsolatedStorage(string path)
        {
            byte[] byteData = ReadBytesFromIsolatedStorage(path);
            return Encoding.UTF8.GetString(byteData, 0, byteData.Length);
        }

        /// <summary>
        /// Reads all byte content from an isolated storage file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>The bytes read from the file.</returns>
        public static byte[] ReadBytesFromIsolatedStorage(string path)
        {
            // Access the file in the application's isolated storage.
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream readstream = new IsolatedStorageFileStream(path, FileMode.Open, FileAccess.Read, file))
                {
                    byte[] valueArray = new byte[readstream.Length];
                    readstream.Read(valueArray, 0, valueArray.Length);
                    return valueArray;
                }
            }
        }
    }
}
