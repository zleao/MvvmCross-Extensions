using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.Exceptions;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvxExtensions.Core.Extensions;
using MvxExtensions.Plugins.Logger;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Plugins.Notification.Messages;
using MvxExtensions.Plugins.Storage;
using MvxExtensions.Plugins.Storage.Models;
using Playground.Core.Resources;
using Playground.Forms.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Playground.Forms.Core.ViewModels
{
    public class StorageViewModel : BaseViewModel
    {
        #region Constants

        private const string PASSWORD = "password";
        private const string ROOTFOLDER = "StorageTest";
        private const string METHODBASENAME = "CaseTest";

        private const int NumberOfCaseTests = 19;

        #endregion

        #region Properties

        public IStorageManager Storage { get; }
        public IStorageEncryptionManager StorageEncryption { get; }
        public ILogger Logger { get; }

        public ObservableCollection<CaseTest> CaseTests { get; } = new ObservableCollection<CaseTest>();

        #endregion

        #region Command

        public ICommand CaseTestCommand { get; private set; }

        #endregion

        #region Constructor

        public StorageViewModel(IMvxJsonConverter jsonConverter,
                                INotificationService notificationManager,
                                IMvxLogProvider logProvider,
                                IMvxNavigationService navigationService,
                                IStorageManager storage,
                                IStorageEncryptionManager storageEncryption,
                                ILogger logger)
            : base(nameof(StorageViewModel), jsonConverter, notificationManager, logProvider, navigationService)
        {
            Storage = storage;
            StorageEncryption = storageEncryption;
            Logger = logger;
        }

        #endregion

        #region Methods

        public override Task Initialize()
        {
            CaseTestCommand = new MvxCommand<CaseTest>(OnCaseTest);

            var caseTestTitleTemplate = TextSource.GetText(TextResourcesKeys.Label_Button_CaseTest_Template);

            for (int i = 1; i <= NumberOfCaseTests; i++)
            {
                CaseTests.Add(new CaseTest(i, caseTestTitleTemplate.SafeFormatTemplate(i), TextSource.GetText(TextResourcesKeys.Label_CaseTest_Description_Template.SafeFormatTemplate(i))));
            }

            return base.Initialize();
        }

        private async void OnCaseTest(CaseTest caseTest)
        {
            if (caseTest != null)
            {
                var method = GetType().GetMethod(METHODBASENAME + caseTest.Id);
                if (method != null)
                    method.Invoke(this, null);
                else
                    await NotificationManager.PublishErrorNotificationAsync("No method found for " + caseTest.Name).ConfigureAwait(false);
            }
            else
            {
                await NotificationManager.PublishErrorNotificationAsync("Case test is empty!").ConfigureAwait(false);
            }
        }

        public async void CaseTest1()
        {
            await DoWorkAsync(CaseTest1Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(1), false).ConfigureAwait(false);
        }
        private async Task CaseTest1Task()
        {
            try
            {
                //Test the file creation
                var createdFileName = CreateFilePath("CaseTest01_FileCreated.txt");

                await Storage.EnsureFolderExistsAsync(StorageLocation.SharedDataDirectory, ROOTFOLDER).ConfigureAwait(false);
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, createdFileName, GetFileTextString()).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(1)).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
                throw;
            }
        }

        public async void CaseTest2()
        {
            await DoWorkAsync(CaseTest2Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(2), false).ConfigureAwait(false);
        }
        private async Task CaseTest2Task()
        {
            try
            {
                //Create file with sigle string
                //Read file contents and show them in a notification
                var createdFileName = CreateFilePath("CaseTest02_FileCreated.txt");

                await Storage.EnsureFolderExistsAsync(StorageLocation.SharedDataDirectory, ROOTFOLDER).ConfigureAwait(false);
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, createdFileName, "Case Test 2 string").ConfigureAwait(false);
                var str = await Storage.TryReadTextFileAsync(StorageLocation.SharedDataDirectory, createdFileName).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(2)).ConfigureAwait(false);
                await NotificationManager.PublishInfoNotificationAsync(str, NotificationModeEnum.MessageBox).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest3()
        {
            await DoWorkAsync(CaseTest3Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(3), false).ConfigureAwait(false);
        }
        private async Task CaseTest3Task()
        {
            try
            {
                //Check if file from case test 2 exists
                var ct2FileName = CreateFilePath("CaseTest02_FileCreated.txt");
                var fileExists = await Storage.FileExistsAsync(StorageLocation.SharedDataDirectory, ct2FileName).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(3)).ConfigureAwait(false);
                await NotificationManager.PublishInfoNotificationAsync("'{0}' {1}!".SafeFormatTemplate(ct2FileName, fileExists ? "found" : "not found"), NotificationModeEnum.MessageBox).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest4()
        {
            await DoWorkAsync(CaseTest4Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(4), false).ConfigureAwait(false);
        }
        private async Task CaseTest4Task()
        {
            try
            {
                //Check if file from case test 2 exists
                var ct2FileName = CreateFilePath("CaseTest02_FileCreated.txt");
                await Storage.DeleteFileAsync(StorageLocation.SharedDataDirectory, ct2FileName).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(4)).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest5()
        {
            await DoWorkAsync(CaseTest5Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(5), false).ConfigureAwait(false);
        }
        private async Task CaseTest5Task()
        {
            try
            {
                //Check creation time of file from case test 2 
                var ct2FileName = CreateFilePath("CaseTest02_FileCreated.txt");
                var dt = await Storage.GetFileCreationTimeAsync(StorageLocation.SharedDataDirectory, ct2FileName).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(5)).ConfigureAwait(false);
                await NotificationManager.PublishInfoNotificationAsync("Creation time: " + dt.ToString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest6()
        {
            await DoWorkAsync(CaseTest6Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(6), false).ConfigureAwait(false);
        }
        private async Task CaseTest6Task()
        {
            try
            {
                //Get and count all files in storage root folder
                var fileList = await Storage.GetFilesInAsync(StorageLocation.SharedDataDirectory, true, ROOTFOLDER).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(5)).ConfigureAwait(false);
                await NotificationManager.PublishInfoNotificationAsync(fileList.SafeCount() > 0 ? "Found {0} file(s)".SafeFormatTemplate(fileList.Count()) : "No files found", NotificationModeEnum.MessageBox).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest7()
        {
            await DoWorkAsync(CaseTest7Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(7), false).ConfigureAwait(false);
        }
        private async Task CaseTest7Task()
        {
            try
            {
                //Delete storage root folder
                await Storage.DeleteFolderAsync(StorageLocation.SharedDataDirectory, ROOTFOLDER).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(7)).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest8()
        {
            await DoWorkAsync(CaseTest8Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(8), false).ConfigureAwait(false);
        }
        private async Task CaseTest8Task()
        {
            try
            {
                //Create decompressed file
                //Compress and ceompress that same file
                var originalFileName = CreateFilePath("CaseTest08_OriginalFile.txt");
                var compressedFileName = CreateFilePath("CaseTest08_CompressedFile.txt");
                var decompressedFileName = CreateFilePath("CaseTest08_DecompressedFile.txt");

                using (var ms = new MemoryStream())
                {
                    using (var sw = new StreamWriter(ms))
                    {
                        await sw.WriteAsync(GetFileTextString()).ConfigureAwait(false);
                        await sw.FlushAsync().ConfigureAwait(false);
                        ms.Position = 0;
                        await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, originalFileName, ms).ConfigureAwait(false);
                        ms.Position = 0;
                        using (var cs = await Storage.CompressStreamAsync(ms).ConfigureAwait(false))
                        {
                            await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, compressedFileName, cs).ConfigureAwait(false);
                            cs.Position = 0;
                            using (var ds = await Storage.DecompressStreamAsync(cs).ConfigureAwait(false))
                            {
                                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, decompressedFileName, ds).ConfigureAwait(false);
                            }
                        }
                    }
                }

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(8)).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest9()
        {
            await DoWorkAsync(CaseTest9Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(9), false).ConfigureAwait(false);
        }
        private async Task CaseTest9Task()
        {
            try
            {
                //Create original file
                //Clone that same file
                var originalFileName = CreateFilePath("CaseTest09_OriginalFile.txt");
                var clonedFileName = CreateFilePath("CaseTest09_ClonedFile.txt");

                await CreateOriginalFileAsync(originalFileName).ConfigureAwait(false);
                await Storage.CloneFileAsync(StorageLocation.SharedDataDirectory, originalFileName, StorageLocation.SharedDataDirectory, clonedFileName, true).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(9)).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest10()
        {
            await DoWorkAsync(CaseTest10Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(10), false).ConfigureAwait(false);
        }
        private async Task CaseTest10Task()
        {
            try
            {
                //Create original file
                //Move that same file to different folder
                var originalFileName = CreateFilePath("CaseTest10_OriginalFile.txt");
                var movedFileName = CreateFilePath(Storage.PathCombine("MoveFolder", "CaseTest10_MovedFile.txt"));

                await CreateOriginalFileAsync(originalFileName).ConfigureAwait(false);
                await Storage.TryMoveAsync(StorageLocation.SharedDataDirectory, originalFileName, StorageLocation.SharedDataDirectory, movedFileName, false).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(10)).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest11()
        {
            await DoWorkAsync(CaseTest11Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(11), false).ConfigureAwait(false);
        }
        private async Task CaseTest11Task()
        {
            //Create original file
            //Encrypt file
            //Decrypt file
            try
            {
                var originalFileName = CreateFilePath("CaseTest11_Original.txt");
                var encryptedFileName = CreateFilePath("CaseTest11_OriginalEncrypted.txt");
                var decryptedFileName = CreateFilePath("CaseTest11_OriginalDecrypted.txt");

                await CreateOriginalFileAsync(originalFileName).ConfigureAwait(false);
                await StorageEncryption.EncryptFileAsync(StorageLocation.SharedDataDirectory, originalFileName, encryptedFileName, PASSWORD).ConfigureAwait(false);
                await StorageEncryption.DecryptFileAsync(StorageLocation.SharedDataDirectory, encryptedFileName, decryptedFileName, PASSWORD).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(11)).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest12()
        {
            await DoWorkAsync(CaseTest12Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(12), false).ConfigureAwait(false);
        }
        private async Task CaseTest12Task()
        {
            //Create encrypted file
            //Decrypt file
            try
            {
                var encryptedFileName = CreateFilePath("CaseTest12_Encrypted.txt");
                var decryptedFileName = CreateFilePath("CaseTest12_Decrypted.txt");

                await StorageEncryption.WriteEncryptedFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, encryptedFileName, GetFileTextString(), PASSWORD).ConfigureAwait(false);
                await StorageEncryption.DecryptFileAsync(StorageLocation.SharedDataDirectory, encryptedFileName, decryptedFileName, PASSWORD).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(12)).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest13()
        {
            await DoWorkAsync(CaseTest13Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(13), false).ConfigureAwait(false);
        }
        private async Task CaseTest13Task()
        {
            //Criar ficheiro encriptado
            //Adiconar texto ao ficheiro encriptado
            //Desencriptar ficheiro
            try
            {
                var encryptedFileName = CreateFilePath("CaseTest13_Encrypted.txt");
                var originalDecryptedFileName = CreateFilePath("CaseTest13_OriginalDecrypted.txt");
                var decryptedFileName = CreateFilePath("CaseTest13_Decrypted.txt");

                await StorageEncryption.WriteEncryptedFileAsync(StorageLocation.SharedDataDirectory, StorageMode.CreateOrAppend, encryptedFileName, GetFileTextString(), PASSWORD).ConfigureAwait(false);
                await StorageEncryption.DecryptFileAsync(StorageLocation.SharedDataDirectory, encryptedFileName, originalDecryptedFileName, PASSWORD).ConfigureAwait(false);
                await StorageEncryption.WriteEncryptedFileAsync(StorageLocation.SharedDataDirectory, StorageMode.CreateOrAppend, encryptedFileName, "TEXT ADDED WITH ENCRYPTION" + Environment.NewLine, PASSWORD).ConfigureAwait(false);
                await StorageEncryption.DecryptFileAsync(StorageLocation.SharedDataDirectory, encryptedFileName, decryptedFileName, PASSWORD).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(13)).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest14()
        {
            await DoWorkAsync(CaseTest14Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(14), false).ConfigureAwait(false);
        }
        private async Task CaseTest14Task()
        {
            //Adicionar varias linhas (uma a uma) a um ficheiro encriptado
            //Desencriptar ficheiro
            try
            {
                var encryptedFileName = CreateFilePath("CaseTest14_Encrypted.txt");
                var decryptedFileName = CreateFilePath("CaseTest14_Decrypted.txt");
                var normalFileName = CreateFilePath("CaseTest14_Normal.txt");

                await Logger.LogAsyncMethodExecutionTimeAsync(async () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        await Logger.LogAsyncMethodExecutionTimeAsync(() => StorageEncryption.WriteEncryptedFileAsync(StorageLocation.SharedDataDirectory, StorageMode.CreateOrAppend, encryptedFileName, "Line " + i + Environment.NewLine, PASSWORD),
                                                                      "WriteEncryptedFile #" + i).ConfigureAwait(false);
                    }
                }, "WriteEncryptedFile").ConfigureAwait(false);

                await Logger.LogAsyncMethodExecutionTimeAsync(() => StorageEncryption.DecryptFileAsync(StorageLocation.SharedDataDirectory, encryptedFileName, decryptedFileName, PASSWORD), "DecryptFile").ConfigureAwait(false);

                await Logger.LogAsyncMethodExecutionTimeAsync(async () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        await Logger.LogAsyncMethodExecutionTimeAsync(() => Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.CreateOrAppend, normalFileName, "Line " + i + Environment.NewLine),
                                                                      "WriteFile #" + i).ConfigureAwait(false);
                    }
                }, "WriteFile").ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(14)).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest15()
        {
            await DoWorkAsync(CaseTest15Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(15), false).ConfigureAwait(false);
        }
        private async Task CaseTest15Task()
        {
            //Criar ficheiro
            //Adicionar varias linhas encriptadas (uma a uma) a um ficheiro inicialmente desencriptado
            //Desencriptar ficheiro
            try
            {
                var originalFileName = CreateFilePath("CaseTest15_Original.txt");
                var originalClonedFileName = CreateFilePath("CaseTest15_OriginalCloned.txt");
                var decryptedFileName = CreateFilePath("CaseTest15_OriginalDecrypted.txt");

                await CreateOriginalFileAsync(originalFileName).ConfigureAwait(false);

                await Storage.CloneFileAsync(StorageLocation.SharedDataDirectory, originalFileName, StorageLocation.SharedDataDirectory, originalClonedFileName, true).ConfigureAwait(false);

                await StorageEncryption.WriteEncryptedFileAsync(StorageLocation.SharedDataDirectory, StorageMode.CreateOrAppend, originalFileName, "Encrypted text added to non encrypted file", PASSWORD).ConfigureAwait(false);

                await StorageEncryption.DecryptFileAsync(StorageLocation.SharedDataDirectory, originalFileName, decryptedFileName, PASSWORD).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(15)).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest16()
        {
            await DoWorkAsync(CaseTest16Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(16), false).ConfigureAwait(false);
        }
        private async Task CaseTest16Task()
        {
            //Criar ficheiro encriptado com uma linha simples
            //Ler ficheiro encriptado e mostrar o texto desencriptado numa mensagem popup
            try
            {
                var encryptedFileName = CreateFilePath("CaseTest16_Encrypted.txt");

                await StorageEncryption.WriteEncryptedFileAsync(StorageLocation.SharedDataDirectory, StorageMode.CreateOrAppend, encryptedFileName, "Just some text", PASSWORD).ConfigureAwait(false);
                var output = await StorageEncryption.TryReadTextEncryptedFileAsync(StorageLocation.SharedDataDirectory, encryptedFileName, PASSWORD).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(16)).ConfigureAwait(false);
                await NotificationManager.PublishInfoNotificationAsync(output, NotificationModeEnum.MessageBox).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest17()
        {
            await DoWorkAsync(CaseTest17Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(17), false).ConfigureAwait(false);
        }
        private async Task CaseTest17Task()
        {
            //Desencriptar ficheiro criado com a versão 'antiga' de encriptação
            //Desencriptar ficheiro criado com a versão 'nova' de encriptação
            try
            {
                var encryptedOldFileName = CreateFilePath("CaseTest17_EncryptedOld.txt");
                var encryptedNewFileName = CreateFilePath("CaseTest17_EncryptedNew.txt");
                var decryptedOldFileName = CreateFilePath("CaseTest17_DecryptedOld.txt");
                var decryptedNewFileName = CreateFilePath("CaseTest17_DecryptedNew.txt");
                var encryptedOldModifiedFileName = CreateFilePath("CaseTest17_EncryptedOldModified.txt");
                var encryptedNewModifiedFileName = CreateFilePath("CaseTest17_EncryptedNewModified.txt");
                var decryptedOldModifiedFileName = CreateFilePath("CaseTest17_DecryptedOldModified.txt");
                var decryptedNewModifiedFileName = CreateFilePath("CaseTest17_DecryptedNewModified.txt");

                var rl = Mvx.IoCProvider.Resolve<IMvxResourceLoader>();

                //Decrypt 'old encrypted' file
                await Storage.CloneFileFromAppResourcesAsync("EncryptedFiles/encryptedOld.356194055156252.txt", StorageLocation.SharedDataDirectory, encryptedOldFileName).ConfigureAwait(false);
                await StorageEncryption.DecryptFileAsync(StorageLocation.SharedDataDirectory, encryptedOldFileName, decryptedOldFileName, "356194055156252").ConfigureAwait(false);

                //Decrypt 'new encrypted' file
                await Storage.CloneFileFromAppResourcesAsync("EncryptedFiles/encryptedNew.password.txt", StorageLocation.SharedDataDirectory, encryptedNewFileName).ConfigureAwait(false);
                await StorageEncryption.DecryptFileAsync(StorageLocation.SharedDataDirectory, encryptedNewFileName, decryptedNewFileName, "password").ConfigureAwait(false);

                //Write to 'old encrypted' file and decrypt it
                await Storage.CloneFileFromAppResourcesAsync("EncryptedFiles/encryptedOld.356194055156252.txt", StorageLocation.SharedDataDirectory, encryptedOldModifiedFileName).ConfigureAwait(false);
                await StorageEncryption.WriteEncryptedFileAsync(StorageLocation.SharedDataDirectory, StorageMode.CreateOrAppend, encryptedOldModifiedFileName, "added text to old encrypted file", "356194055156252").ConfigureAwait(false);
                await StorageEncryption.DecryptFileAsync(StorageLocation.SharedDataDirectory, encryptedOldModifiedFileName, decryptedOldModifiedFileName, "356194055156252").ConfigureAwait(false);

                //Write to 'new encrypted' file and decrypt it
                await Storage.CloneFileFromAppResourcesAsync("EncryptedFiles/encryptedNew.password.txt", StorageLocation.SharedDataDirectory, encryptedNewModifiedFileName).ConfigureAwait(false);
                await StorageEncryption.WriteEncryptedFileAsync(StorageLocation.SharedDataDirectory, StorageMode.CreateOrAppend, encryptedNewModifiedFileName, "added text to old encrypted file", "password").ConfigureAwait(false);
                await StorageEncryption.DecryptFileAsync(StorageLocation.SharedDataDirectory, encryptedNewModifiedFileName, decryptedNewModifiedFileName, "password").ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(17)).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest18()
        {
            await DoWorkAsync(CaseTest18Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(18), false).ConfigureAwait(false);
        }
        private async Task CaseTest18Task()
        {
            try
            {
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder1", "File1.txt")), "Contents of file 1 in folder 1").ConfigureAwait(false);
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder1", "File2.txt")), "Contents of file 2 in folder 1").ConfigureAwait(false);
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder2", "File1.txt")), "Contents of file 1 in folder 2").ConfigureAwait(false);
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder2", "File2.txt")), "Contents of file 2 in folder 2").ConfigureAwait(false);
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder3", "File1.txt")), "Contents of file 1 in folder 3").ConfigureAwait(false);

                //Get and count all files in storage root folder
                var fileList = await Storage.GetFilesInAsync(StorageLocation.SharedDataDirectory, true, ROOTFOLDER, "txt").ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(18)).ConfigureAwait(false);
                await NotificationManager.PublishInfoNotificationAsync(fileList.SafeCount() > 0 ? "Found {0} file(s)".SafeFormatTemplate(fileList.Count()) : "No files found", NotificationModeEnum.MessageBox).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        public async void CaseTest19()
        {
            await DoWorkAsync(CaseTest19Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(19), false).ConfigureAwait(false);
        }
        private async Task CaseTest19Task()
        {
            try
            {
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder1", "File1.txt")), "Contents of file 1 in folder 1").ConfigureAwait(false);
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder1", "File2.txt")), "Contents of file 2 in folder 1").ConfigureAwait(false);
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder2", "File1.txt")), "Contents of file 1 in folder 2").ConfigureAwait(false);
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder2", "File2.txt")), "Contents of file 2 in folder 2").ConfigureAwait(false);
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder3", "File1.txt")), "Contents of file 1 in folder 3").ConfigureAwait(false);

                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder1", "File1.m2u")), "Contents of file 1 in folder 1").ConfigureAwait(false);
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder1", "File2.m2u")), "Contents of file 2 in folder 1").ConfigureAwait(false);
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder2", "File1.m2u")), "Contents of file 1 in folder 2").ConfigureAwait(false);
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder2", "File2.m2u")), "Contents of file 2 in folder 2").ConfigureAwait(false);
                await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, CreateFilePath(Path.Combine("Folder3", "File1.m2u")), "Contents of file 1 in folder 3").ConfigureAwait(false);

                //Get and count all files in storage root folder
                var fileList = await Storage.GetFilesInAsync(StorageLocation.SharedDataDirectory, true, ROOTFOLDER).ConfigureAwait(false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(19)).ConfigureAwait(false);
                await NotificationManager.PublishInfoNotificationAsync(fileList.SafeCount() > 0 ? "Found {0} file(s)".SafeFormatTemplate(fileList.Count()) : "No files found", NotificationModeEnum.MessageBox).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox).ConfigureAwait(false);
                Debugger.Break();
            }
        }

        private async Task CreateOriginalFileAsync(string originalFileName)
        {
            await Storage.WriteFileAsync(StorageLocation.SharedDataDirectory, StorageMode.Create, originalFileName, GetFileTextString()).ConfigureAwait(false);
        }

        private string GetFileTextString()
        {
            var fileText = string.Empty;
            for (int i = 0; i < 10; i++)
                fileText += "Log text number #{0}{1}".SafeFormatTemplate(i, Environment.NewLine);

            return fileText;
        }

        private string CreateFilePath(string fileName)
        {
            return Storage.PathCombine(ROOTFOLDER, fileName);
        }

        #endregion
    }
}
