using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyTodo.Common.Cache
{
    internal class CacheManager
    {
        private static string fileName = System.IO.Path.Combine(Environment.CurrentDirectory, "Common", "Cache", "info.ini");

        public string path;
        [DllImport("kernel32")] //返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")] //返回取得字符串缓冲区的长度
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        private static void checkFile()
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "Common", "Cache");
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(fileName))
            {
                File.Create(fileName);
            }
        }

        /// <summary>
        /// 写Ini文件
        /// 调用示例：ini.IniWritevalue("Server","name","localhost");
        /// </summary>
        /// <param name="Section">[缓冲区]</param>
        /// <param name="Key">键</param>
        /// <param name="value">值</param>
        public static void IniWritevalue(string Section, string Key, string value)
        {
            checkFile();
            WritePrivateProfileString(Section, Key, value, fileName);
        }
        /// <summary>
        /// 读Ini文件
        /// 调用示例：ini.IniWritevalue("Server","name");
        /// </summary>
        /// <param name="Section">[缓冲区]</param>
        /// <param name="Key">键</param>
        /// <returns>值</returns>
        public static string IniReadvalue(string Section, string Key)
        {
            checkFile();
            StringBuilder temp = new StringBuilder(255);

            int i = GetPrivateProfileString(Section, Key, "", temp, 255, fileName);
            return temp.ToString();
        }

        /// <summary>
        /// 删除节
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static long DeleteSection(string section)
        {
            checkFile();
            return WritePrivateProfileString(section, null, null, fileName);
        }

        /// <summary>
        /// 删除键
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static long DeleteKey(string section, string key)
        {
            checkFile();
            return WritePrivateProfileString(section, key, null, fileName);
        }


    }
}
