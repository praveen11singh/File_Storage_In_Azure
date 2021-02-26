using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;
using System;

namespace File_Storage_In_Azure
{
    class Program
    {
        static void Main(string[] args)
        {

            //code for retrieve the connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("fileShareConnectionString"));
            
            CloudFileClient cloudFileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.
            CloudFileShare fileShare = cloudFileClient.GetShareReference("hgdhdhdgh");

            // Ensure that the share exists.
            if (fileShare.Exists())
            {
                // Get a reference to the root directory for the share.
                CloudFileDirectory fileDirectory = fileShare.GetRootDirectoryReference();

                // Get a reference to the directory we created previously.
                CloudFileDirectory customDirectory = fileDirectory.GetDirectoryReference("CustomDocs");

                // Ensure that the directory exists.
                if (customDirectory.Exists())
                {
                    // Get a reference to the file we created previously.
                    CloudFile fileInfo = customDirectory.GetFileReference("Log1.txt");

                    // Ensure that the file exists.
                    if (fileInfo.Exists())
                    {
                        // Write the contents of the file to the console window.
                        Console.WriteLine(fileInfo.DownloadTextAsync().Result);
                    }
                }

                NewFileCreate();
            }
        }

        public static void NewFileCreate()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("fileShareConnectionString"));
            
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
            CloudFileShare share = fileClient.GetShareReference("hgdhdhdgh");
            // Ensure that the share exists.
            if (share.Exists())
            {
                string policyName = "sampleSharePolicy" + DateTime.UtcNow.Ticks;
               
                SharedAccessFilePolicy sharedPolicy = new SharedAccessFilePolicy()
                {
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                    Permissions = SharedAccessFilePermissions.Read | SharedAccessFilePermissions.Write
                };

                FileSharePermissions permissions = share.GetPermissions();
                
                permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
                share.SetPermissions(permissions);
                
                CloudFileDirectory rootDir = share.GetRootDirectoryReference();
                CloudFileDirectory sampleDir = rootDir.GetDirectoryReference("CustomDocs");

                CloudFile file = sampleDir.GetFileReference("Log1.txt");
                string sasToken = file.GetSharedAccessSignature(null, policyName);
                Uri fileSasUri = new Uri(file.StorageUri.PrimaryUri.ToString() + sasToken);
                
                // Create a new CloudFile object from the SAS, and write some text to the file.
                CloudFile fileSas = new CloudFile(fileSasUri);
                fileSas.UploadText("This file created by the Console App at Runtime");
                Console.WriteLine(fileSas.DownloadText());
            }
        }
    }
}
