using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace tlsTestN {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }
        #region  api 声明
        
         [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //progress =downloadedBytes/totalBytes*100
    public delegate void ProgressCallbackDelegate(double progress, long downloadedBytes, long totalBytes);

         [DllImport("tlsHelper.dll", CallingConvention = CallingConvention.Cdecl)]
         private static extern void SetProgressCallback(ProgressCallbackDelegate callback);
        //[DllImport("tlsHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern IntPtr DownloadFile([MarshalAs(UnmanagedType.LPStr)] string url, [MarshalAs(UnmanagedType.LPStr)] string filePath);
         [DllImport("tlsHelper.dll")]
         public static extern void DownloadFile(byte[] url, byte[] filePath, out bool success, out IntPtr message);

         [DllImport("tlsHelper.dll")]
         public static extern void FreeString(IntPtr str);
      //#if WIN64
//     [DllImport("tlsHelper64.dll", CallingConvention = CallingConvention.Cdecl)]
//#else
        [DllImport("tlsHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        //#endif



        #endregion
        private static extern IntPtr HttpGet(string url);

        private void btnTestDll_Click(object sender, EventArgs e) {
            string url = textBox2.Text.Trim();// "https://v3.lansaguo.com/app/uservip/vip_sku";
            var resultPtr = HttpGet(url);
            string tmpStr = Marshal.PtrToStringAnsi(resultPtr);
            request2Show(tmpStr);
        }

        void request2Show(string tmpStr) {
            try {

                var tmpStr1 = U2CnCode(tmpStr);
                textBox1.Text = tmpStr1;
                return;
                var jsonObj = JsonConvert.DeserializeObject<CommonProblemInterface>(tmpStr1);
                var lst = jsonObj.data.problem;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < lst.Count; i++) {
                    var item = lst[i];
                    sb.Append(string.Format("{0}-->{1} {2}\r\n", i, item.title, item.url));

                }
                richTextBox1.Text = sb.ToString();

            } catch (Exception ex) {

                MessageBox.Show(ex.Message);
            }
        }
        string U2CnCode(string str) {
            Match m;
            Regex r = new Regex("(?<code>\\\\u[a-z0-9]{4})", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch()) {
                string strValue = m.Result("${code}");   //代码
                int CharNum = Int32.Parse(strValue.Substring(2, 4), System.Globalization.NumberStyles.HexNumber);
                string ch = string.Format("{0}", (char)CharNum);
                str = str.Replace(strValue, ch);
            }
            return str;
        }

        private void Form1_Load(object sender, EventArgs e) {
           
            var dPath=Application.StartupPath+"\\tlsHelper.dll";
            var fPath = Application.StartupPath + "\\dll\\tlsHelper32.dll";
             if(is64BitSys() )
                 fPath = Application.StartupPath + "\\dll\\tlsHelper64.dll";
            if(!File.Exists(dPath)){
                File.Copy(fPath, dPath);
            }

        }
        bool is64BitSys() {
            int ptrSize = IntPtr.Size;
            //this.Text = "ptrSize: " + ptrSize;
            //if (ptrSize == 4) {
            //    Console.WriteLine("当前系统是32位的。");
            //} else if (ptrSize == 8) {
            //    Console.WriteLine("当前系统是64位的。");
            //} else {
            //    Console.WriteLine("无法识别的系统位数。");
            //}
            return ptrSize == 8;
        }


        #region //////////////// ////////////////////// download //////////////////////////
        private void button1_Click(object sender, EventArgs e) {
            progressBar1.Maximum = 100;
            var fPath = Application.StartupPath + "\\download\\";
            if (!Directory.Exists(fPath))
                Directory.CreateDirectory(fPath);
            var url = txtFile.Text;
            var fName=Path.GetFileName(url);
            string msg = "";
           var ret= StartDownload(url, fPath + fName, (progress, downloadedBytes, totalBytes) => {
                this.Invoke(new Action(()=>setProgress(progress, downloadedBytes, totalBytes)));
            },out msg);
           if (ret) {//下载成功
               //检验md5。。。。
               MessageBox.Show("下载成功   " + (fPath + fName));
           } else {
               System.Diagnostics.Process.Start(fPath);
               MessageBox.Show("注意：测试版不支持改url,下载失败   " + msg);
           }
            
        }
        //回调
        void setProgress(double progress,long downloadedBytes,long totalBytes) {
            
            if (downloadedBytes >= totalBytes) {
                progressBar1.Value = 100;
                System.Diagnostics.Debug.Print("下载完成!");
            } else {
               var iValue= Convert.ToInt32(progress);
               progressBar1.Value = iValue;
            }
        }

        private ProgressCallbackDelegate progressCallback;

        public bool StartDownload(string url, string filePath, Action<double, long, long> progressHandler,out string msg) {
           //#处理中文
           var fBytes= Encoding.UTF8.GetBytes(filePath);
           var urlBytes = Encoding.UTF8.GetBytes(url);
            msg="";
            progressCallback = (progress, downloadedBytes, totalBytes) => progressHandler(progress, downloadedBytes, totalBytes);
            SetProgressCallback(progressCallback);
            bool isOk; IntPtr msgPtr;
           DownloadFile(urlBytes,fBytes, out isOk, out msgPtr);
              msg = Marshal.PtrToStringAnsi(msgPtr);
            if (isOk)   // Resume download completed successfully 表示断点下载 
                System.Diagnostics.Debug.Print("下载成功   路径》" +  filePath);
            else
                System.Diagnostics.Debug.Print("下载失败 " + msg);
             return isOk;
        }
        #endregion end downLoad

    }






 

 
}
