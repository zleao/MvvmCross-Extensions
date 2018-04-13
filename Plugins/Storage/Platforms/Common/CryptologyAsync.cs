// Credits to http://lukhezo.com/2011/11/06/encrypting-files-in-net-using-the-advanced-encryption-standard-aes/
// Code taken from above link, and adapted for my own implementation
// @zleao

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MvvmCross.Logging;
using MvvmCross.Exceptions;

#if ANDROID
using MvxExtensions.Plugins.Storage.Droid;
#elif XAML
using MvxExtensions.Plugins.Storage.Wpf;
#endif

namespace MvxExtensions.Plugins.Storage.Platforms.Common
{
    internal sealed class CryptologyAsync : BaseCryptologyAsync
    {
        #region Constants

        private const int ENCRYPTION_HEADER_STRING_LENGTH = 32;
        private const int ENCRYPTION_HEADER_BYTEARRAY_LENGTH = 48;
        private const int CRYPTOLINE_HEADER_SIZE = 4;
        private const char ENCRYPTION_HEADER_FILL_CHAR = ' ';
        private const string SALT = "d5fg4df5sg4ds5fg45sdfg4";
        private const int SIZE_OF_BUFFER = 1024 * 8;

        #endregion

        #region Public Methods

        internal static void SetDebugEnabled(bool value)
        {
            IsDebugEnabled = value;
        }

        internal static async Task EncryptFileAsync(string decriptedFilePath, string encryptedFilePath, string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            await FileSafeExecutionAsync(decriptedFilePath, async () =>
            {
                try
                {
                    using (var decryptedStream = CreateFileStream(decriptedFilePath, FileMode.Open, FileAccess.Read))
                        await EncryptStreamToFileAsync(decryptedStream, encryptedFilePath, password, EncryptionModeEnum.CRYPTOFULL);
                }
                catch (CryptographicException ex)
                {
                    throw new InvalidDataException("Please supply a correct password", ex);
                }
            });
        }

        internal static async Task EncryptStreamToFileAsync(Stream decryptedSourceStream, string encryptedFilePath, string password, EncryptionModeEnum encryptionMode)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            InitializeAlgorithm(password);

            await FileSafeExecutionAsync(encryptedFilePath, async () =>
            {
                switch (encryptionMode)
                {
                    case EncryptionModeEnum.CRYPTOFULL:
                        var outputFull = CreateFileStream(encryptedFilePath, FileMode.Create, FileAccess.Write);

                        await WriteEncryptionHeaderAsync(outputFull, password, encryptionMode);

                        using (var encryptedStream = new CryptoStream(outputFull, Algorithm.CreateEncryptor(), CryptoStreamMode.Write))
                            await CopyStreamAsync(decryptedSourceStream, encryptedStream);
                        break;

                    case EncryptionModeEnum.CRYPTOLINE:
                        using (var outputLine = CreateFileStream(encryptedFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            if (outputLine.Length > 0)
                            {
                                var existingFileEncryptionMode = await GetEncryptionModeAsync(outputLine, password);
                                if (existingFileEncryptionMode == InnerEncryptionModeEnum.UNKNOWN)
                                {
                                    //Copy existing file to a temporary stream
                                    if (outputLine.CanSeek)
                                        outputLine.Seek(0, SeekOrigin.Begin);
                                    var temporaryStream = new MemoryStream();
                                    await outputLine.CopyToAsync(temporaryStream);

                                    //Delete all contents from existing file
                                    outputLine.SetLength(0);
                                    if (outputLine.CanSeek)
                                        outputLine.Seek(0, SeekOrigin.Begin);

                                    //Write encryption header
                                    await WriteEncryptionHeaderAsync(outputLine, password, encryptionMode);

                                    //encrypt temporary stream to file
                                    temporaryStream.Seek(0, SeekOrigin.Begin);
                                    await EncryptCryptolineStreamAsync(temporaryStream, password, outputLine);
                                }
                                else if (existingFileEncryptionMode == InnerEncryptionModeEnum.CRYPTOLINE_OLD)
                                {
                                    //Must convert encryption using new algorithm
                                    var decryptedString = await DecryptCryptolineStreamAsync(outputLine, password, AlgorithmOld, GetDebugFilePath(encryptedFilePath, "Decryption"));
                                    var encryptedBytes = await EncryptStringToBytesAsync(decryptedString, password);
                                    outputLine.Position = 0;
                                    outputLine.SetLength(0);
                                    await WriteEncryptionHeaderAsync(outputLine, password, EncryptionModeEnum.CRYPTOLINE);

                                    //Write encrypted bytes to output, using cryptoline
                                    var encryptedBytesLengthInBytes = ConvertIntToBytes(encryptedBytes.Length);
                                    await outputLine.WriteAsync(encryptedBytesLengthInBytes, 0, encryptedBytesLengthInBytes.Length);
                                    await outputLine.WriteAsync(encryptedBytes, 0, encryptedBytes.Length);
                                }
                                else if (existingFileEncryptionMode != InnerEncryptionModeEnum.CRYPTOLINE)
                                {
                                    throw new NotSupportedException("File already encrypted with another mode of encryption. " + encryptedFilePath);
                                }
                            }

                            if (outputLine.Length == 0)
                                await WriteEncryptionHeaderAsync(outputLine, password, encryptionMode);
                            else
                                outputLine.Seek(0, SeekOrigin.End);

                            await EncryptCryptolineStreamAsync(decryptedSourceStream, password, outputLine);
                        }
                        break;

                    case EncryptionModeEnum.UNKNOWN:
                    default:
                        MvxPluginLog.Instance.Warn("EncryptStreamToFile: encryption mode unknown");
                        break;
                }
            });
        }

        internal static async Task DecryptFileAsync(string encryptedFilePath, string decryptedFilePath, string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            try
            {
                await FileSafeExecutionAsync(decryptedFilePath, async () =>
                {
                    using (var output = CreateFileStream(decryptedFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (var decryptedStream = await DecryptFileToStreamAsync(encryptedFilePath, password))
                        {
                            if (decryptedStream.CanSeek)
                                decryptedStream.Seek(0, SeekOrigin.Begin);
                            await CopyStreamAsync(decryptedStream, output);
                        }
                    }
                });
            }
            catch (AggregateException ag)
            {
                if (ag.InnerException.GetType() == typeof(CryptographicException))
                    throw new InvalidDataException("Please supply a correct password");

                MvxPluginLog.Instance.Error(ag.ToLongString());
                throw;
            }
            catch (CryptographicException ex)
            {
                throw new InvalidDataException("Please supply a correct password", ex);
            }
        }


        internal static async Task<Stream> DecryptFileToStreamAsync(string encryptedFilePath, string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            var decryptedMemoryStream = new MemoryStream();

            InitializeAlgorithm(password);

            await FileSafeExecutionAsync(encryptedFilePath, async () =>
            {
                using (var input = CreateFileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
                {
                    if (input.Length > 0)
                    {
                        switch (await GetEncryptionModeAsync(input, password))
                        {
                            case InnerEncryptionModeEnum.CRYPTOFULL:
                                var decryptedStream = new CryptoStream(decryptedMemoryStream, Algorithm.CreateDecryptor(), CryptoStreamMode.Write);
                                await CopyStreamAsync(input, decryptedStream);
                                decryptedStream.FlushFinalBlock();
                                break;

                            case InnerEncryptionModeEnum.CRYPTOFULL_OLD:
                                var decryptedStreamOld = new CryptoStream(decryptedMemoryStream, AlgorithmOld.CreateDecryptor(), CryptoStreamMode.Write);
                                await CopyStreamAsync(input, decryptedStreamOld);
                                decryptedStreamOld.FlushFinalBlock();
                                break;

                            case InnerEncryptionModeEnum.CRYPTOLINE:
                                var decryptedString = await DecryptCryptolineStreamAsync(input, password, Algorithm, GetDebugFilePath(encryptedFilePath, "Decryption"));
                                var sw = new StreamWriter(decryptedMemoryStream);
                                await sw.WriteAsync(decryptedString);
                                await sw.FlushAsync();
                                break;

                            case InnerEncryptionModeEnum.CRYPTOLINE_OLD:
                                var decryptedStringOld = await DecryptCryptolineStreamAsync(input, password, AlgorithmOld, GetDebugFilePath(encryptedFilePath, "Decryption"));
                                var swOld = new StreamWriter(decryptedMemoryStream);
                                await swOld.WriteAsync(decryptedStringOld);
                                await swOld.FlushAsync();
                                break;

                            case InnerEncryptionModeEnum.UNKNOWN:
                                MvxPluginLog.Instance.Warn("DecryptFileToStream: encryption mode unknown. Returning stream as is...");
                                if (input.CanSeek)
                                    input.Seek(0, SeekOrigin.Begin);
                                await CopyStreamAsync(input, decryptedMemoryStream);
                                break;
                        }
                    }
                }
            });

            if (decryptedMemoryStream != null && decryptedMemoryStream.Length > 0 && decryptedMemoryStream.CanSeek)
                decryptedMemoryStream.Seek(0, SeekOrigin.Begin);

            return decryptedMemoryStream;
        }

        internal static async Task<byte[]> EncryptStringToBytesAsync(string plainText, string password)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            byte[] encrypted;

            // Create an AesManaged object
            // with the specified password.
            InitializeAlgorithm(password);

            // Create a decrytor to perform the stream transform.
            ICryptoTransform encryptor = Algorithm.CreateEncryptor();

            // Create the streams used for encryption.
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        await swEncrypt.WriteAsync(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        internal static Task<string> DecryptStringFromBytesAsync(byte[] cipherText, string password)
        {
            InitializeAlgorithm(password);
            return DecryptStringFromBytesAsync(cipherText, password, Algorithm);
        }

        internal static async Task<string> DecryptStringFromBytesAsync(byte[] cipherText, string password, AesManaged algorithm)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            // Create an AesManaged object
            // with the specified password.
            InitializeAlgorithm(password);

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
                        plaintext = await srDecrypt.ReadToEndAsync();
                    }
                }
            }

            return plaintext;
        }

        internal static async Task RecoverEncryptedFileAsync(string encryptedFilePath, string recoveredFilePath, string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            var decryptedString = string.Empty;
            var recoveryStats = new CryptolineFileRecoveryStats();

            InitializeAlgorithm(password);

            await FileSafeExecutionAsync(encryptedFilePath, async () =>
            {
                using (var input = CreateFileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
                {
                    if (input.Length > 0)
                    {
                        var encryptionMode = await GetEncryptionModeAsync(input, password);
                        switch (encryptionMode)
                        {
                            case InnerEncryptionModeEnum.CRYPTOFULL:
                            case InnerEncryptionModeEnum.CRYPTOFULL_OLD:
                            case InnerEncryptionModeEnum.UNKNOWN:
                                throw new NotSupportedException(string.Format("RecoverEncryptedFile only applies to files encrypted with CRYPTOLINE algorithm. {0} ({1})", encryptedFilePath, encryptionMode));

                            case InnerEncryptionModeEnum.CRYPTOLINE:
                                recoveryStats.SourceFilePath = encryptedFilePath;
                                recoveryStats.SourceFileSyzeInBytes = input.Length;
                                recoveryStats.RecoveredFilePath = recoveredFilePath;
                                recoveryStats.ExecutionDateTime = DateTime.Now;

                                decryptedString = await RecoverCryptolineStreamAsync(input, password, Algorithm, GetDebugFilePath(recoveredFilePath), recoveryStats);

                                recoveryStats.RecoveryTime = DateTime.Now - recoveryStats.ExecutionDateTime;
                                break;

                            case InnerEncryptionModeEnum.CRYPTOLINE_OLD:
                                recoveryStats.SourceFilePath = encryptedFilePath;
                                recoveryStats.SourceFileSyzeInBytes = input.Length;
                                recoveryStats.RecoveredFilePath = recoveredFilePath;
                                recoveryStats.ExecutionDateTime = DateTime.Now;

                                decryptedString = await RecoverCryptolineStreamAsync(input, password, AlgorithmOld, GetDebugFilePath(recoveredFilePath), recoveryStats);

                                recoveryStats.RecoveryTime = DateTime.Now - recoveryStats.ExecutionDateTime;
                                break;
                        }
                    }
                }
            });

            await FileSafeExecutionAsync(recoveredFilePath, async () =>
            {
                using (var output = CreateFileStream(recoveredFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (var sw = new StreamWriter(output))
                    {
                        await sw.WriteLineAsync(decryptedString);
                        recoveryStats.RecoveredFileSyzeInBytes = output.Length;
                    }
                }
            });

            var recoveryStatsFilePath = GetRecoveryStatsFilePath(recoveredFilePath);
            await FileSafeExecutionAsync(recoveryStatsFilePath, async () =>
            {
                using (var output = CreateFileStream(recoveryStatsFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (var sw = new StreamWriter(output))
                    {
                        await sw.WriteLineAsync("Execution Date: " + recoveryStats.ExecutionDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        await sw.WriteLineAsync("Source file location: " + recoveryStats.SourceFilePath);
                        await sw.WriteLineAsync("Source file size (bytes): " + recoveryStats.SourceFileSyzeInBytes);
                        await sw.WriteLineAsync("Recovered file location: " + recoveryStats.RecoveredFilePath);
                        await sw.WriteLineAsync("Recovered file size (bytes): " + recoveryStats.RecoveredFileSyzeInBytes);
                        await sw.WriteLineAsync("Bytes lost: " + recoveryStats.BytesLost);
                        await sw.WriteLineAsync("Lines recovered count: " + recoveryStats.NumberOfLinesRecovered);
                        await sw.WriteLineAsync("Recovery time spent: " + recoveryStats.RecoveryTime);
                    }
                }
            });
        }

        internal static void DisposeAlgorithmAndPassword()
        {
            Password = null;
            if (Algorithm != null)
                Algorithm.Clear();
            if (AlgorithmOld != null)
                AlgorithmOld.Clear();
        }

        #endregion

        #region Private Methods

        private static bool IsDebugEnabled = false;
        private static AesManaged Algorithm;
        private static AesManaged AlgorithmOld;
        private static string Password;
        private static void InitializeAlgorithm(string password)
        {
            if (Password != password || Algorithm == null)
            {
                DisposeAlgorithmAndPassword();

                // Essentially, if you want to use AesManaged as AES you need to make sure that:
                // 1.The block size is set to 128 bits
                // 2.You are not using CFB mode, or if you are the feedback size is also 128 bits

                Password = password;

                Algorithm = new AesManaged { KeySize = 256, BlockSize = 128 };
                AlgorithmOld = new AesManaged { KeySize = 256, BlockSize = 128 };

                var key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(SALT));

                Algorithm.Key = key.GetBytes(Algorithm.KeySize / 8);
                AlgorithmOld.Key = Algorithm.Key;
                AlgorithmOld.IV = key.GetBytes(Algorithm.BlockSize / 8);
                key.Reset(); //needed for WinRT compatibility
                Algorithm.IV = key.GetBytes(Algorithm.BlockSize / 8);
            }
        }

        private static async Task CopyStreamAsync(Stream input, Stream output)
        {
            if (input.Length > 0)
            {
                byte[] buffer = new byte[SIZE_OF_BUFFER];
                int read;

                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    await output.WriteAsync(buffer, 0, read);
                }
            }
        }

        private static async Task<InnerEncryptionModeEnum> GetEncryptionModeAsync(Stream streamToCheck, string password)
        {
            try
            {
                if (streamToCheck == null || streamToCheck.Length < ENCRYPTION_HEADER_BYTEARRAY_LENGTH)
                    return InnerEncryptionModeEnum.UNKNOWN;

                var initialStreamPosition = streamToCheck.Position;

                var headerBytes = new Byte[ENCRYPTION_HEADER_BYTEARRAY_LENGTH];
                var bytesRead = await streamToCheck.ReadAsync(headerBytes, 0, ENCRYPTION_HEADER_BYTEARRAY_LENGTH);
                if (bytesRead != ENCRYPTION_HEADER_BYTEARRAY_LENGTH)
                {
                    if (streamToCheck.CanSeek)
                        streamToCheck.Seek(initialStreamPosition, SeekOrigin.Begin);
                    return InnerEncryptionModeEnum.UNKNOWN;
                }

                var result = await ConvertHeaderBytesToEnumAsync(headerBytes, Algorithm, password);
                if (result == InnerEncryptionModeEnum.UNKNOWN)
                {
                    //try to use the old algorithm
                    var resultOld = await ConvertHeaderBytesToEnumAsync(headerBytes, AlgorithmOld, password);
                    if (resultOld == InnerEncryptionModeEnum.UNKNOWN)
                    {
                        if (streamToCheck.CanSeek)
                            streamToCheck.Seek(initialStreamPosition, SeekOrigin.Begin);
                        result = InnerEncryptionModeEnum.UNKNOWN;
                    }
                    else
                    {
                        switch (resultOld)
                        {
                            case InnerEncryptionModeEnum.CRYPTOFULL:
                                result = InnerEncryptionModeEnum.CRYPTOFULL_OLD;
                                break;

                            case InnerEncryptionModeEnum.CRYPTOLINE:
                                result = InnerEncryptionModeEnum.CRYPTOLINE_OLD;
                                break;
                        }
                    }
                }

                if (streamToCheck.CanSeek)
                    streamToCheck.Seek(initialStreamPosition + ENCRYPTION_HEADER_BYTEARRAY_LENGTH, SeekOrigin.Begin);

                return result;
            }
            catch (Exception ex)
            {
                MvxPluginLog.Instance.Error(ex.Message);
                return InnerEncryptionModeEnum.UNKNOWN;
            }
        }

        private static async Task<InnerEncryptionModeEnum> ConvertHeaderBytesToEnumAsync(byte[] headerBytes, AesManaged algorithm, string password)
        {
            var result = InnerEncryptionModeEnum.UNKNOWN;

            var headerString = await DecryptStringFromBytesAsync(headerBytes, password, algorithm);
            var trimmedHeaderString = (headerString != null ? headerString.Trim(ENCRYPTION_HEADER_FILL_CHAR) : headerString);
            if (string.IsNullOrEmpty(trimmedHeaderString) || !Enum.TryParse(trimmedHeaderString, out result))
            {
                result = InnerEncryptionModeEnum.UNKNOWN;
            }

            return result;
        }

        private static async Task WriteEncryptionHeaderAsync(Stream targetStream, string password, EncryptionModeEnum mode)
        {
            switch (mode)
            {
                case EncryptionModeEnum.UNKNOWN:
                    return;
            }

            var headerString = mode.ToString();
            headerString = headerString.PadRight(ENCRYPTION_HEADER_STRING_LENGTH, ENCRYPTION_HEADER_FILL_CHAR);

            var headerBytesEncrypted = await EncryptStringToBytesAsync(headerString, password);

            await targetStream.WriteAsync(headerBytesEncrypted, 0, headerBytesEncrypted.Length);
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

        private static async Task<string> DecryptCryptolineStreamAsync(FileStream input, string password, AesManaged algorithm, string debugFilePath)
        {
            await WriteToDebugLogAsync("Trying to decrypt file with size " + input.Length + Environment.NewLine, debugFilePath);

            var recoveredString = string.Empty;

            var res = new Tuple<bool, string>(true, string.Empty);
            do
            {
                res = await TryDecryptLine(input, password, algorithm, debugFilePath);
                recoveredString += res.Item2;
            } while (res.Item1);

            return recoveredString;
        }
        private static async Task<Tuple<bool, string>> TryDecryptLine(FileStream input, string password, AesManaged algorithm, string debugFilePath)
        {
            await WriteToDebugLogAsync(string.Format("Position: {0}  Length: {1}", input.Position, input.Length), debugFilePath);

            if (input.Position + 4 >= input.Length)
            {
                await WriteToDebugLogAsync("Reached end of file. Returning empty string for this line", debugFilePath);
                return new Tuple<bool, string>(false, string.Empty);
            }

            var blockLengthInBytes = new Byte[4];
            var bytesRead = await input.ReadAsync(blockLengthInBytes, 0, 4);
            if (bytesRead != 4)
            {
                await WriteToDebugLogAsync("Not enough bytes to line header: " + bytesRead, debugFilePath);
                return new Tuple<bool, string>(false, string.Empty);
            }

            var blockLength = ConvertBytesToInt(blockLengthInBytes);

            await WriteToDebugLogAsync(string.Format("blockLength [{0},{1},{2},{3}] => {4}",
                                                    blockLengthInBytes[0],
                                                    blockLengthInBytes[1],
                                                    blockLengthInBytes[2],
                                                    blockLengthInBytes[3],
                                                    blockLength),
                                        debugFilePath);

            var blockInBytes = new Byte[blockLength];
            bytesRead = await input.ReadAsync(blockInBytes, 0, blockLength);
            if (bytesRead != blockLength)
            {
                await WriteToDebugLogAsync("Not enough bytes to line body: " + bytesRead, debugFilePath);
                return new Tuple<bool, string>(false, string.Empty);
            }

            var decriptedBlockString = await DecryptStringFromBytesAsync(blockInBytes, password, algorithm);

            await WriteToDebugLogAsync(decriptedBlockString + Environment.NewLine, debugFilePath);

            return new Tuple<bool, string>(true, decriptedBlockString);
        }

        private static async Task EncryptCryptolineStreamAsync(Stream decryptedSourceStream, string password, FileStream outputLine)
        {
            using (var sr = new StreamReader(decryptedSourceStream))
            {
                var encryptedBytes = await EncryptStringToBytesAsync(sr.ReadToEnd(), password);
                var encryptedBytesLengthInBytes = ConvertIntToBytes(encryptedBytes.Length);

                await outputLine.WriteAsync(encryptedBytesLengthInBytes, 0, encryptedBytesLengthInBytes.Length);
                await outputLine.WriteAsync(encryptedBytes, 0, encryptedBytes.Length);
            }
        }

        private static async Task WriteToDebugLogAsync(string debugLog, string debugFilePath, bool addNewLine = true)
        {
            if (IsDebugEnabled && !string.IsNullOrEmpty(debugFilePath))
            {
                await FileSafeExecutionAsync(debugFilePath, async () =>
                {
                    using (var fs = CreateFileStream(debugFilePath, FileMode.Append, FileAccess.Write))
                    {
                        using (var sw = new StreamWriter(fs))
                        {
                            if (addNewLine)
                                await sw.WriteLineAsync(debugLog);
                            else
                                await sw.WriteAsync(debugLog);
                        }
                    }
                });
            }
        }

        private static string GetDebugFilePath(string targetFilePath, string prefix = "")
        {
            var path = string.Empty;

            if (!string.IsNullOrEmpty(targetFilePath))
            {
                var targetFileName = Path.GetFileNameWithoutExtension(targetFilePath);
                path = targetFilePath.Replace(targetFileName, targetFileName + string.Format("_{0}Debug", prefix));
            }

            return path;
        }

        private static string GetRecoveryStatsFilePath(string targetFilePath)
        {
            var path = string.Empty;

            if (!string.IsNullOrEmpty(targetFilePath))
            {
                var targetFileName = Path.GetFileNameWithoutExtension(targetFilePath);
                path = targetFilePath.Replace(targetFileName, targetFileName + "_Stats");
            }

            return path;
        }

        private static async Task<string> RecoverCryptolineStreamAsync(FileStream input, string password, AesManaged algorithm, string debugFilePath, CryptolineFileRecoveryStats recoveryStats)
        {
            await WriteToDebugLogAsync("Trying to recover encrypted file with size " + input.Length + Environment.NewLine, debugFilePath);

            var recoveredString = string.Empty;
            var res = new Tuple<bool, string>(true, string.Empty);
            do
            {
                res = await TryRecoverEncryptedLine(input, password, algorithm, debugFilePath, recoveryStats);
                recoveredString += res.Item2;
            } while (res.Item1);

            return recoveredString;
        }
        private static async Task<Tuple<bool, string>> TryRecoverEncryptedLine(FileStream input, string password, AesManaged algorithm, string debugFilePath, CryptolineFileRecoveryStats recoveryStats)
        {
            await WriteToDebugLogAsync(string.Format("Position: {0}   Length: {1}", input.Position, input.Length), debugFilePath);

            if (input.Position + CRYPTOLINE_HEADER_SIZE >= input.Length)
            {
                await WriteToDebugLogAsync("Reached end of file. Returning empty string for this line", debugFilePath);
                recoveryStats.BytesLost += Math.Abs(input.Length - input.Position);
                return new Tuple<bool, string>(false, string.Empty);
            }

            var blockLengthInBytes = new Byte[CRYPTOLINE_HEADER_SIZE];
            var bytesRead = await input.ReadAsync(blockLengthInBytes, 0, CRYPTOLINE_HEADER_SIZE);
            if (bytesRead != CRYPTOLINE_HEADER_SIZE)
            {
                await WriteToDebugLogAsync(string.Format("Not enough bytes to line header: (bytesread: {0})", bytesRead), debugFilePath);
                recoveryStats.BytesLost += Math.Abs(input.Length - (input.Position - bytesRead));
                return new Tuple<bool, string>(false, string.Empty);
            }

            var blockLength = ConvertBytesToInt(blockLengthInBytes);

            await WriteToDebugLogAsync(string.Format("blockLength [{0},{1},{2},{3}] => {4}",
                                                    blockLengthInBytes[0],
                                                    blockLengthInBytes[1],
                                                    blockLengthInBytes[2],
                                                    blockLengthInBytes[3],
                                                    blockLength),
                                        debugFilePath);

            var decriptedBlockString = string.Empty;
            if (blockLength <= 0 || (input.Position + blockLength) > input.Length)
            {
                //Invalid blockLength
                await WriteToDebugLogAsync(string.Format("blockLength invalid. Trying next {0} bytes" + Environment.NewLine, CRYPTOLINE_HEADER_SIZE), debugFilePath);
                recoveryStats.BytesLost += CRYPTOLINE_HEADER_SIZE;
            }
            else
            {
                Exception e = null;
                var initialPosition = input.Position;
                try
                {
                    await WriteToDebugLogAsync(string.Format("Trying to read {0} Bytes starting on position {1}", blockLength, input.Position), debugFilePath);

                    var blockInBytes = new Byte[blockLength];
                    bytesRead = input.Read(blockInBytes, 0, blockLength);
                    if (bytesRead != blockLength)
                    {
                        await WriteToDebugLogAsync(string.Format("Not enough bytes to line body: (bytesRead: {0})", bytesRead), debugFilePath);
                        recoveryStats.BytesLost += Math.Abs(input.Length - (input.Position - bytesRead));
                        return new Tuple<bool, string>(false, string.Empty);
                    }
                    else
                    {
                        decriptedBlockString = await DecryptStringFromBytesAsync(blockInBytes, password, algorithm);
                        await WriteToDebugLogAsync(decriptedBlockString + Environment.NewLine, debugFilePath);
                        recoveryStats.NumberOfLinesRecovered++;
                    }
                }
                catch (Exception ex)
                {
                    e = ex;
                }

                if (e != null)
                {
                    await WriteToDebugLogAsync(e.ToLongString() + Environment.NewLine, debugFilePath);
                    decriptedBlockString = string.Empty;
                    input.Position = initialPosition;
                    recoveryStats.BytesLost += CRYPTOLINE_HEADER_SIZE;
                }
            }

            return new Tuple<bool, string>(true, decriptedBlockString);
        }

        private static async Task FileSafeExecutionAsync(string key, Func<Task> actionAsync)
        {
            if (Thread.CurrentThread.ManagedThreadId == 1)
                await Task.Run(() => { return FileSafeExecutionAsync(key, actionAsync); });
            else
            {
                await StorageHelpers.GetSemaphore(key).WaitAsync();

                try
                {
                    await actionAsync.Invoke();
                }
                finally
                {
                    StorageHelpers.GetSemaphore(key).Release();
                }
            }
        }

        #endregion

        enum InnerEncryptionModeEnum
        {
            UNKNOWN,
            /// <summary>
            /// Full file encryption using Crypto
            /// </summary>
            CRYPTOFULL,
            /// <summary>
            /// Full file encryption using Crypto with old implementation
            /// </summary>
            CRYPTOFULL_OLD,
            /// <summary>
            /// Line encryption using Crypto
            /// </summary>
            CRYPTOLINE,
            /// <summary>
            /// Line encryption using Crypto with old implementation
            /// </summary>
            CRYPTOLINE_OLD,
        }
    }
}
