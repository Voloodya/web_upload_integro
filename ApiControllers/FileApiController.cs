using AploadPaymentsAccruals.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace AploadPaymentsAccruals.ApiControllers
{
    public class FileApiController : ApiController
    {
        [HttpPost]
        [Route("api/FileApi/UploadFile")]
        public void UploadFile()
        {
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            if (HttpContext.Current.Request.Files.Count > 0)
            {
                try
                {
                    foreach (var fileName in HttpContext.Current.Request.Files.AllKeys)
                    {
                        HttpPostedFile file = HttpContext.Current.Request.Files[fileName];
                        if (file != null)
                        {
                            FileDTO fileDTO = new FileDTO();

                            fileDTO.FileActualName = file.FileName;
                            fileDTO.FileExt = Path.GetExtension(file.FileName);
                            fileDTO.ContentType = file.ContentType;

                            //Generate a unique name using Guid
                            //fileDTO.FileUniqueName = Guid.NewGuid().ToString();
                            DateTime dateTime = new DateTime();
                            string fileUniqueNameExt = file.FileName.Replace(".", " " + dateTime.ToString().Replace('.', '_').Replace(':', '_') + ".");
                            fileDTO.FileUniqueName = fileUniqueNameExt.Substring(0, fileUniqueNameExt.LastIndexOf('.'));

                            //Get physical path of our folder where we want to save images
                            //string rootPath = HttpContext.Current.Server.MapPath("~/UploadedFiles");
                            string rootPath = AppDomain.CurrentDomain.BaseDirectory; // You get main rott
                            //string rootPath = Directory.GetCurrentDirectory();
                            string safePath;
                            if (Directory.Exists(rootPath + "\\UploadedFiles"))
                            {
                                safePath = rootPath + "\\UploadedFiles";
                            }
                            else
                            {
                                Directory.CreateDirectory(rootPath + "\\UploadedFiles");
                                safePath = rootPath + "\\UploadedFiles";
                            }                            

                            var fileSavePath = System.IO.Path.Combine(safePath, fileDTO.FileUniqueName + fileDTO.FileExt);

                            // Save the uploaded file to "UploadedFiles" folder
                            file.SaveAs(fileSavePath);

                            //Save File Meta data in Database
                            FileModel.SaveFileInDB(fileDTO);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    HttpContext.Current.Response.Write("<h3>" + ex.ToString() + "</h3>");
                }
            }
            var infoFile = HttpContext.Current.Request["infoFile"];
        }

        [HttpGet]
        [Route("api/FileApi/DownloadFile")]
        public Object DownloadFile(String uniqueName)
        {
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            //Physical Path of Root Folder
            string rootPath = System.Web.HttpContext.Current.Server.MapPath("~/UploadedFiles");

            FileModel.GetFilesInPath(rootPath);
            //Find File from DB against unique name
            FileDTO fileDTO = FileModel.GetFileByUniqueID(uniqueName);

            if (fileDTO != null)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                var fileFullPath = System.IO.Path.Combine(rootPath, fileDTO.FileUniqueName + fileDTO.FileExt);

                byte[] file = System.IO.File.ReadAllBytes(fileFullPath);
                System.IO.MemoryStream ms = new System.IO.MemoryStream(file);

                response.Content = new ByteArrayContent(file);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                //String mimeType = MimeType.GetMimeType(file); //You may do your hard coding here based on file extension

                response.Content.Headers.ContentType = new MediaTypeHeaderValue(fileDTO.ContentType);// obj.DocumentName.Substring(obj.DocumentName.LastIndexOf(".") + 1, 3);
                response.Content.Headers.ContentDisposition.FileName = fileDTO.FileActualName+fileDTO.FileExt;
                return response;
            }
            else
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
                return response;
            }
        }

        [HttpGet]
        [Route("api/FileApi/RemoveFile")]
        public Object RemoveFile(String uniqueName)
        {
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            //Physical Path of Root Folder
            string rootPath = System.Web.HttpContext.Current.Server.MapPath("~/UploadedFiles");

            //Find File from DB against unique name
            FileDTO fileDTO = FileModel.GetFileByUniqueID(uniqueName);

            if (fileDTO != null)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                var fileFullPath = System.IO.Path.Combine(rootPath, fileDTO.FileUniqueName + fileDTO.FileExt);

                File.Delete(fileFullPath);

                return response;
            }
            else
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
                return response;
            }
        }


        [HttpPost]
        [Route("api/FileApi/uploadData")]
        public void UploadData()
        {
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            //Physical Path of Root Folder
            string rootPath = System.Web.HttpContext.Current.Server.MapPath("~/UploadedFiles");

            string fileName = HttpContext.Current.Request.Params.Get("uname");

            //Find File from DB against unique name
            FileDTO fileDTO = FileModel.GetFileByUniqueID(fileName);

            UploadPaymentsAccruals uploadPaymentsAccruals = new UploadPaymentsAccruals();

            string answerUpload="";
            if (fileDTO != null)
            {
                answerUpload=uploadPaymentsAccruals.createNewRecording("localhost");
            }
            else
            {
            }
            HttpContext.Current.Response.Write("<h4>" + answerUpload + "</h4>");
        }
    }    
}