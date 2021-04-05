using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AploadPaymentsAccruals.Models;


namespace AploadPaymentsAccruals.Controllers
{
    public class HomeController : Controller
    {        public ActionResult Index()
        {
            //Physical Path of Root Folder
            string rootPath = System.Web.HttpContext.Current.Server.MapPath("~/UploadedFiles");
            //
            List<FileDTO> files = new List<FileDTO>();
            if (Directory.Exists(rootPath))
            {
                //var files = FileModel.GetAllFiles();
                files = FileModel.GetFilesInPath(rootPath);
            }
            else
            {
                
            }
            return View(files);
        }
    }
}