using System;
using System.IO;

namespace MvxExtensions.Extensions
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
                var totalBytesRead = 0;
                int bytesRead;
                var readBuffer = new byte[4096];

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        var nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            var temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                var buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }

                return buffer;
            }

            return null;
        }
    }
}
