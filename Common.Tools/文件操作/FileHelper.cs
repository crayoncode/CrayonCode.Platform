using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Common.Tools
{
    /// <summary>
    /// 文件操作类
    /// </summary>
    public class FileHelper
    {

        /// <summary>
        /// 将文件流存入内存中返回
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static MemoryStream ReadFileMemoryStream(string filePath)
        {
            MemoryStream ms = null;
            byte[] bt = ReadFileByteArray(filePath);
            ms = new MemoryStream(bt, 0, bt.Length, true);
            return ms;
        }

        public static byte[] ReadFileByteArray(string filePath)
        {
            FileStream pFileStream = null;
            byte[] pReadByte = new byte[0];
            try
            {
                pFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(pFileStream);
                r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
                pReadByte = r.ReadBytes((int)r.BaseStream.Length);
                return pReadByte;
            }
            catch
            {
                return pReadByte;
            }

            finally
            {
                if (pFileStream != null)
                    pFileStream.Close();
            }
        }

        private static bool WriteFile(byte[] pReadByte, string fileName)
        {
            FileStream pFileStream = null;
            try
            {
                pFileStream = new FileStream(fileName, FileMode.OpenOrCreate);
                pFileStream.Write(pReadByte, 0, pReadByte.Length);
            }

            catch
            {
                return false;
            }

            finally
            {
                if (pFileStream != null)
                    pFileStream.Close();
            }
            return true;
        }


        /// <summary>
        /// 读取TXT里的内容 
        /// </summary>
        /// <param name="sTxtPath">文本的所在路径</param>
        /// <param name="sEncodingString">Encoding编码</param>
        /// <returns>String[]={第一行内容，第二行内容，……} </returns>
        public static string[] ReaderTxtContent(string sTxtPath, string sEncodingString)
        {
            try
            {
                return File.ReadAllLines(sTxtPath, Encoding.GetEncoding(sEncodingString));
            }
            catch { return null; }
        }

        /// <summary>
        /// 将数组里的内容sTxtContent写入到指定的TXT文本当中去
        /// </summary>
        /// <param name="sTxtContent">文本数组；String[]={将会添加到TXT第一行内容，将会添加到TXT第二行内容，……}</param>
        /// <param name="sTxtPath">TXT文本的所在路径</param>
        /// <param name="sEncodingString">Encoding编码</param>
        /// <param name="bAppendText">是否追加文本（true 追加 false 不追加）</param>
        /// <param name="bCreatePath">如果路径不存在，是否自动创建路径（true创建 false 不创建）</param>
        /// <returns></returns>
        public static bool WriteTxtContent(String[] sTxtContent, String sTxtPath, String sEncodingString, bool bAppendText, bool bCreatePath)//写TXT文本
        {
            /*  Implementation Notes：*/
            bool bFlag = false;
            /* 1、根据bCreatePath和路径是否存在判断是否来创建路径，同时为是，则创建路径
             */
            if (File.Exists(sTxtPath) == false && bCreatePath)
            {
                String sDirectoryPath = sTxtPath.Substring(0, sTxtPath.LastIndexOf("\\"));
                if (!Directory.Exists(sDirectoryPath))
                {
                    Directory.CreateDirectory(sDirectoryPath);
                }
                File.Create(sTxtPath).Close();
            }
            if ((File.Exists(sTxtPath)) && (System.IO.Path.GetExtension(sTxtPath) == ".txt" || System.IO.Path.GetExtension(sTxtPath) == ".TXT"))
            {
                /* 2、根据bAppendText来判断是否追加写入文本，为是则追加写入，否则覆盖写入
                 */
                if (bAppendText != true)
                {
                    File.WriteAllLines(sTxtPath, sTxtContent, Encoding.GetEncoding(sEncodingString));
                    bFlag = true;
                }
                else
                {
                    for (int iNumOfTxt = 0; iNumOfTxt < sTxtContent.Length; iNumOfTxt++)
                    {
                        StreamWriter sw = File.AppendText(sTxtPath);
                        if (iNumOfTxt == 0)
                            sw.WriteLine("");
                        sw.WriteLine(sTxtContent[iNumOfTxt]);
                        sw.Close();
                    }
                    bFlag = true;
                }
            }
            return bFlag;
        }

        /// <summary>
        /// 采用递归算法取指定目录下所有文件名称
        /// </summary>
        /// <param name="sDirectoryPath">指定目录地址，如：D:\NGNCN</param>
        /// <returns></returns>
        public static List<string> GetFileNamesFromADirectory(String sDirectoryPath)//采用递归算法取指定目录下指定后缀的所有文件名称
        {
            return GetFileNamesFromADirectory(sDirectoryPath, "");
        }

        /// <summary>
        /// 采用递归算法取指定目录下指定后缀的所有文件名称
        /// </summary>
        /// <param name="sDirectoryPath">指定目录地址，如：D:\NGNCN</param>
        /// <param name="sEndWithString">指定后缀</param>
        /// <returns></returns>
        public static List<string> GetFileNamesFromADirectory(String sDirectoryPath, String sEndWithString)//采用递归算法取指定目录下指定后缀的所有文件名称
        {
            //将字符串路径组合成长度为1的数组
            List<string> fileNames = new List<string>();
            Int32 iFileNamesIndex = -1;
            String[] saDirectorys = new String[] { sDirectoryPath };
            //调用GetFileNamesFromADirectory(saDirectorys,sEndWithString)取文件名称
            GetFileNamesFromADirectory(saDirectorys, sEndWithString, fileNames, ref iFileNamesIndex);
            //返回文件名称数组
            return fileNames;
        }

        /// <summary>
        /// 采用递归算法取指定目录下指定后缀的所有文件名称
        /// </summary>
        /// <param name="saAllPath">所有目录(文件)路径，如：saAllPath[0] = D:\NGNCN,saAllPath[1] = D:\NGNCN.jpg</param>
        /// <param name="sEndWithString">指定后缀</param>
        /// <param name="fileNames"></param>
        /// <param name="iFileNamesIndex"></param>
        private static void GetFileNamesFromADirectory(String[] saAllPath, String sEndWithString, List<String> fileNames, ref Int32 iFileNamesIndex)
        {
            /*遍历所有路径，如果所遍历到的路径为指定后缀的文件则把它添加到全局文件数组.
              否则判断是否是文件夹路径，是则取其下所有文件夹及文件路径递归调用本函数来找到所需文件*/
            foreach (String sPath in saAllPath)
            {
                if ((sEndWithString == string.Empty || sPath.EndsWith(sEndWithString.ToLower()) || sPath.EndsWith(sEndWithString.ToUpper())) && File.Exists(sPath))
                {
                    fileNames.Add(sPath);
                }
                else if (System.IO.Directory.Exists(sPath))
                {
                    String[] saDirectories = System.IO.Directory.GetDirectories(sPath);
                    String[] saFiles = System.IO.Directory.GetFiles(sPath);
                    String[] saAllPathBuf = new String[(saDirectories.Length + saFiles.Length)];
                    for (Int32 i = 0; i < saFiles.Length; i++)
                        saAllPathBuf[i] = saFiles[i];
                    for (Int32 j = saFiles.Length; j < saAllPathBuf.Length; j++)
                        saAllPathBuf[j] = saDirectories[j - saFiles.Length];
                    GetFileNamesFromADirectory(saAllPathBuf, sEndWithString, fileNames, ref iFileNamesIndex);
                }
            }
        }

        /// <summary>
        /// 修改文件名
        /// </summary>
        /// <param name="sOldFilePath"></param>
        /// <param name="sNewFilePath"></param>
        /// <returns></returns>
        public static bool AlterFileName(String sOldFilePath, String sNewFilePath)
        {
            bool bReturn = false;
            if (File.Exists(sNewFilePath) == false && File.Exists(sOldFilePath) == true)
            {
                File.Copy(sOldFilePath, sNewFilePath);
                File.Delete(sOldFilePath);
                bReturn = true;
            }
            else
                bReturn = false;
            return bReturn;
        }

        /// <summary>
        /// 修改文件路径名称
        /// </summary>
        /// <param name="sDirectoryPath">待修改文件夹路径</param>
        /// <param name="sNewName">新文件夹名称</param>
        /// <returns></returns>
        public static bool AlterDirectoryName(String sDirectoryPath, String sNewName)
        {
            bool bReturn = false;
            DirectoryInfo dInfo = new DirectoryInfo(sDirectoryPath);
            sNewName = sDirectoryPath.Substring(0, sDirectoryPath.LastIndexOf('\\')) + "\\" + sNewName;
            if (Directory.Exists(sNewName) == false)
            {
                Directory.Move(sDirectoryPath, sNewName);
            }
            else
                bReturn = false;
            return bReturn;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="sFileDir"></param>
        public static void OpenFile(String sFileDir)
        {
            if (File.Exists(sFileDir))
                System.Diagnostics.Process.Start(sFileDir);
        }

        /// <summary>
        /// 定位到文件所在文件夹，并将该文件标为选中状态
        /// </summary>
        /// <param name="sFileDir"></param>
        public static void LocatingFile(String sFileDir)
        {
            if (File.Exists(sFileDir))
                System.Diagnostics.Process.Start("explorer.exe", @"/select," + sFileDir);
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sOrgPath">原始文件路径</param>
        /// <param name="sNewPath">新路径</param>
        /// <param name="bCopyOrCut">fale：剪切,true:复制</param>
        /// <returns></returns>
        public static bool MoveFile(String sOrgPath, String sNewPath, bool bCopyOrCut)
        {
            bool bReturn = false;
            if (File.Exists(sOrgPath))
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(sOrgPath);
                if (System.IO.Directory.Exists(sNewPath.Substring(0, sNewPath.LastIndexOf('\\'))) == false)
                    System.IO.Directory.CreateDirectory(sNewPath.Substring(0, sNewPath.LastIndexOf('\\')));
                if (bCopyOrCut)
                {
                    fileInfo.CopyTo(sNewPath, true);
                    bReturn = true;
                }
                else
                {
                    fileInfo.MoveTo(sNewPath);
                    bReturn = true;
                }

            }
            else if (Directory.Exists(sOrgPath))
            {
                DirectoryInfo pathInfo = new DirectoryInfo(sOrgPath);

                pathInfo.MoveTo(sNewPath);
                bReturn = true;
            }
            return bReturn;
        }
        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="srcdir">源路径</param>
        /// <param name="desdir">目标路径</param>
        /// <param name="bCover">如果文件已存在是否覆盖</param>
        public static void CopyDirectory(string srcdir, string desdir, bool bCover)
        {
            if (Directory.Exists(srcdir))
            {
                string[] filenames = Directory.GetFileSystemEntries(srcdir);

                foreach (string file in filenames)// 遍历所有的文件和目录
                {
                    string currentdir = desdir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                    if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                    {

                        if (!Directory.Exists(currentdir))
                        {
                            Directory.CreateDirectory(currentdir);
                        }

                        CopyDirectory(file, currentdir, bCover);
                    }
                    else // 否则直接copy文件
                    {
                        if (!Directory.Exists(desdir))
                        {
                            Directory.CreateDirectory(desdir);
                        }
                        if (File.Exists(currentdir))
                        {
                            if (bCover)
                            {
                                FileInfo tempfile = new FileInfo(currentdir);
                                tempfile.Delete();
                                File.Copy(file, currentdir);
                            }
                        }
                        else
                            File.Copy(file, currentdir);
                    }
                }//foreach
            }
        }

        public static bool DeleteDirectory(string directoryPath)
        {
            bool bReturn = false;
            try
            {
                if (System.IO.Directory.Exists(directoryPath))
                {
                    System.IO.DirectoryInfo pathInfo = new DirectoryInfo(directoryPath);
                    pathInfo.Delete(true);
                    bReturn = true;
                }
            }
            catch { }
            return bReturn;
        }

        public static void SavaAs(string sourceFile)
        {
            string targetPath = string.Empty;
            string fileName = sourceFile.Substring(sourceFile.LastIndexOf("/") + 1);

            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            // 设置对话框的说明信息
            folderDialog.Description = "请选择文件保存目录";
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                targetPath = folderDialog.SelectedPath;
                targetPath = System.IO.Path.Combine(targetPath, fileName);
                if (File.Exists(sourceFile) == false)
                {
                    MessageBox.Show("被复制的源文件“" + sourceFile + "”不存在。");
                    return;
                }
                if (File.Exists(targetPath) == true)
                {
                    if (MessageBox.Show("该目录下已经存在名为“" + fileName + "”的文件。是否覆盖保存？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        FileInfo file = new FileInfo(targetPath);
                        file.Delete();
                    }
                    else
                    {
                        return;
                    }
                }

                File.Copy(sourceFile, targetPath);
                MessageBox.Show("保存成功。");
            }
            else
            {
                return;
            }
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr _lopen(string lpPathName, int iReadWrite);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        private const int OF_READWRITE = 2;
        private const int OF_SHARE_DENY_NONE = 0x40;
        private static IntPtr HFILE_ERROR = new IntPtr(-1);

        /// <summary>
        ///  文件是否已被打开
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsFileOpened(string filePath)
        {
            IntPtr vHandle = _lopen(filePath, OF_READWRITE | OF_SHARE_DENY_NONE);
            if (vHandle == HFILE_ERROR)
            {
                return true;
            }

            CloseHandle(vHandle);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="containExt"></param>
        /// <returns></returns>
        public static string PickFileNameFromDirectory(string dir, bool containExt = true)
        {
            Debug.Assert(!string.IsNullOrEmpty(dir));

            try
            {
                string fileName = dir.Substring(dir.LastIndexOf("\\") + 1);
                if (!containExt)
                    fileName = fileName.Substring(0, fileName.Length - fileName.LastIndexOf("."));
                return fileName;
            }
            catch (System.Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
