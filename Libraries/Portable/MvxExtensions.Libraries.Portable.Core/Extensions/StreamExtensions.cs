using System;
using System.IO;

namespace MvxExtensions.Libraries.Portable.Core.Extensions
{
    /// <summary>
    /// Extensions for Stream type
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads all the bytes from a stream.
        /// Deals with null streams
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static byte[] SafeReadToEnd(this Stream stream)
        {
            if (stream != null)
            {
                long originalPosition = stream.Position;
                int totalBytesRead = 0;
                int bytesRead;
                byte[] readBuffer = new byte[4096];

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }

                return buffer;
            }
            else
            {
                return null;
            }
        }
    }
}
