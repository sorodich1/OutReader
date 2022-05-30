using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinSCP;

namespace OutReader.Model.LiftWater
{
    public class WinSCPHelper
    {
        private const string Url = "192.168.22.150";
        private const string Path = "/mnt/ufs/media/sda1/";
        private const string Username = "root";
        private const string Password = "root";
        private static readonly string LocalPath = System.IO.Path.GetTempPath() + "owen";
        private static readonly SessionOptions sessionOptions = new SessionOptions
        {
            Protocol = Protocol.Scp,
            HostName = Url,
            UserName = Username,
            Password = Password,
            PortNumber = 22,
            SshHostKeyFingerprint = "ssh-rsa 1040 80:ba:1a:72:98:a2:50:d7:46:55:3b:4d:33:86:b5:28"
        };

        public static string Download()
        {
            
            //try
            //{
                using (Session session = new Session())
                {
                    session.SessionLogPath = "";
                    session.Open(sessionOptions);
                    RemoteDirectoryInfo directory = session.ListDirectory(Path);
                    var strDate = DateTime.Now.ToString("yyyyMMdd");
                    
                    var loadFiles = new List<string>();
                    //while (true)
                    //{
                       
                            foreach (RemoteFileInfo fileInfo in directory.Files)
                            {
                                //try
                                //{
                                var name = fileInfo.Name;
                                if (!loadFiles.Contains(name) && name.Contains(strDate) && (name.Contains("p" + strDate) || name.Contains("p1") || name.Contains("p2")))
                                    {
                                        TransferOptions transferOptions = new TransferOptions
                                        {
                                            TransferMode = TransferMode.Binary,
                                            FilePermissions = null,
                                            PreserveTimestamp = false,
                                            ResumeSupport = { State = TransferResumeSupportState.Off }
                                        };
                                        var transferResult = session.GetFiles(name, LocalPath, false, transferOptions);
                                        //transferResult.Check();
                                        loadFiles.Add(name);
                                    }
                                //}
                                //catch (Exception ex)
                                //{
                                //    Thread.Sleep(1000);
                                //}
                            }
                            //break;
                        
                    //}
                    
                    
                }
            //}
            //catch (Exception ex)
            //{
            //}
            var res = "";
            foreach (var path in Directory.GetFiles(LocalPath))
            {
                res += File.ReadAllText(path);
            }
            Directory.Delete(LocalPath, true);
            return res;
        }


        //private static bool UploadFile()
        //{
        //    bool success = false;
        //    string sourcefilepath = "Input File Path";
        //    try
        //    {
                
        //        //string filename = Path.GetFileName(sourcefilepath);
        //        //string ftpfullpath = ftpurl + "/" + filename;

        //        using (Session session = new Session())
        //        {
        //            // Connect
        //            session.Open(sessionOptions);

        //            // Upload files
        //            TransferOptions transferOptions = new TransferOptions();
        //            transferOptions.TransferMode = TransferMode.Binary;

        //            TransferOperationResult transferResult = session.PutFiles(sourcefilepath, Path, false, transferOptions);

        //            // Throw on any error
        //            transferResult.Check();

        //            // Print results
        //            foreach (TransferEventArgs transfer in transferResult.Transfers)
        //            {
        //                success = true;
        //            }
        //        }

        //        // Delete the file after uploading
        //        if (File.Exists(sourcefilepath))
        //        {
        //            File.Delete(sourcefilepath);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return success;
        //}     
    }
}
