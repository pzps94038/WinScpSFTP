using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace WinScpSFTP
{
    public enum ProtocolType
    {
        Sftp = 0,
        Scp = 1,
        Ftp = 2,
        Webdav = 3,
        S3 = 4
    }
    public class SftpClient
    {
        private SessionOptions _sessionOptions { get; set; }
        // 一般ftp
        public SftpClient(ProtocolType type, string hostName, int port, string userName, string password)
        {
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = (Protocol)type,
                HostName = hostName,
                PortNumber = port,
                UserName = userName,
                Password = password,
            };
            this._sessionOptions = sessionOptions;
        }
        // sftp
        public SftpClient(ProtocolType type, string hostName, int port, string userName, string password, string sshHostKeyFingerprint)
        {
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = (Protocol)type,
                HostName = hostName,
                PortNumber = port,
                UserName = userName,
                Password = password,
                // 指紋需要測試連接過一次才會知道，你的KeyFingerprin是甚麼
                // ex: ssh-rsa 3072 vQgOPew8expPDGHX1FwunI12OMAHOH4i7xp8WPPRCmM
                SshHostKeyFingerprint = sshHostKeyFingerprint,
            };
            this._sessionOptions = sessionOptions;
        }

        public void Upload(string localPath, string remotePath)
        {
            using (Session session = new Session())
            {
                session.Open(this._sessionOptions);
                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Automatic;
                // localPath ex: c:\Users\asus\Desktop\sftp\*.txt"
                // remotePath ex: /C:/Users/sftpuser/Temp/
                TransferOperationResult transferResult = session.PutFiles(localPath, remotePath, false, transferOptions);
                transferResult.Check();
            }
        }

        public void Downloads(string remotePath, string localPath)
        {
            using (Session session = new Session())
            {
                session.Open(this._sessionOptions);
                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Automatic;
                // localPath ex: c:\Users\asus\Desktop\Temp\"
                // remotePath ex: /C:/Users/sftpuser/Temp/*.txt
                if (!Directory.Exists(localPath))
                    Directory.CreateDirectory(localPath);

                TransferOperationResult transferResult = session.GetFiles(remotePath, localPath, false, transferOptions);
                transferResult.Check();
            }
        }

        public void Move(string sourcePath, string targetPath)
        {
            using (Session session = new Session())
            {
                session.Open(this._sessionOptions);
                // sourcePath ex: c:\Users\asus\Desktop\Temp\"
                // remotePath ex: /C:/Users/sftpuser/Temp/*.txt
                session.MoveFile(sourcePath, targetPath);
            }
        }

        public void RemoveFile(string path)
        {
            using (Session session = new Session())
            {
                session.Open(this._sessionOptions);
                session.RemoveFile(path);
            }
        }

        public void RemoveFiles(string path)
        {
            using (Session session = new Session())
            {
                session.Open(this._sessionOptions);
                session.RemoveFiles(path);
            }
        }

        public void CreateDirectory(string targetDicPath)
        {
            using (Session session = new Session())
            {
                session.Open(this._sessionOptions);
                if (!session.FileExists(targetDicPath))
                    session.CreateDirectory(targetDicPath);
            }
        }
    }
}
