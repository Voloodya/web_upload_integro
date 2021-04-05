using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace AploadPaymentsAccruals.Models
{
    public class ReadWriteFileTxt
    {
        public static List<string> readFile(string filePath)
        {
            List<String> fileContentList = new List<string>();

            StreamReader fileStream = new StreamReader(filePath, Encoding.GetEncoding("Windows-1251"));

            while (!fileStream.EndOfStream)
            {
                //Encoding utf = Encoding.UTF8;
                //Encoding win = Encoding.GetEncoding(1251);

                //byte[] winArr = win.GetBytes(fileStream.ReadLine());
                //byte[] utfArr = Encoding.Convert(utf, win, winArr);
                //string str = utf.GetString(utfArr);

                fileContentList.Add(fileStream.ReadLine());
            }
            return fileContentList;
        }

        public static void writeFile(List<string[]> listWrite, string path, string nameFile, string typeFile)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            using (FileStream fileStream = new FileStream($"{path}\\{nameFile}.{typeFile}", FileMode.Create))
            {
                StreamWriter writer = new StreamWriter(fileStream, Encoding.GetEncoding("Windows-1251"));


                foreach (string[] valueColumns in listWrite)
                {
                    string newStr = "";
                    for (int i = 0; i < valueColumns.Length; i++)
                    {
                        newStr += valueColumns[i] + "|";
                    }

                    writer.WriteLine(newStr);
                }
                writer.Close();
            }
        }
        public static List<string[]> splitString(List<string> stringList, char parserChar)
        {
            List<string[]> splitStringList = new List<string[]>(stringList.Count);

            for (int i = 0; i < stringList.Count; i++)
            {
                string[] valueColumns = stringList[i].Split(parserChar);
                splitStringList.Add(valueColumns);
            }

            return splitStringList;
        }
    }
}