using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.Exceptions;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvxExtensions.Extensions;
using MvxExtensions.Plugins.Logger;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Plugins.Notification.Messages;
using MvxExtensions.Plugins.Storage;
using MvxExtensions.Plugins.Storage.Models;
using Playground.Core.Models;
using Playground.Core.Resources;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Playground.Core.ViewModels
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

        public IStorageManager Storage{ get; private set; }
        public IStorageEncryptionManager StorageEncryption{ get; private set; }
        public ILogger Logger{ get; private set; }

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
                    await NotificationManager.PublishErrorNotificationAsync("No method found for " + caseTest.Name);
            }
            else
            {
                await NotificationManager.PublishErrorNotificationAsync("Case test is empty!");
            }
        }

        public async void CaseTest1()
        {
            await DoWorkAsync(CaseTest1Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(1), false);
        }
        private async Task CaseTest1Task()
        {
            try
            {
                //Test the file creation
                var createdFileName = CreateFilePath("CaseTest01_FileCreated.txt");

                await Storage.EnsureFolderExistsAsync(StorageLocation.ExternalPublic, ROOTFOLDER);
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, createdFileName, GetFileTextString());

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(1));
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
                throw;
            }
        }

        public async void CaseTest2()
        {
            await DoWorkAsync(CaseTest2Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(2), false);
        }
        private async Task CaseTest2Task()
        {
            try
            {
                //Create file with sigle string
                //Read file contents and show them in a notification
                var createdFileName = CreateFilePath("CaseTest02_FileCreated.txt");

                await Storage.EnsureFolderExistsAsync(StorageLocation.ExternalPublic, ROOTFOLDER);
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, createdFileName, "Case Test 2 string");
                var str = await Storage.TryReadTextFileAsync(StorageLocation.ExternalPublic, createdFileName);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(2));
                await NotificationManager.PublishInfoNotificationAsync(str, NotificationModeEnum.MessageBox);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest3()
        {
            await DoWorkAsync(CaseTest3Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(3), false);
        }
        private async Task CaseTest3Task()
        {
            try
            {
                //Check if file from case test 2 exists
                var ct2FileName = CreateFilePath("CaseTest02_FileCreated.txt");
                var fileExists = await Storage.FileExistsAsync(StorageLocation.ExternalPublic, ct2FileName);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(3));
                await NotificationManager.PublishInfoNotificationAsync("'{0}' {1}!".SafeFormatTemplate(ct2FileName, fileExists ? "found" : "not found"), NotificationModeEnum.MessageBox);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest4()
        {
            await DoWorkAsync(CaseTest4Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(4), false);
        }
        private async Task CaseTest4Task()
        {
            try
            {
                //Check if file from case test 2 exists
                var ct2FileName = CreateFilePath("CaseTest02_FileCreated.txt");
                await Storage.DeleteFileAsync(StorageLocation.ExternalPublic, ct2FileName);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(4));
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest5()
        {
            await DoWorkAsync(CaseTest5Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(5), false);
        }
        private async Task CaseTest5Task()
        {
            try
            {
                //Check creation time of file from case test 2 
                var ct2FileName = CreateFilePath("CaseTest02_FileCreated.txt");
                var dt = await Storage.GetFileCreationTimeAsync(StorageLocation.ExternalPublic, ct2FileName);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(5));
                await NotificationManager.PublishInfoNotificationAsync("Creation time: " + dt.ToString(), NotificationModeEnum.MessageBox);

            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest6()
        {
            await DoWorkAsync(CaseTest6Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(6), false);
        }
        private async Task CaseTest6Task()
        {
            try
            {
                //Get and count all files in storage root folder
                var fileList = await Storage.GetFilesInAsync(StorageLocation.ExternalPublic, true, ROOTFOLDER);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(5));
                await NotificationManager.PublishInfoNotificationAsync(fileList.SafeCount() > 0 ? "Found {0} file(s)".SafeFormatTemplate(fileList.Count()) : "No files found", NotificationModeEnum.MessageBox);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest7()
        {
            await DoWorkAsync(CaseTest7Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(7), false);
        }
        private async Task CaseTest7Task()
        {
            try
            {
                //Delete storage root folder
                await Storage.DeleteFolderAsync(StorageLocation.ExternalPublic, ROOTFOLDER);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(7));
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest8()
        {
            await DoWorkAsync(CaseTest8Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(8), false);
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
                        await sw.WriteAsync(GetFileTextString());
                        await sw.FlushAsync();
                        ms.Position = 0;
                        await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, originalFileName, ms);
                        ms.Position = 0;
                        using (var cs = await Storage.CompressStreamAsync(ms))
                        {
                            await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, compressedFileName, cs);
                            cs.Position = 0;
                            using (var ds = await Storage.DecompressStreamAsync(cs))
                            {
                                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, decompressedFileName, ds);
                            }
                        }
                    }
                }

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(8));
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest9()
        {
            await DoWorkAsync(CaseTest9Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(9), false);
        }
        private async Task CaseTest9Task()
        {
            try
            {
                //Create original file
                //Clone that same file
                var originalFileName = CreateFilePath("CaseTest09_OriginalFile.txt");
                var clonedFileName = CreateFilePath("CaseTest09_ClonedFile.txt");

                await CreateOriginalFileAsync(originalFileName);
                await Storage.CloneFileAsync(StorageLocation.ExternalPublic, originalFileName, StorageLocation.ExternalPublic, clonedFileName, true);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(9));
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest10()
        {
            await DoWorkAsync(CaseTest10Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(10), false);
        }
        private async Task CaseTest10Task()
        {
            try
            {
                //Create original file
                //Move that same file to different folder
                var originalFileName = CreateFilePath("CaseTest10_OriginalFile.txt");
                var movedFileName = CreateFilePath(Storage.PathCombine("MoveFolder", "CaseTest10_MovedFile.txt"));

                await CreateOriginalFileAsync(originalFileName);
                await Storage.TryMoveAsync(StorageLocation.ExternalPublic, originalFileName, StorageLocation.ExternalPublic, movedFileName, false);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(10));
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest11()
        {
            await DoWorkAsync(CaseTest11Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(11), false);
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

                await CreateOriginalFileAsync(originalFileName);
                await StorageEncryption.EncryptFileAsync(StorageLocation.ExternalPublic, originalFileName, encryptedFileName, PASSWORD);
                await StorageEncryption.DecryptFileAsync(StorageLocation.ExternalPublic, encryptedFileName, decryptedFileName, PASSWORD);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(11));
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest12()
        {
            await DoWorkAsync(CaseTest12Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(12), false);
        }
        private async Task CaseTest12Task()
        {
            //Create encrypted file
            //Decrypt file
            try
            {
                var encryptedFileName = CreateFilePath("CaseTest12_Encrypted.txt");
                var decryptedFileName = CreateFilePath("CaseTest12_Decrypted.txt");

                await StorageEncryption.WriteEncryptedFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, encryptedFileName, GetFileTextString(), PASSWORD);
                await StorageEncryption.DecryptFileAsync(StorageLocation.ExternalPublic, encryptedFileName, decryptedFileName, PASSWORD);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(12));
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest13()
        {
            await DoWorkAsync(CaseTest13Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(13), false);
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

                await StorageEncryption.WriteEncryptedFileAsync(StorageLocation.ExternalPublic, StorageMode.CreateOrAppend, encryptedFileName, GetFileTextString(), PASSWORD);
                await StorageEncryption.DecryptFileAsync(StorageLocation.ExternalPublic, encryptedFileName, originalDecryptedFileName, PASSWORD);
                await StorageEncryption.WriteEncryptedFileAsync(StorageLocation.ExternalPublic, StorageMode.CreateOrAppend, encryptedFileName, "TEXT ADDED WITH ENCRYPTION" + Environment.NewLine, PASSWORD);
                await StorageEncryption.DecryptFileAsync(StorageLocation.ExternalPublic, encryptedFileName, decryptedFileName, PASSWORD);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(13));
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest14()
        {
            await DoWorkAsync(CaseTest14Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(14), false);
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
                        await Logger.LogAsyncMethodExecutionTimeAsync(() => StorageEncryption.WriteEncryptedFileAsync(StorageLocation.ExternalPublic, StorageMode.CreateOrAppend, encryptedFileName, "Line " + i + Environment.NewLine, PASSWORD),
                                                                      "WriteEncryptedFile #" + i);
                    }
                }, "WriteEncryptedFile");

                await Logger.LogAsyncMethodExecutionTimeAsync(() => StorageEncryption.DecryptFileAsync(StorageLocation.ExternalPublic, encryptedFileName, decryptedFileName, PASSWORD), "DecryptFile");

                await Logger.LogAsyncMethodExecutionTimeAsync(async () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        await Logger.LogAsyncMethodExecutionTimeAsync(() => Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.CreateOrAppend, normalFileName, "Line " + i + Environment.NewLine),
                                                                      "WriteFile #" + i);
                    }
                }, "WriteFile");


                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(14));
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest15()
        {
            await DoWorkAsync(CaseTest15Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(15), false);
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

                await CreateOriginalFileAsync(originalFileName);

                await Storage.CloneFileAsync(StorageLocation.ExternalPublic, originalFileName, StorageLocation.ExternalPublic, originalClonedFileName, true);

                await StorageEncryption.WriteEncryptedFileAsync(StorageLocation.ExternalPublic, StorageMode.CreateOrAppend, originalFileName, "Encrypted text added to non encrypted file", PASSWORD);

                await StorageEncryption.DecryptFileAsync(StorageLocation.ExternalPublic, originalFileName, decryptedFileName, PASSWORD);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(15));
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }

        }

        public async void CaseTest16()
        {
            await DoWorkAsync(CaseTest16Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(16), false);
        }
        private async Task CaseTest16Task()
        {
            //Criar ficheiro encriptado com uma linha simples
            //Ler ficheiro encriptado e mostrar o texto desencriptado numa mensagem popup
            try
            {
                var encryptedFileName = CreateFilePath("CaseTest16_Encrypted.txt");

                await StorageEncryption.WriteEncryptedFileAsync(StorageLocation.ExternalPublic, StorageMode.CreateOrAppend, encryptedFileName, "Just some text", PASSWORD);
                var output = await StorageEncryption.TryReadTextEncryptedFileAsync(StorageLocation.ExternalPublic, encryptedFileName, PASSWORD);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(16));
                await NotificationManager.PublishInfoNotificationAsync(output, NotificationModeEnum.MessageBox);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest17()
        {
            await DoWorkAsync(CaseTest17Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(17), false);
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
                await Storage.CloneFileFromAppResourcesAsync("EncryptedFiles/encryptedOld.356194055156252.txt", StorageLocation.ExternalPublic, encryptedOldFileName);
                await StorageEncryption.DecryptFileAsync(StorageLocation.ExternalPublic, encryptedOldFileName, decryptedOldFileName, "356194055156252");

                //Decrypt 'new encrypted' file
                await Storage.CloneFileFromAppResourcesAsync("EncryptedFiles/encryptedNew.password.txt", StorageLocation.ExternalPublic, encryptedNewFileName);
                await StorageEncryption.DecryptFileAsync(StorageLocation.ExternalPublic, encryptedNewFileName, decryptedNewFileName, "password");


                //Write to 'old encrypted' file and decrypt it
                await Storage.CloneFileFromAppResourcesAsync("EncryptedFiles/encryptedOld.356194055156252.txt", StorageLocation.ExternalPublic, encryptedOldModifiedFileName);
                await StorageEncryption.WriteEncryptedFileAsync(StorageLocation.ExternalPublic, StorageMode.CreateOrAppend, encryptedOldModifiedFileName, "added text to old encrypted file", "356194055156252");
                await StorageEncryption.DecryptFileAsync(StorageLocation.ExternalPublic, encryptedOldModifiedFileName, decryptedOldModifiedFileName, "356194055156252");

                //Write to 'new encrypted' file and decrypt it
                await Storage.CloneFileFromAppResourcesAsync("EncryptedFiles/encryptedNew.password.txt", StorageLocation.ExternalPublic, encryptedNewModifiedFileName);
                await StorageEncryption.WriteEncryptedFileAsync(StorageLocation.ExternalPublic, StorageMode.CreateOrAppend, encryptedNewModifiedFileName, "added text to old encrypted file", "password");
                await StorageEncryption.DecryptFileAsync(StorageLocation.ExternalPublic, encryptedNewModifiedFileName, decryptedNewModifiedFileName, "password");


                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(17));
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest18()
        {
            await DoWorkAsync(CaseTest18Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(18), false);
        }
        private async Task CaseTest18Task()
        {
            try
            {
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder1", "File1.txt")), "Contents of file 1 in folder 1");
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder1", "File2.txt")), "Contents of file 2 in folder 1");
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder2", "File1.txt")), "Contents of file 1 in folder 2");
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder2", "File2.txt")), "Contents of file 2 in folder 2");
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder3", "File1.txt")), "Contents of file 1 in folder 3");

                //Get and count all files in storage root folder
                var fileList = await Storage.GetFilesInAsync(StorageLocation.ExternalPublic, true, ROOTFOLDER, "txt");

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(18));
                await NotificationManager.PublishInfoNotificationAsync(fileList.SafeCount() > 0 ? "Found {0} file(s)".SafeFormatTemplate(fileList.Count()) : "No files found", NotificationModeEnum.MessageBox);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        public async void CaseTest19()
        {
            await DoWorkAsync(CaseTest19Task, TextSource.GetText(TextResourcesKeys.MessageTemplate_ProcessingCaseTest).SafeFormatTemplate(19), false);
        }
        private async Task CaseTest19Task()
        {
            try
            {
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder1", "File1.txt")), "Contents of file 1 in folder 1");
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder1", "File2.txt")), "Contents of file 2 in folder 1");
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder2", "File1.txt")), "Contents of file 1 in folder 2");
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder2", "File2.txt")), "Contents of file 2 in folder 2");
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder3", "File1.txt")), "Contents of file 1 in folder 3");

                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder1", "File1.m2u")), "Contents of file 1 in folder 1");
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder1", "File2.m2u")), "Contents of file 2 in folder 1");
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder2", "File1.m2u")), "Contents of file 1 in folder 2");
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder2", "File2.m2u")), "Contents of file 2 in folder 2");
                await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, CreateFilePath(Path.Combine("Folder3", "File1.m2u")), "Contents of file 1 in folder 3");

                //Get and count all files in storage root folder
                var fileList = await Storage.GetFilesInAsync(StorageLocation.ExternalPublic, true, ROOTFOLDER);

                await NotificationManager.PublishInfoNotificationAsync(TextSource.GetText(TextResourcesKeys.MessageTemplate_CaseTestCompleted).SafeFormatTemplate(19));
                await NotificationManager.PublishInfoNotificationAsync(fileList.SafeCount() > 0 ? "Found {0} file(s)".SafeFormatTemplate(fileList.Count()) : "No files found", NotificationModeEnum.MessageBox);
            }
            catch (AggregateException ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.InnerException.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
            catch (Exception ex)
            {
                await NotificationManager.PublishErrorNotificationAsync(ex.ToLongString(), NotificationModeEnum.MessageBox);
                Debugger.Break();
            }
        }

        private async Task CreateOriginalFileAsync(string originalFileName)
        {
            await Storage.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.Create, originalFileName, GetFileTextString());
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
