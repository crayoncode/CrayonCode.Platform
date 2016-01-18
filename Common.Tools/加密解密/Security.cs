using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;

namespace Common.Tools.Security
{
    /// <summary>
    /// CRC 效验
    /// 快速检测算法
    /// </summary>
    public class CRC32
    {
        /// <summary>
        /// 校验表
        /// </summary>
        protected ulong[] crc32Table;

        /// <summary>
        /// 构造：初始化效验表
        /// </summary>
        public CRC32()
        {
            const ulong ulPolynomial = 0xEDB88320;
            ulong dwCrc;
            crc32Table = new ulong[256];
            int i, j;
            for (i = 0; i < 256; i++)
            {
                dwCrc = (ulong)i;
                for (j = 8; j > 0; j--)
                {
                    if ((dwCrc & 1) == 1)
                        dwCrc = (dwCrc >> 1) ^ ulPolynomial;
                    else
                        dwCrc >>= 1;
                }
                crc32Table[i] = dwCrc;
            }
        }

        /// <summary>
        /// 字节数组效验
        /// </summary>
        /// <param name="buffer">ref 字节数组</param>
        /// <returns></returns>
        public ulong ByteCRC(ref byte[] buffer)
        {
            ulong ulCRC = 0xffffffff;
            ulong len;
            len = (ulong)buffer.Length;
            for (ulong buffptr = 0; buffptr < len; buffptr++)
            {
                ulong tabPtr = ulCRC & 0xFF;
                tabPtr = tabPtr ^ buffer[buffptr];
                ulCRC = ulCRC >> 8;
                ulCRC = ulCRC ^ crc32Table[tabPtr];
            }
            return ulCRC ^ 0xffffffff;
        }

        /// <summary>
        /// 字符串效验
        /// </summary>
        /// <param name="sInputString">字符串</param>
        /// <returns></returns>
        public ulong StringCRC(string sInputString)
        {
            byte[] buffer = System.Text.Encoding.Default.GetBytes(sInputString);
            return ByteCRC(ref buffer);
        }


        /// <summary>
        /// 文件效验
        /// </summary>
        /// <param name="sInputFilename">输入文件</param>
        /// <returns></returns>
        public ulong FileCRC(string sInputFilename)
        {
            FileStream inFile = new System.IO.FileStream(sInputFilename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            byte[] bInput = new byte[inFile.Length];
            inFile.Read(bInput, 0, bInput.Length);
            inFile.Close();

            return ByteCRC(ref bInput);
        }

    }

    /// <summary>
    /// MD5 无逆向编码
    /// 获取唯一特征串，可用于密码加密
    /// (无法还原)
    /// </summary>
    public class MD5
    {

        /// <summary>
        /// 获取字符串的特征串
        /// </summary>
        /// <param name="inputString">输入文本</param>
        /// <returns></returns>
        public string GetHashString(string inputString)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            string encoded = BitConverter.ToString(md5.ComputeHash(System.Text.Encoding.Default.GetBytes(inputString))).Replace("-", "");
            return encoded;
        }
        /// <summary>
        /// 返回16位字符串
        /// </summary>
        /// <param name="ConvertString"></param>
        /// <returns></returns>
        public string GetMd5Str(string ConvertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }
        /// <summary>
        /// 获取文件的特征串
        /// </summary>
        /// <param name="inputFilename">输入文件</param>
        /// <returns></returns>
        public string GetHashFile(string inputFilename)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            FileStream inFile = new System.IO.FileStream(inputFilename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            byte[] bInput = new byte[inFile.Length];
            inFile.Read(bInput, 0, bInput.Length);
            inFile.Close();

            string encoded = BitConverter.ToString(md5.ComputeHash(bInput)).Replace("-", "");
            return encoded;
        }

    }

    /// <summary>
    /// Base64 UUEncoded 编码
    /// 将二进制编码为ASCII文本，用于网络传输
    /// (可还原)
    /// </summary>
    public class Base64
    {
        /// <summary>
        /// 解码字符串
        /// </summary>
        /// <param name="sInputString">输入文本</param>
        /// <returns></returns>
        public static string DecryptString(string sInputString)
        {
            char[] sInput = sInputString.ToCharArray();
            try
            {
                byte[] bOutput = System.Convert.FromBase64String(sInputString);
                return System.Text.Encoding.Default.GetString(bOutput);
            }
            catch (System.ArgumentNullException)
            {
                //base 64 字符数组为null
                return "";
            }
            catch (System.FormatException)
            {
                //长度错误，无法整除4
                return "";
            }
        }

        /// <summary>
        /// 编码字符串
        /// </summary>
        /// <param name="sInputString">输入文本</param>
        /// <returns></returns>
        public static string EncryptString(string sInputString)
        {
            byte[] bInput = System.Text.Encoding.Default.GetBytes(sInputString);
            try
            {
                return System.Convert.ToBase64String(bInput, 0, bInput.Length);
            }
            catch (System.ArgumentNullException)
            {
                //二进制数组为NULL.
                return "";
            }
            catch (System.ArgumentOutOfRangeException)
            {
                //长度不够
                return "";
            }
        }

        /// <summary>
        /// 解码文件
        /// </summary>
        /// <param name="sInputFilename">输入文件</param>
        /// <param name="sOutputFilename">输出文件</param>
        public static void DecryptFile(string sInputFilename, string sOutputFilename)
        {
            System.IO.StreamReader inFile;
            char[] base64CharArray;

            try
            {
                inFile = new System.IO.StreamReader(sInputFilename, System.Text.Encoding.ASCII);
                base64CharArray = new char[inFile.BaseStream.Length];
                inFile.Read(base64CharArray, 0, (int)inFile.BaseStream.Length);
                inFile.Close();
            }
            catch
            {//(System.Exception exp) {
                return;
            }

            // 转换Base64 UUEncoded为二进制输出
            byte[] binaryData;
            try
            {
                binaryData =
                  System.Convert.FromBase64CharArray(base64CharArray,
                  0,
                  base64CharArray.Length);
            }
            catch (System.ArgumentNullException)
            {
                //base 64 字符数组为null
                return;
            }
            catch (System.FormatException)
            {
                //长度错误，无法整除4
                return;
            }

            // 写输出数据
            System.IO.FileStream outFile;
            try
            {
                outFile = new System.IO.FileStream(sOutputFilename, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                outFile.Write(binaryData, 0, binaryData.Length);
                outFile.Close();
            }
            catch
            {// (System.Exception exp) {
                //流错误
            }

        }

        /// <summary>
        /// 编码文件
        /// </summary>
        /// <param name="sInputFilename">输入文件</param>
        /// <param name="sOutputFilename">输出文件</param>
        public static void EncryptFile(string sInputFilename, string sOutputFilename)
        {

            System.IO.FileStream inFile;
            byte[] binaryData;

            try
            {
                inFile = new System.IO.FileStream(sInputFilename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                binaryData = new Byte[inFile.Length];
                long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);
                inFile.Close();
            }
            catch
            { //(System.Exception exp) {
                return;
            }

            // 转换二进制输入为Base64 UUEncoded输出
            // 每3个字节在源数据里作为4个字节 
            long arrayLength = (long)((4.0d / 3.0d) * binaryData.Length);

            // 如果无法整除4
            if (arrayLength % 4 != 0)
            {
                arrayLength += 4 - arrayLength % 4;
            }

            char[] base64CharArray = new char[arrayLength];
            try
            {
                System.Convert.ToBase64CharArray(binaryData, 0, binaryData.Length, base64CharArray, 0);
            }
            catch (System.ArgumentNullException)
            {
                //二进制数组为NULL.
                return;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                //长度不够
                return;
            }

            // 写UUEncoded数据到文件内
            System.IO.StreamWriter outFile;
            try
            {
                outFile = new System.IO.StreamWriter(sOutputFilename, false, System.Text.Encoding.ASCII);
                outFile.Write(base64CharArray);
                outFile.Close();
            }
            catch
            {// (System.Exception exp) {
                //文件流出错
            }


        }
    }

    /// <summary>
    /// DES 加密
    /// 支持Key(钥匙)加密变化
    /// 支持还原
    /// 
    /// 演示操作：
    ///  // 64位，8个字节
    ///  string sSecretKey;
    ///
    ///  // 获取Key
    ///  sSecretKey = GenerateKey();
    ///
    ///  // 托管
    ///  GCHandle gch = GCHandle.Alloc( sSecretKey,GCHandleType.Pinned );
    ///
    ///  // 加密文件
    ///  EncryptFile(@"C:\MyData.txt",
    ///  @"C:\Encrypted.txt",
    ///  sSecretKey);
    ///
    ///  // 解密文件
    ///  DecryptFile(@"C:\Encrypted.txt",
    ///  @"C:\Decrypted.txt",
    ///  sSecretKey);
    ///
    ///  // 释放托管内容
    ///  ZeroMemory(gch.AddrOfPinnedObject(), sSecretKey.Length * 2);
    ///  gch.Free();
    /// </summary>
    public class DES
    {
        /// <summary>
        /// 创建Key
        /// </summary>
        /// <returns></returns>
        public static string GenerateKey()
        {
            // 创建一个DES 算法的实例。自动产生Key
            DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();

            // 返回自动创建的Key 用于加密
            return ASCIIEncoding.ASCII.GetString(desCrypto.Key);
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="sInputString">输入字符</param>
        /// <param name="sKey">Key</param>
        /// <returns>加密结果</returns>
        public static string EncryptString(string sInputString, string sKey)
        {
            byte[] data = System.Text.Encoding.Default.GetBytes(sInputString);
            byte[] result;
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = DES.CreateEncryptor();
            result = desencrypt.TransformFinalBlock(data, 0, data.Length);

            string desString = "";
            for (int i = 0; i < result.Length; i++)
            {
                desString += result[i].ToString() + "-";
            }

            //return desString.TrimEnd('-');
            return BitConverter.ToString(result);
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="sInputString">输入字符</param>
        /// <param name="sKey">Key</param>
        /// <returns>解密结果</returns>
        public static string DecryptString(string sInputString, string sKey)
        {
            string[] sInput = sInputString.Split("-".ToCharArray());
            byte[] data = new byte[sInput.Length];
            byte[] result;
            for (int i = 0; i < sInput.Length; i++)
                data[i] = byte.Parse(sInput[i], System.Globalization.NumberStyles.HexNumber);

            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = DES.CreateDecryptor();
            result = desencrypt.TransformFinalBlock(data, 0, data.Length);
            return System.Text.Encoding.Default.GetString(result);
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="notEncryptStr">待加密的明文字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string EncryptString(string notEncryptStr)
        {
            //初始化加密器生成器
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] byteNotEncrypt = Encoding.UTF8.GetBytes(notEncryptStr);
            MemoryStream encryptMs = new MemoryStream();
            CryptoStream encryptCs = new CryptoStream(encryptMs, des.CreateEncryptor(des.Key, des.IV), CryptoStreamMode.Write);

            //加密数据
            encryptCs.Write(byteNotEncrypt, 0, byteNotEncrypt.Length);
            encryptCs.FlushFinalBlock();
            encryptMs.Seek(0, SeekOrigin.Begin);

            //将加密后的数据读取出来
            byte[] byteEncrypt = new byte[1024];
            int i = encryptMs.Read(byteEncrypt, 0, 1024);

            encryptCs.Close();

            //将加密后的字节转换为BASE64编码
            string decryptStr = Convert.ToBase64String(byteEncrypt, 0, i);
            return decryptStr;
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="decryptStr">待解密的密文字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptString(string decryptStr)
        {
            //初始化解密器生成器
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();


            byte[] byteNotDecrypt = Convert.FromBase64String(decryptStr);
            MemoryStream decryptMs = new MemoryStream(byteNotDecrypt);
            CryptoStream decryptCs = new CryptoStream(decryptMs, des.CreateDecryptor(des.Key, des.IV), CryptoStreamMode.Read);

            //解密数据
            byte[] byteDecrypt = new byte[1024];
            int i = decryptCs.Read(byteDecrypt, 0, 1024);
            decryptCs.Close();

            //将解密后的字节转换为BASE64编码
            string notEncryptStr = Encoding.UTF8.GetString(byteDecrypt, 0, i);
            return notEncryptStr;
        }

        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="sInputFilename">输入文件</param>
        /// <param name="sOutputFilename">输出文件</param>
        /// <param name="sKey">Key</param>
        public static void EncryptFile(string sInputFilename, string sOutputFilename, string sKey)
        {
            FileStream fsInput = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);

            FileStream fsEncrypted = new FileStream(sOutputFilename, FileMode.Create, FileAccess.Write);
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = DES.CreateEncryptor();
            CryptoStream cryptostream = new CryptoStream(fsEncrypted, desencrypt, CryptoStreamMode.Write);

            byte[] bytearrayinput = new byte[fsInput.Length];
            fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Close();
            fsInput.Close();
            fsEncrypted.Close();
        }

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="sInputFilename">输入文件</param>
        /// <param name="sOutputFilename">输出文件</param>
        /// <param name="sKey">Key</param>
        public static void DecryptFile(string sInputFilename, string sOutputFilename, string sKey)
        {
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

            FileStream fsread = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read); ICryptoTransform desdecrypt = DES.CreateDecryptor();
            CryptoStream cryptostreamDecr = new CryptoStream(fsread, desdecrypt, CryptoStreamMode.Read);
            StreamWriter fsDecrypted = new StreamWriter(sOutputFilename);
            fsDecrypted.Write(new StreamReader(cryptostreamDecr).ReadToEnd());
            fsDecrypted.Flush();
            fsDecrypted.Close();
        }

    }

    /// <summary>
    /// 新版的des加密解密类
    /// </summary>
    public class NewDES
    {

        //加密钥匙
        private static byte[] DESKey = new byte[] { 11, 23, 93, 102, 72, 41, 18, 12 };
        //解密钥匙
        private static byte[] DESIV = new byte[] { 75, 158, 46, 97, 78, 57, 17, 36 };

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="value">待加密的字符串</param>
        /// <returns></returns>
        public string Encode(string value)
        {
            DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
            MemoryStream objMemoryStream = new MemoryStream();
            CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateEncryptor(DESKey, DESIV), CryptoStreamMode.Write);
            StreamWriter objStreamWriter = new StreamWriter(objCryptoStream);
            objStreamWriter.Write(value);
            objStreamWriter.Flush();
            objCryptoStream.FlushFinalBlock();
            objMemoryStream.Flush();
            return Convert.ToBase64String(objMemoryStream.GetBuffer(), 0, (int)objMemoryStream.Length);

        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="value">待解密的字符串</param>
        /// <returns></returns>
        public string Decode(string value)
        {
            DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
            byte[] Input = Convert.FromBase64String(value);
            MemoryStream objMemoryStream = new MemoryStream(Input);
            CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateDecryptor(DESKey, DESIV), CryptoStreamMode.Read);
            StreamReader objStreamReader = new StreamReader(objCryptoStream);
            return objStreamReader.ReadToEnd();
        }
    }

    /*
    SEDO 的摘要说明。 
    SEDO 实现的是用一个封装了4种对称加密方法(Des,Rc2,Rijndael,TripleDes)的组件 
    注意事项： 
    1:TripleDes和Rijndael加密/解密对象使用16或者24位byte的Key 
    2:Rijndael只能使用16位的初始化向量IV 
    3:Des和Rc2均使用8位Byte的Key和IV 
    4:对需要加密/解密的数据流采用何种方法进行编码/解码，由调用组件的用户自己决定 
    5:密钥和初始化向量IV由使用者自己定义 
    */

    /// <summary>
    /// 加密类型的枚举
    /// </summary>
    public enum EncryptionAlgorithm
    {
        /// <summary>
        /// DES加密类型
        /// </summary>
        Des = 1,

        /// <summary>
        /// Rc2加密类型
        /// </summary>
        Rc2,

        /// <summary>
        /// Rijndael加密类型
        /// </summary>
        Rijndael,

        /// <summary>
        /// TripleDes加密类型
        /// </summary>
        TripleDes
    }

    /// <summary>
    /// 加密类
    /// </summary>
    public class EncryptTransformer
    {
        private EncryptionAlgorithm algorithmID;
        private byte[] initVec;
        private byte[] encKey;

        /// <summary>
        /// 初始化实例
        /// </summary>
        /// <param name="algId"></param>
        public EncryptTransformer(EncryptionAlgorithm algId)
        {
            algorithmID = algId;
        }

        /// <summary>
        /// 定义加密算法
        /// </summary>
        /// <param name="bytesKey">加密的数据</param>
        /// <returns></returns>
        public ICryptoTransform GetCryptoServiceProvider(byte[] bytesKey)
        {
            //当数据密钥Key或者初始化向量IV为空的时候，将使用加密对象自动产生的密钥Key或者初始化向量IV 
            switch (algorithmID)
            {
                case EncryptionAlgorithm.Des:
                    {
                        System.Security.Cryptography.DES des = new DESCryptoServiceProvider();
                        des.Mode = CipherMode.CBC;

                        // See if a key was provided 
                        if (null == bytesKey)
                        {
                            encKey = des.Key;
                        }
                        else
                        {
                            des.Key = bytesKey;
                            encKey = des.Key;
                        }
                        if (null == initVec)
                        {
                            initVec = des.IV;
                        }
                        else
                        {
                            des.IV = initVec;
                        }
                        return des.CreateEncryptor();
                    }
                case EncryptionAlgorithm.TripleDes:
                    {
                        TripleDES des3 = new TripleDESCryptoServiceProvider();
                        des3.Mode = CipherMode.CBC;

                        if (null == bytesKey)
                        {
                            encKey = des3.Key;
                        }
                        else
                        {
                            des3.Key = bytesKey;
                            encKey = des3.Key;
                        }

                        if (null == initVec)
                        {
                            initVec = des3.IV;
                        }
                        else
                        {
                            des3.IV = initVec;
                        }
                        return des3.CreateEncryptor();
                    }
                case EncryptionAlgorithm.Rc2:
                    {
                        RC2 rc2 = new RC2CryptoServiceProvider();
                        rc2.Mode = CipherMode.CBC;

                        if (null == bytesKey)
                        {
                            encKey = rc2.Key;
                        }
                        else
                        {
                            rc2.Key = bytesKey;
                            encKey = rc2.Key;
                        }

                        if (null == initVec)
                        {
                            initVec = rc2.IV;
                        }
                        else
                        {
                            rc2.IV = initVec;
                        }
                        return rc2.CreateEncryptor();
                    }
                case EncryptionAlgorithm.Rijndael:
                    {
                        Rijndael rijndael = new RijndaelManaged();
                        rijndael.Mode = CipherMode.CBC;

                        if (null == bytesKey)
                        {
                            encKey = rijndael.Key;
                        }
                        else
                        {
                            rijndael.Key = bytesKey;
                            encKey = rijndael.Key;
                        }

                        if (null == initVec)
                        {
                            initVec = rijndael.IV;
                        }
                        else
                        {
                            rijndael.IV = initVec;
                        }
                        return rijndael.CreateEncryptor();
                    }
                default:
                    {
                        throw new CryptographicException("Algorithm ID ''" +
                        algorithmID +
                        "'' not supported.");
                    }
            }
        }

        /// <summary>
        /// 获取或设置加密的偏移向量
        /// </summary>
        public byte[] IV
        {
            get { return initVec; }
            set { initVec = value; }
        }

        /// <summary>
        /// 获取或设置加密的密钥
        /// </summary>
        public byte[] Key
        {
            get { return encKey; }
            set { encKey = value; }
        }

    }

    /// <summary>
    /// 解密类
    /// </summary> 
    public class DecryptTransformer
    {
        private EncryptionAlgorithm algorithmID;
        private byte[] initVec;
        private byte[] encKey;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deCryptId"></param>
        public DecryptTransformer(EncryptionAlgorithm deCryptId)
        {
            algorithmID = deCryptId;
        }

        /// <summary>
        /// 获取或设置加密的偏移向量 
        /// </summary>
        public byte[] IV
        {
            get { return initVec; }
            set { initVec = value; }
        }

        /// <summary>
        /// 获取或设置加密的密钥
        /// </summary>
        public byte[] Key
        {
            get { return encKey; }
            set { encKey = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytesKey"></param>
        /// <returns></returns>
        public ICryptoTransform GetCryptoServiceProvider(byte[] bytesKey)
        {
            //当数据密钥Key或者初始化向量IV为空的时候，将使用加密对象自动产生的密钥Key或者初始化向量IV 
            switch (algorithmID)
            {
                case EncryptionAlgorithm.Des:
                    {
                        System.Security.Cryptography.DES des = new DESCryptoServiceProvider();
                        des.Mode = CipherMode.CBC;
                        des.Key = bytesKey;
                        des.IV = initVec;
                        return des.CreateDecryptor();
                    }
                case EncryptionAlgorithm.TripleDes:
                    {
                        TripleDES des3 = new TripleDESCryptoServiceProvider();
                        des3.Mode = CipherMode.CBC;
                        return des3.CreateDecryptor(bytesKey, initVec);
                    }
                case EncryptionAlgorithm.Rc2:
                    {
                        RC2 rc2 = new RC2CryptoServiceProvider();
                        rc2.Mode = CipherMode.CBC;
                        return rc2.CreateDecryptor(bytesKey, initVec);
                    }
                case EncryptionAlgorithm.Rijndael:
                    {
                        Rijndael rijndael = new RijndaelManaged();
                        rijndael.Mode = CipherMode.CBC;
                        return rijndael.CreateDecryptor(bytesKey, initVec);
                    }
                default:
                    {
                        throw new CryptographicException("Algorithm ID ''" +
                        algorithmID +
                        "'' not supported.");
                    }
            }
        }

    }

    /// <summary>
    /// 加密者类
    /// </summary>
    public class Encryptor
    {
        private EncryptTransformer transformer;
        private byte[] initVec;
        private byte[] encKey;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="algId"></param>
        public Encryptor(EncryptionAlgorithm algId)
        {
            transformer = new EncryptTransformer(algId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytesData"></param>
        /// <param name="bytesKey"></param>
        /// <param name="bytesIV"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] bytesData, byte[] bytesKey, byte[] bytesIV)
        {
            //设置流对象用来保存加密数据字节流. 
            MemoryStream memStreamEncryptedData = new MemoryStream();

            transformer.IV = bytesIV;
            transformer.Key = bytesKey;

            ICryptoTransform transform = transformer.GetCryptoServiceProvider(bytesKey);
            CryptoStream encStream = new CryptoStream(memStreamEncryptedData, transform, CryptoStreamMode.Write);

            try
            {
                //将加密数据写进流对象 
                encStream.Write(bytesData, 0, bytesData.Length);
            }
            catch (Exception ex)
            {
                throw new Exception("在数据加密的时候出现错误！错误提示： \n" + ex.Message);
            }

            //设置加密的Key和初始向量IV属性 
            encKey = transformer.Key;
            initVec = transformer.IV;

            encStream.FlushFinalBlock();
            encStream.Close();

            //Send the data back. 
            return memStreamEncryptedData.ToArray();
        }

        /// <summary>
        /// 获取或设置加密的偏移向量
        /// </summary>
        public byte[] IV
        {
            get { return initVec; }
            set { initVec = value; }
        }

        /// <summary>
        /// 获取或设置加密的密钥
        /// </summary>
        public byte[] Key
        {
            get { return encKey; }
            set { encKey = value; }
        }

    }

    /// <summary>
    /// 解密者类
    /// </summary>
    public class Decryptor
    {
        private DecryptTransformer transformer;
        private byte[] initVec;
        private byte[] encKey;

        /// <summary>
        /// 初始化实例
        /// </summary>
        /// <param name="algId"></param>
        public Decryptor(EncryptionAlgorithm algId)
        {
            transformer = new DecryptTransformer(algId);
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="bytesData">待加密的数据</param>
        /// <param name="bytesKey">密匙</param>
        /// <param name="bytesIV">偏移量</param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] bytesData, byte[] bytesKey, byte[] bytesIV)
        {
            //设置流对象用来保存解密数据字节流. 
            MemoryStream memStreamDecryptedData = new MemoryStream();

            //Pass in the initialization vector. 
            transformer.IV = bytesIV;
            transformer.Key = bytesKey;

            ICryptoTransform transform = transformer.GetCryptoServiceProvider(bytesKey);
            CryptoStream decStream = new CryptoStream(memStreamDecryptedData, transform, CryptoStreamMode.Write);

            try
            {
                decStream.Write(bytesData, 0, bytesData.Length);
            }
            catch (Exception ex)
            {
                throw new Exception("在数据解密的时候出现错误！错误提示： \n" + ex.Message);
            }
            decStream.FlushFinalBlock();
            decStream.Close();
            // 返回解密数据. 
            return memStreamDecryptedData.ToArray();
        }

        /// <summary>
        /// 获取或设置加密的偏移向量
        /// </summary>
        public byte[] IV
        {
            get { return initVec; }
            set { initVec = value; }
        }

        /// <summary>
        /// 获取或设置加密的密钥
        /// </summary>
        public byte[] Key
        {
            get { return encKey; }
            set { encKey = value; }
        }

    }

    /// <summary>
    /// 文件加密解密类
    /// </summary>
    public class SecurityFile
    {
        private DecryptTransformer Dec_Transformer; //解密转换器 
        private EncryptTransformer Enc_Transformer; //加密转换器 
        private byte[] initVec;
        private byte[] encKey;

        /// <summary>
        /// 初始化实例
        /// </summary>
        /// <param name="algId"></param>
        public SecurityFile(EncryptionAlgorithm algId)
        {
            Dec_Transformer = new DecryptTransformer(algId);
            Enc_Transformer = new EncryptTransformer(algId);
        }

        /// <summary>
        /// 获取或设置加密的偏移向量
        /// </summary>
        public byte[] IV
        {
            get { return initVec; }
            set { initVec = value; }
        }

        /// <summary>
        /// 获取或设置加密的密钥
        /// </summary>
        public byte[] Key
        {
            get { return encKey; }
            set { encKey = value; }
        }

        /// <summary>
        /// 加密文件 
        /// </summary>
        /// <param name="inFileName">待加密的文件的文件名</param>
        /// <param name="outFileName">输出的文件名</param>
        /// <param name="bytesKey">密钥</param>
        /// <param name="bytesIV">偏移量</param>
        public void EncryptFile(string inFileName, string outFileName, byte[] bytesKey, byte[] bytesIV)
        {
            try
            {
                FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                //Create variables to help with read and write. 
                byte[] bin = new byte[100]; //This is intermediate storage for the encryption. 
                long rdlen = 0; //This is the total number of bytes written. 
                long totlen = fin.Length; //This is the total length of the input file. 
                int len; //This is the number of bytes to be written at a time. 

                Enc_Transformer.IV = bytesIV;
                Enc_Transformer.Key = bytesKey;

                ICryptoTransform transform = Enc_Transformer.GetCryptoServiceProvider(bytesKey);
                CryptoStream encStream = new CryptoStream(fout, transform, CryptoStreamMode.Write);

                //Read from the input file, then encrypt and write to the output file. 
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    encStream.Write(bin, 0, len);
                    rdlen = rdlen + len;
                }

                encStream.Close();
                fout.Close();
                fin.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("在文件加密的时候出现错误！错误提示： \n" + ex.Message);
            }
        }

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="inFileName">待加密的文件的文件名</param>
        /// <param name="outFileName">输出的文件名</param>
        /// <param name="bytesKey">密钥</param>
        /// <param name="bytesIV">偏移量</param>
        public void DecryptFile(string inFileName, string outFileName, byte[] bytesKey, byte[] bytesIV)
        {
            try
            {
                FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                //Create variables to help with read and write. 
                byte[] bin = new byte[100]; //This is intermediate storage for the encryption. 
                long rdlen = 0; //This is the total number of bytes written. 
                long totlen = fin.Length; //This is the total length of the input file. 
                int len; //This is the number of bytes to be written at a time. 

                Dec_Transformer.IV = bytesIV;
                Dec_Transformer.Key = bytesKey;

                ICryptoTransform transform = Dec_Transformer.GetCryptoServiceProvider(bytesKey);
                CryptoStream encStream = new CryptoStream(fout, transform, CryptoStreamMode.Write);

                //Read from the input file, then encrypt and write to the output file. 
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    encStream.Write(bin, 0, len);
                    rdlen = rdlen + len;
                }

                encStream.Close();
                fout.Close();
                fin.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("在文件加密的时候出现错误！错误提示： \n" + ex.Message);
            }
        }

    }

    /// <summary>
    /// 非对称RSA
    /// </summary>
    public class RSA
    {
        private RSACryptoServiceProvider rsa;

        /// <summary>
        /// 初始化实例
        /// </summary>
        public RSA()
        {
            rsa = new RSACryptoServiceProvider();
        }

        /// <summary>
        /// 得到公钥
        /// </summary>
        /// <returns></returns>
        public string GetPublicKey()
        {
            return rsa.ToXmlString(false);
        }

        /// <summary>
        /// 得到私钥
        /// </summary>
        /// <returns></returns>
        public string GetPrivateKey()
        {
            return rsa.ToXmlString(true);

        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Source">待加密字符串</param>
        /// <param name="PublicKey">公钥</param>
        /// <returns></returns>
        public string Encrypt(string Source, string PublicKey)
        {
            rsa.FromXmlString(PublicKey);
            byte[] done = rsa.Encrypt(Convert.FromBase64String(Source), false);
            return Convert.ToBase64String(done);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Source">待加密字符数组</param>
        /// <param name="PublicKey">公钥</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] Source, string PublicKey)
        {
            rsa.FromXmlString(PublicKey);
            return rsa.Encrypt(Source, false);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="inFileName">待加密文件路径</param>
        /// <param name="outFileName">加密后文件路径</param>
        /// <param name="PublicKey">公钥</param>
        public void Encrypt(string inFileName, string outFileName, string PublicKey)
        {
            rsa.FromXmlString(PublicKey);
            FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            byte[] bin = new byte[1000];
            long rdlen = 0;
            long totlen = fin.Length;
            int len;

            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 1000);
                byte[] bout = rsa.Encrypt(bin, false);
                fout.Write(bout, 0, bout.Length);
                rdlen = rdlen + len;
            }

            fout.Close();
            fin.Close();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Source">待解密字符串</param>
        /// <param name="PrivateKey">私钥</param>
        /// <returns></returns>
        public string Decrypt(string Source, string PrivateKey)
        {
            rsa.FromXmlString(PrivateKey);
            byte[] done = rsa.Decrypt(Convert.FromBase64String(Source), false);
            return Convert.ToBase64String(done);
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Source">待解密字符数组</param>
        /// <param name="PrivateKey">私钥</param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] Source, string PrivateKey)
        {
            rsa.FromXmlString(PrivateKey);
            return rsa.Decrypt(Source, false);
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="inFileName">待解密文件路径</param>
        /// <param name="outFileName">解密后文件路径</param>
        /// <param name="PrivateKey">私钥</param>
        public void Decrypt(string inFileName, string outFileName, string PrivateKey)
        {
            rsa.FromXmlString(PrivateKey);
            FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            byte[] bin = new byte[1000];
            long rdlen = 0;
            long totlen = fin.Length;
            int len;

            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 1000);
                byte[] bout = rsa.Decrypt(bin, false);
                fout.Write(bout, 0, bout.Length);
                rdlen = rdlen + len;
            }

            fout.Close();
            fin.Close();

        }
    }
}
