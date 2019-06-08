using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HeBianGu.ImagePlayer.ImagePlayerControl.Provider
{
    class ShareHelper
    {
        public static bool connectState(string path, string userName, string passWord)
        {
            bool Flag = false;
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"net use " + path + " /User:" + userName + " " + passWord + " /PERSISTENT:YES";
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg))
                {
                    Flag = true;
                }
                else
                {
                    throw new Exception(errormsg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }


        public void UpLoadFile(string fileNamePath, string urlPath, string User, string Pwd)
        {
            string newFileName = fileNamePath.Substring(fileNamePath.LastIndexOf(@"\") + 1);//取文件名称


            //Debug.WriteLine(newFileName);


            if (urlPath.EndsWith(@"\") == false) urlPath = urlPath + @"\";

            urlPath = urlPath + newFileName;

            WebClient myWebClient = new WebClient();

            NetworkCredential cread = new NetworkCredential();

            myWebClient.Credentials = cread;
            FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);

            try
            {
                byte[] postArray = r.ReadBytes((int)fs.Length);
                Stream postStream = myWebClient.OpenWrite(urlPath);
                // postStream.m
                if (postStream.CanWrite)
                {
                    postStream.Write(postArray, 0, postArray.Length);

                    Debug.WriteLine("文件上传成功");

                }
                else
                {

                    Debug.WriteLine("文件上传错误");

                }

                postStream.Close();
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);

            }

        }

        public void DownLoadFile(string URL, string DIR)
        {
            string FileName = URL.Substring(URL.LastIndexOf("\\") + 1);
            string PATH = DIR + FileName;
            try
            {
                WebRequest SC = WebRequest.Create(URL);
            }
            catch
            {
            }
            try
            {
                //client.DownloadFile(URL, PATH);
            }
            catch
            {
            }
        }
    }
}
