using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AploadPaymentsAccruals.Models
{
    public class FileModel
    {
        static List<FileDTO> files = new List<FileDTO>();

        public static void SaveFileInDB(FileDTO dto)
        {
            files.Add(dto);
        }
        public static FileDTO GetFileByUniqueID(String uniqueName)
        {
            return files.Where(p => p.FileUniqueName == uniqueName).FirstOrDefault();
        }

        public static List<FileDTO> GetAllFilesInDB()
        {
            return files;
        }

        public static List<FileDTO> GetFilesInPath(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            //string[] dirs = Directory.GetDirectories(directoryInfo.FullName);
            //string[] files_dir = Directory.GetFiles(path);

            files.Clear();
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                FileDTO fileDTO = new FileDTO();
                fileDTO.FileActualName = file.Name.Substring(0, file.Name.LastIndexOf('.'));
                fileDTO.FileUniqueName = file.Name.Substring(0,file.Name.LastIndexOf('.'));
                fileDTO.FileExt = file.Extension;
                fileDTO.ContentType = GetContentType(file.Name);

                files.Add(fileDTO);
            }
           
            return files;
        }

        public static string GetContentType(string fileName)
        {
            string contentType = "application/unknown";
            string ext = Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext); // henter info fra windows registry
            if (regKey != null && regKey.GetValue("Content Type") != null)
            {
                contentType = regKey.GetValue("Content Type").ToString();
            }
            else if (ext == ".doc") // a couple of extra info, due to missing information on the server
            {
                contentType = "application/msword";
            }
            else if (ext == ".docx") // a couple of extra info, due to missing information on the server
            {
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            }
            else if (ext == ".xls") // a couple of extra info, due to missing information on the server
            {
                contentType = "application/ms-excel";
            }
            else if (ext == ".txt") // a couple of extra info, due to missing information on the server
            {
                contentType = "text/plain";
            }
            else if (ext == ".pdf") // a couple of extra info, due to missing information on the server
            {
                contentType = "application/pdf";
            }
            else if (ext == ".png") // a couple of extra info, due to missing information on the server
            {
                contentType = "image/png";
            }
            else if (ext == ".jpg" || ext == ".jpeg") // a couple of extra info, due to missing information on the server
            {
                contentType = "image/jpeg";
            }
            else if (ext == ".flv")
            {
                contentType = "video/x-flv";
            }
            return contentType;
        }
    }

    public class FileDTO
    {
        public String FileUniqueName { get; set; }
        public String FileActualName { get; set; }
        public String ContentType { get; set; }
        public String FileExt { get; set; }
    }
}