using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using CMS_2026.Services;

namespace CMS_2026.Controllers
{
    [Route("admin/ckfinder/core/connector")]
    public class CKFinderController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".bmp", ".gif", ".jpg", ".jpeg", ".png" };

        public CKFinderController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        [HttpPost]
        [Route("aspx/connector.aspx")]
        public IActionResult Connector()
        {
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"[CKFinder] Request: {Request.Method} {Request.Path}{Request.QueryString}");
            
            // Kiểm tra authentication sử dụng AuthenticationService
            var isAuthenticated = AuthenticationService.CheckAuthenticatedUser(HttpContext);
            System.Diagnostics.Debug.WriteLine($"[CKFinder] Authentication: {isAuthenticated}");
            
            if (!isAuthenticated)
            {
                System.Diagnostics.Debug.WriteLine($"[CKFinder] Unauthorized - returning error");
                return UnauthorizedXml("Unauthorized");
            }

            var command = GetValue("command")?.Trim();
            System.Diagnostics.Debug.WriteLine($"[CKFinder] Command: '{command}'");
            
            if (string.IsNullOrEmpty(command))
                return BadRequestXml("Missing command");

            var baseUrl = $"{Request.Scheme}://{Request.Host}/upload/";
            var baseDir = Path.Combine(_env.WebRootPath, "upload");
            Directory.CreateDirectory(baseDir);
            
            System.Diagnostics.Debug.WriteLine($"[CKFinder] baseUrl: '{baseUrl}', baseDir: '{baseDir}', exists: {Directory.Exists(baseDir)}");

            var currentFolder = NormalizeFolder(GetValue("currentFolder"));
            System.Diagnostics.Debug.WriteLine($"[CKFinder] currentFolder: '{currentFolder}'");

            var xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
            var connector = xml.CreateElement("Connector");
            xml.AppendChild(connector);

            try
            {
                switch (command)
                {
                    case "Init": HandleInit(xml, connector, baseUrl, baseDir); break;
                    case "GetFolders": HandleGetFolders(xml, connector, baseDir, currentFolder); break;
                    case "GetFiles": HandleGetFiles(xml, connector, baseDir, currentFolder); break;
                    case "FileUpload": return HandleFileUpload(xml, connector, baseDir, baseUrl, currentFolder);
                    case "QuickUpload": return HandleQuickUpload(baseDir, baseUrl, currentFolder);
                    case "CreateFolder": return HandleCreateFolder(xml, connector, baseDir, currentFolder);
                    case "RenameFolder": return HandleRenameFolder(xml, connector, baseDir, currentFolder);
                    case "DeleteFolder": return HandleDeleteFolder(xml, connector, baseDir, currentFolder);
                    case "RenameFile": return HandleRenameFile(xml, connector, baseDir, currentFolder);
                    case "DeleteFile": return HandleDeleteFile(xml, connector, baseDir, currentFolder);
                    case "Thumbnail": return HandleThumbnail(baseDir, currentFolder);
                    case "DownloadFile": return HandleDownloadFile(baseDir, currentFolder);
                    default:
                        AddError(connector, 10, $"Unknown command: {command}");
                        break;
                }
            }
            catch (Exception ex)
            {
                AddError(connector, 500, ex.Message);
            }

            return Content(xml.OuterXml, "text/xml", System.Text.Encoding.UTF8);
        }

        // =================================================================
        // Helper
        // =================================================================
        private string GetValue(string key)
            => Request.Query[key].ToString() ?? (Request.HasFormContentType ? Request.Form[key].ToString() : null);

        private string NormalizeFolder(string f)
            => string.IsNullOrEmpty(f) || f == "/" ? "/" : "/" + f.Trim('/').Replace("\\", "/") + "/";

        private string PhysicalPath(string baseDir, string folder)
            => Path.Combine(baseDir, folder.Trim('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

        private IActionResult BadRequestXml(string msg) => Content(
            $@"<?xml version=""1.0"" encoding=""utf-8""?><Connector><Error number=""1"" text=""{msg}"" /></Connector>",
            "text/xml");

        private IActionResult UnauthorizedXml(string msg) => BadRequestXml(msg);

        private void AddError(XmlElement parent, int number, string text)
        {
            var err = parent.OwnerDocument!.CreateElement("Error");
            err.SetAttribute("number", number.ToString());
            err.SetAttribute("text", text);
            parent.AppendChild(err);
        }

        private string GetMime(string fileName) => Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };

        // =================================================================
        // Command handlers
        // =================================================================
        private void HandleInit(XmlDocument xml, XmlElement c, string baseUrl, string baseDir)
        {
            var cf = xml.CreateElement("CurrentFolder");
            cf.SetAttribute("path", "/");
            cf.SetAttribute("url", baseUrl);
            cf.SetAttribute("acl", "255");
            c.AppendChild(cf);

            var rts = xml.CreateElement("ResourceTypes");
            var rt = xml.CreateElement("ResourceType");
            rt.SetAttribute("name", "Images");
            rt.SetAttribute("url", baseUrl);
            rt.SetAttribute("dir", baseDir);
            rt.SetAttribute("maxSize", "0");
            rt.SetAttribute("allowedExtensions", "bmp,gif,jpeg,jpg,png");
            rt.SetAttribute("deniedExtensions", "");
            rt.SetAttribute("hasChildren", "true");
            rt.SetAttribute("acl", "255");
            rts.AppendChild(rt);
            c.AppendChild(rts);

            c.AppendChild(xml.CreateElement("PluginsInfo"));

            var ch = xml.CreateElement("Changelog");
            ch.InnerText = "false";
            c.AppendChild(ch);
        }

        private void HandleGetFolders(XmlDocument xml, XmlElement c, string baseDir, string folder)
        {
            var folders = xml.CreateElement("Folders");
            c.AppendChild(folders);
            var path = PhysicalPath(baseDir, folder);
            
            System.Diagnostics.Debug.WriteLine($"[CKFinder] GetFolders - folder: '{folder}', path: '{path}', exists: {Directory.Exists(path)}");
            
            if (!Directory.Exists(path))
            {
                System.Diagnostics.Debug.WriteLine($"[CKFinder] GetFolders - Directory does not exist");
                return;
            }

            var dirs = Directory.GetDirectories(path);
            System.Diagnostics.Debug.WriteLine($"[CKFinder] GetFolders - Found {dirs.Length} directories");
            
                foreach (var dir in dirs)
            {
                var name = Path.GetFileName(dir);
                if (name.StartsWith(".") || name == "_thumbs" || name == ".svn" || name == "CVS")
                {
                    System.Diagnostics.Debug.WriteLine($"[CKFinder] GetFolders - Skipping hidden folder: '{name}'");
                    continue;
                }

                var f = xml.CreateElement("Folder");
                f.SetAttribute("name", name);
                f.SetAttribute("hasChildren", Directory.GetDirectories(dir).Length > 0 ? "true" : "false");
                f.SetAttribute("acl", "255");
                folders.AppendChild(f);
                System.Diagnostics.Debug.WriteLine($"[CKFinder] GetFolders - Added folder: '{name}'");
            }
        }

        private void HandleGetFiles(XmlDocument xml, XmlElement c, string baseDir, string folder)
        {
            var files = xml.CreateElement("Files");
            c.AppendChild(files);
            var path = PhysicalPath(baseDir, folder);
            
            System.Diagnostics.Debug.WriteLine($"[CKFinder] GetFiles - folder: '{folder}', path: '{path}', exists: {Directory.Exists(path)}");
            
            if (!Directory.Exists(path))
            {
                System.Diagnostics.Debug.WriteLine($"[CKFinder] GetFiles - Directory does not exist");
                return;
            }

            var allFiles = Directory.GetFiles(path);
            var imageFiles = allFiles.Where(f => 
            {
                var ext = Path.GetExtension(f).ToLowerInvariant();
                var name = Path.GetFileName(f);
                return _allowedExtensions.Contains(ext) && !name.StartsWith(".");
            }).ToList();
            
            System.Diagnostics.Debug.WriteLine($"[CKFinder] GetFiles - Found {allFiles.Length} total files, {imageFiles.Count} image files");

            foreach (var file in imageFiles)
            {
                var fi = new FileInfo(file);
                var f = xml.CreateElement("File");
                f.SetAttribute("name", fi.Name);
                f.SetAttribute("size", fi.Length.ToString());
                f.SetAttribute("date", fi.LastWriteTime.ToString("yyyyMMddHHmmss"));
                files.AppendChild(f);
                System.Diagnostics.Debug.WriteLine($"[CKFinder] GetFiles - Added file: '{fi.Name}'");
            }
        }

        private IActionResult HandleThumbnail(string baseDir, string folder)
        {
            var fileName = GetValue("FileName");
            if (string.IsNullOrEmpty(fileName)) return NotFound();

            var folderPath = PhysicalPath(baseDir, folder);
            var filePath = Path.Combine(folderPath, fileName);
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var thumbsDir = Path.Combine(folderPath, "_thumbs");
            Directory.CreateDirectory(thumbsDir);
            var thumbPath = Path.Combine(thumbsDir, fileName);

            if (!System.IO.File.Exists(thumbPath))
            {
                using var img = Image.Load(filePath);
                img.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(120, 120), Mode = ResizeMode.Max }));
                img.Save(thumbPath);
            }

            return PhysicalFile(thumbPath, GetMime(fileName));
        }

        private IActionResult HandleDownloadFile(string baseDir, string folder)
        {
            var fileName = GetValue("FileName");
            if (string.IsNullOrEmpty(fileName)) return NotFound();
            var filePath = Path.Combine(PhysicalPath(baseDir, folder), fileName);
            return System.IO.File.Exists(filePath)
                ? PhysicalFile(filePath, GetMime(fileName), fileName)
                : NotFound();
        }

        private IActionResult HandleQuickUpload(string baseDir, string baseUrl, string folder)
        {
            if (Request.Form.Files.Count == 0) return BadRequest();
            var url = UploadFile(Request.Form.Files[0], baseDir, baseUrl, folder);
            return Content($"<script>window.parent.CKFinder.tools.callFunction(1, '{url}', '');</script>", "text/html");
        }

        private IActionResult HandleFileUpload(XmlDocument xml, XmlElement c, string baseDir, string baseUrl, string folder)
        {
            var file = Request.Form.Files["NewFile"] ?? Request.Form.Files.FirstOrDefault();
            if (file == null)
            {
                AddError(c, 202, "No file uploaded");
                return Content(xml.OuterXml, "text/xml");
            }

            var url = UploadFile(file, baseDir, baseUrl, folder);
            var uploaded = xml.CreateElement("Uploaded");
            uploaded.SetAttribute("name", Path.GetFileName(url));
            c.AppendChild(uploaded);
            return Content(xml.OuterXml, "text/xml");
        }

        private string UploadFile(IFormFile file, string baseDir, string baseUrl, string folder)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(ext))
                throw new UnauthorizedAccessException("File type not allowed");

            var folderPath = PhysicalPath(baseDir, folder);
            Directory.CreateDirectory(folderPath);

            var safeName = Path.GetFileName(file.FileName);
            var fullPath = Path.Combine(folderPath, safeName);
            if (System.IO.File.Exists(fullPath))
            {
                var name = Path.GetFileNameWithoutExtension(safeName);
                safeName = $"{name}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                fullPath = Path.Combine(folderPath, safeName);
            }

            using (var stream = new FileStream(fullPath, FileMode.Create))
                file.CopyTo(stream);

            return $"{baseUrl}{folder.TrimStart('/')}{safeName}";
        }

        // ====================== CÁC HÀM CÒN LẠI ======================
        private IActionResult HandleCreateFolder(XmlDocument xml, XmlElement c, string baseDir, string currentFolder)
        {
            var newFolderName = GetValue("NewFolderName");
            if (string.IsNullOrEmpty(newFolderName) || newFolderName.Contains("..") || newFolderName.Contains("/") || newFolderName.Contains("\\"))
            {
                AddError(c, 102, "Invalid folder name");
                return Content(xml.OuterXml, "text/xml");
            }

            var parentPath = PhysicalPath(baseDir, currentFolder);
            var newPath = Path.Combine(parentPath, newFolderName);
            if (Directory.Exists(newPath))
            {
                AddError(c, 115, "Folder already exists");
                return Content(xml.OuterXml, "text/xml");
            }

            Directory.CreateDirectory(newPath);
            return Content(xml.OuterXml, "text/xml");
        }

        private IActionResult HandleRenameFolder(XmlDocument xml, XmlElement c, string baseDir, string currentFolder)
        {
            var oldName = GetValue("OldFolderName");
            var newName = GetValue("NewFolderName");
            if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName) || newName.Contains("..") || newName.Contains("/") || newName.Contains("\\"))
            {
                AddError(c, 102, "Invalid name");
                return Content(xml.OuterXml, "text/xml");
            }

            var parentPath = PhysicalPath(baseDir, currentFolder);
            var oldPath = Path.Combine(parentPath, oldName);
            var newPath = Path.Combine(parentPath, newName);

            if (!Directory.Exists(oldPath)) { AddError(c, 104, "Folder not found"); return Content(xml.OuterXml, "text/xml"); }
            if (Directory.Exists(newPath)) { AddError(c, 115, "Already exists"); return Content(xml.OuterXml, "text/xml"); }

            Directory.Move(oldPath, newPath);
            return Content(xml.OuterXml, "text/xml");
        }

        private IActionResult HandleDeleteFolder(XmlDocument xml, XmlElement c, string baseDir, string currentFolder)
        {
            var folderName = GetValue("FolderName");
            if (string.IsNullOrEmpty(folderName))
            {
                AddError(c, 102, "Invalid folder name");
                return Content(xml.OuterXml, "text/xml");
            }

            var folderPath = Path.Combine(PhysicalPath(baseDir, currentFolder), folderName);
            if (!Directory.Exists(folderPath))
            {
                AddError(c, 104, "Folder not found");
                return Content(xml.OuterXml, "text/xml");
            }

            Directory.Delete(folderPath, true);
            return Content(xml.OuterXml, "text/xml");
        }

        private IActionResult HandleRenameFile(XmlDocument xml, XmlElement c, string baseDir, string currentFolder)
        {
            var oldName = GetValue("OldFileName");
            var newName = GetValue("NewFileName");
            if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName))
            {
                AddError(c, 102, "Invalid file name");
                return Content(xml.OuterXml, "text/xml");
            }

            var ext = Path.GetExtension(newName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(ext))
            {
                AddError(c, 105, "Invalid extension");
                return Content(xml.OuterXml, "text/xml");
            }

            var folderPath = PhysicalPath(baseDir, currentFolder);
            var oldPath = Path.Combine(folderPath, oldName);
            var newPath = Path.Combine(folderPath, newName);

            if (!System.IO.File.Exists(oldPath)) { AddError(c, 104, "File not found"); return Content(xml.OuterXml, "text/xml"); }
            if (System.IO.File.Exists(newPath)) { AddError(c, 115, "File exists"); return Content(xml.OuterXml, "text/xml"); }

            System.IO.File.Move(oldPath, newPath);
            return Content(xml.OuterXml, "text/xml");
        }

        private IActionResult HandleDeleteFile(XmlDocument xml, XmlElement c, string baseDir, string currentFolder)
        {
            var fileName = GetValue("FileName");
            if (string.IsNullOrEmpty(fileName))
            {
                AddError(c, 102, "Invalid file name");
                return Content(xml.OuterXml, "text/xml");
            }

            var filePath = Path.Combine(PhysicalPath(baseDir, currentFolder), fileName);
            if (!System.IO.File.Exists(filePath))
            {
                AddError(c, 104, "File not found");
                return Content(xml.OuterXml, "text/xml");
            }

            System.IO.File.Delete(filePath);
            return Content(xml.OuterXml, "text/xml");
        }
    }
}