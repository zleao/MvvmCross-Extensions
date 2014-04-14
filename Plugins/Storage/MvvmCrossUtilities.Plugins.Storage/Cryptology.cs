/// Credits to http://lukhezo.com/2011/11/06/encrypting-files-in-net-using-the-advanced-encryption-standard-aes/
/// Code taken from above link, and adapted for my own implementation
/// @zleao

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Cirrious.CrossCore.Platform;

namespace MvvmCrossUtilities.Plugins.Storage
{
    internal sealed class Cryptology
    {
        #region Constants

        private const int ENCRYPTION_HEADER_STRING_LENGTH = 32;
        private const int ENCRYPTION_HEADER_BYTEARRAY_LENGTH = 48;
        private const char ENCRYPTION_HEADER_FILL_CHAR = ' ';
        private const string SALT = "d5fg4df5sg4ds5fg45sdfg4";
        private const int SIZE_OF_BUFFER = 1024 * 8;

        #endregion

        #region Public Methods

        internal static byte[] EncryptStringToBytes(string plainText, string password)
        {
            byte[] encrypted;

            // Create an RijndaelManaged object
            // with the specified password.
            InitializeAlgorithm(password);
            encrypted = EncryptStringToBytes(plainText, password, Algorithm);

            return encrypted;
        }
        internal static byte[] EncryptStringToBytes(string plainText, string password, RijndaelManaged algorithm)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            byte[] encrypted;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform encryptor = algorithm.CreateEncryptor();

            // Create the streams used for encryption.
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        internal static void EncryptFile(string decriptedFilePath, string encryptedFilePath, string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            try
            {
                using (var decryptedStream = new FileStream(decriptedFilePath, FileMode.Open, FileAccess.Read))
                    EncryptStreamToFile(decryptedStream, encryptedFilePath, password, BaseStorageManager.EncryptionMode.CRYPTOFULL);
            }
            catch (CryptographicException)
            {
                throw new InvalidDataException("Please supply a correct password");
            }
        }

        internal static void EncryptStreamToFile(Stream decryptedSourceStream, string encryptedFilePath, string password, BaseStorageManager.EncryptionMode encryptionMode)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            InitializeAlgorithm(password);
            switch (encryptionMode)
            {
                case BaseStorageManager.EncryptionMode.CRYPTOFULL:
                    var outputFull = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.Write);

                    WriteEncryptionHeader(outputFull, password, encryptionMode, Algorithm);

                    using (var encryptedStream = new CryptoStream(outputFull, Algorithm.CreateEncryptor(), CryptoStreamMode.Write))
                        CopyStream(decryptedSourceStream, encryptedStream);
                    break;

                case BaseStorageManager.EncryptionMode.CRYPTOLINE:
                    using (var outputLine = new FileStream(encryptedFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        if (outputLine.Length > 0)
                        {
                            var existingFileEncryptionMode = GetEncryptionMode(outputLine, password, Algorithm);
                            if (existingFileEncryptionMode == BaseStorageManager.EncryptionMode.UNKNOWN)
                            {
                                //Copy existing file to a temporary stream
                                outputLine.Seek(0, SeekOrigin.Begin);
                                var temporaryStream = new MemoryStream();
                                outputLine.CopyTo(temporaryStream);

                                //Delete all contents from existing file
                                outputLine.SetLength(0);
                                outputLine.Seek(0, SeekOrigin.Begin);

                                //Write encryption header
                                WriteEncryptionHeader(outputLine, password, encryptionMode, Algorithm);

                                //encrypt temporary stream to file
                                temporaryStream.Seek(0, SeekOrigin.Begin);
                                EncryptStream(temporaryStream, password, outputLine);
                            }
                            else if (existingFileEncryptionMode != BaseStorageManager.EncryptionMode.CRYPTOLINE)
                            {
                                throw new NotSupportedException("File already encrypted with another mode of encryption. " + encryptedFilePath);
                            }
                        }

                        if (outputLine.Length == 0)
                            WriteEncryptionHeader(outputLine, password, encryptionMode, Algorithm);
                        else
                            outputLine.Seek(0, SeekOrigin.End);

                        EncryptStream(decryptedSourceStream, password, outputLine);
                    }
                    break;

                case BaseStorageManager.EncryptionMode.UNKNOWN:
                default:
                    MvxTrace.Warning("EncryptStreamToFile: encryption mode unknown");
                    break;
            }
        }

        private static void EncryptStream(Stream decryptedSourceStream, string password, FileStream outputLine)
        {
            using (var sr = new StreamReader(decryptedSourceStream))
            {
                var encryptedBytes = EncryptStringToBytes(sr.ReadToEnd(), password, Algorithm);
                var encryptedBytesLengthInBytes = ConvertIntToBytes(encryptedBytes.Length);

                outputLine.Write(encryptedBytesLengthInBytes, 0, encryptedBytesLengthInBytes.Length);
                outputLine.Write(encryptedBytes, 0, encryptedBytes.Length);
            }
        }


        internal static string DecryptStringFromBytes(byte[] cipherText, string password)
        {
            string plaintext;

            // Create an RijndaelManaged object
            // with the specified password.
            InitializeAlgorithm(password);
            plaintext = DecryptStringFromBytes(cipherText, password, Algorithm);

            return plaintext;
        }
        internal static string DecryptStringFromBytes(byte[] cipherText, string password, RijndaelManaged algorithm)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = algorithm.CreateDecryptor();

            // Create the streams used for decryption.
            using (var msDecrypt = new MemoryStream(cipherText))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

            return plaintext;
        }

        internal static void DecryptFile(string encryptedFilePath, string decryptedFilePath, string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            try
            {
                using (var output = new FileStream(decryptedFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                using (var decryptedStream = DecryptFileToStream(encryptedFilePath, password))
                {
                    decryptedStream.Seek(0, SeekOrigin.Begin);
                    CopyStream(decryptedStream, output);
                }
            }
            catch (CryptographicException)
            {
                throw new InvalidDataException("Please supply a correct password");
            }
        }

        internal static Stream DecryptFileToStream(string encryptedFilePath, string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            var decryptedMemoryStream = new MemoryStream();

            InitializeAlgorithm(password);
            using (var input = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
            {
                if (input.Length > 0)
                {
                    switch (GetEncryptionMode(input, password, Algorithm))
                    {
                        case BaseStorageManager.EncryptionMode.CRYPTOFULL:
                            var decryptedStream = new CryptoStream(decryptedMemoryStream, Algorithm.CreateDecryptor(), CryptoStreamMode.Write);
                            CopyStream(input, decryptedStream);
                            decryptedStream.FlushFinalBlock();
                            break;

                        case BaseStorageManager.EncryptionMode.CRYPTOLINE:
                            var decryptedString = DecryptLineStreamRecursively(input, password, Algorithm);
                            var sw = new StreamWriter(decryptedMemoryStream);
                            sw.Write(decryptedString);
                            sw.Flush();
                            break;

                        case BaseStorageManager.EncryptionMode.UNKNOWN:
                            MvxTrace.Warning("DecryptFileToStream: encryption mode unknown. Returning stream as is...");
                            input.Seek(0, SeekOrigin.Begin);
                            CopyStream(input, decryptedMemoryStream);
                            break;
                    }
                }
            }

            if (decryptedMemoryStream != null && decryptedMemoryStream.Length > 0 && decryptedMemoryStream.CanSeek)
                decryptedMemoryStream.Seek(0, SeekOrigin.Begin);

            return decryptedMemoryStream;
        }

        internal static void DisposeAlgorithmAndPassword()
        {
            Password = null;
            if (Algorithm != null)
                Algorithm.Dispose();
        }

        #endregion

        #region Private Methods

        private static RijndaelManaged Algorithm;
        private static string Password;
        private static void InitializeAlgorithm(string password)
        {
            if (Password != password || Algorithm == null)
            {
                DisposeAlgorithmAndPassword();

                // Essentially, if you want to use RijndaelManaged as AES you need to make sure that:
                // 1.The block size is set to 128 bits
                // 2.You are not using CFB mode, or if you are the feedback size is also 128 bits

                Password = password;

                Algorithm = new RijndaelManaged { KeySize = 256, BlockSize = 128 };

                var key = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(SALT));

                Algorithm.Key = key.GetBytes(Algorithm.KeySize / 8);

                Algorithm.IV = key.GetBytes(Algorithm.BlockSize / 8);
            }
        }

        private static void CopyStream(Stream input, Stream output)
        {
            if (input.Length > 0)
            {
                byte[] buffer = new byte[SIZE_OF_BUFFER];
                int read;

                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, read);
                }
            }
        }

        private static BaseStorageManager.EncryptionMode GetEncryptionMode(Stream streamToCheck, string password, RijndaelManaged algorithm)
        {
            try
            {
                if (streamToCheck == null || streamToCheck.Length < ENCRYPTION_HEADER_BYTEARRAY_LENGTH)
                    return BaseStorageManager.EncryptionMode.UNKNOWN;

                var initialStreamPosition = streamToCheck.Position;

                var headerBytes = new Byte[ENCRYPTION_HEADER_BYTEARRAY_LENGTH];
                var bytesRead = streamToCheck.Read(headerBytes, 0, ENCRYPTION_HEADER_BYTEARRAY_LENGTH);
                if (bytesRead != ENCRYPTION_HEADER_BYTEARRAY_LENGTH)
                {
                    streamToCheck.Seek(initialStreamPosition, SeekOrigin.Begin);
                    return BaseStorageManager.EncryptionMode.UNKNOWN;
                }

                var headerString = DecryptStringFromBytes(headerBytes, password, algorithm);
                //var headerString = GetString(headerBytes);
                var trimmedHeaderString = (headerString != null ? headerString.Trim(ENCRYPTION_HEADER_FILL_CHAR) : headerString);

                BaseStorageManager.EncryptionMode result = BaseStorageManager.EncryptionMode.UNKNOWN;

                if (string.IsNullOrEmpty(trimmedHeaderString) || !Enum.TryParse(trimmedHeaderString, out result))
                {
                    streamToCheck.Seek(initialStreamPosition, SeekOrigin.Begin);
                    result = BaseStorageManager.EncryptionMode.UNKNOWN;
                }

                streamToCheck.Seek(initialStreamPosition + ENCRYPTION_HEADER_BYTEARRAY_LENGTH, SeekOrigin.Begin);

                return result;
            }
            catch (Exception ex)
            {
                MvxTrace.Error(ex.Message);
                return BaseStorageManager.EncryptionMode.UNKNOWN;
            }
        }

        private static void WriteEncryptionHeader(Stream targetStream, string password, BaseStorageManager.EncryptionMode mode, RijndaelManaged algorithm)
        {
            switch (mode)
            {
                case BaseStorageManager.EncryptionMode.UNKNOWN:
                    return;
            }

            var headerString = mode.ToString();
            headerString = headerString.PadRight(ENCRYPTION_HEADER_STRING_LENGTH, ENCRYPTION_HEADER_FILL_CHAR);

            var headerBytesEncrypted = EncryptStringToBytes(headerString, password, algorithm);

            targetStream.Write(headerBytesEncrypted, 0, headerBytesEncrypted.Length);
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private static Byte[] ConvertIntToBytes(int value)
        {
            byte[] intBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return intBytes;
        }

        private static int ConvertBytesToInt(Byte[] value)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(value);
            int intBytes = BitConverter.ToInt32(value, 0);
            return intBytes;
        }

        private static string DecryptLineStreamRecursively(FileStream input, string password, RijndaelManaged algorithm)
        {
            if (input.Position >= input.Length)
                return string.Empty;

            var blockLengthInBytes = new Byte[4];
            var bytesRead = input.Read(blockLengthInBytes, 0, 4);
            if (bytesRead != 4)
                return string.Empty;

            var blockLength = ConvertBytesToInt(blockLengthInBytes);
            var blockInBytes = new Byte[blockLength];
            bytesRead = input.Read(blockInBytes, 0, blockLength);
            if (bytesRead != blockLength)
                return string.Empty;

            var decriptedBlockString = DecryptStringFromBytes(blockInBytes, password, algorithm);

            return decriptedBlockString + DecryptLineStreamRecursively(input, password, algorithm);
        }

        #endregion
    }
}

