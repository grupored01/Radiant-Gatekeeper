//-----------------------------------------------------------------------
// <copyright file="IOExtensionMethods.cs" company="Tiempo Development">
//     Copyright Tiempo Development. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace WcfRadiant.Utils.ExtensionMethods.IO//TD.Utils.ExtensionMethods.IO
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Class for Extension methods
    /// </summary>
    public static class IOExtensionMethods
    {
        /// <summary>
        /// Writes the stream to a file.
        /// </summary>
        /// <param name="pStream">Stream object to write as a file.</param>
        /// <param name="pFilePath">File where the stream will be saved.</param>
        public static void WriteToFile(this Stream pStream, string pFilePath)
        {
            try
            {
                if (pStream == null)
                {
                    throw new ArgumentNullException("pStream");
                }

                if (pStream.Length == 0)
                {
                    throw new ArgumentException("The stream is empty", "pStream");
                }

                // Create a FileStream object to write a stream to a file     
                using (FileStream fileStream = File.Create(pFilePath, (int)pStream.Length))
                {
                    // Fill the bytes[] array with the stream pString         
                    byte[] bytesInStream = new byte[pStream.Length];

                    pStream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                    // Use FileStream object to write to the specified file         
                    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Writes the text to a FileInfo
        /// </summary>
        /// <param name="pFileInfo">File Info to be used</param>
        /// <param name="pText">Text to write to the FileInfo</param>
        public static void WriteAllText(this FileInfo pFileInfo, string pText)
        {
            try
            {
                using (FileStream destination = new FileStream(pFileInfo.FullName, FileMode.CreateNew))
                {
                    using (StreamWriter writer = new StreamWriter(destination, Encoding.Default))
                    {
                        writer.Write(pText);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Appends a line to a FileInfo
        /// </summary>
        /// <param name="pFileInfo">File Info to be used</param>
        /// <param name="pText">Text to write to the FileInfo</param>
        public static void AppendLine(this FileInfo pFileInfo, string pText)
        {
            try
            {
                using (FileStream destination = new FileStream(pFileInfo.FullName, FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(destination, Encoding.Default))
                    {
                        writer.WriteLine(pText);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Opens a binary file, reads the contents of the file into a byte array, and then closes the file.
        /// </summary>
        /// <param name="pFileInfo">FileInfo object to read from</param>
        /// <returns>Contents from the file in byte[]</returns>
        public static byte[] ReadAllBytes(this FileInfo pFileInfo)
        {
            try
            {
                if (pFileInfo == null)
                {
                    throw new ArgumentNullException("pFileInfo");
                }

                if (!pFileInfo.Exists)
                {
                    throw new FileNotFoundException("File not found", pFileInfo.FullName);
                }

                byte[] bytesInStream = null;

                // Creates a FileStream object to read from the file     
                using (FileStream fileStream = pFileInfo.OpenRead())
                {
                    // Set size
                    bytesInStream = new byte[fileStream.Length];

                    using (BinaryReader bynaryReade = new BinaryReader(fileStream, Encoding.Default))
                    {
                        // Fill the bytes[] array with the stream pString
                        bytesInStream = bynaryReade.ReadBytes((int)fileStream.Length);
                    }
                }

                return bytesInStream;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Reads all the text from a FileInfo
        /// </summary>
        /// <param name="pFileInfo">FileInfo to read from</param>
        /// <returns>Text content of the FileInfo</returns>
        public static string ReadAllText(this FileInfo pFileInfo)
        {
            try
            {
                if (pFileInfo == null)
                {
                    throw new ArgumentNullException("pFileInfo");
                }

                if (!pFileInfo.Exists)
                {
                    throw new FileNotFoundException("File not found", pFileInfo.FullName);
                }

                string textInStream = string.Empty;

                // Creates a FileStream object to read from the file     
                using (FileStream fileStream = pFileInfo.OpenRead())
                {
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.Default))
                    {
                        textInStream = streamReader.ReadToEnd();
                    }
                }

                return textInStream;
            }
            catch
            {
                throw;
            }
        }
    }
}