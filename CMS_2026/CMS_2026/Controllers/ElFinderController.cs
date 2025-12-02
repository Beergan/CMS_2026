using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using CMS_2026.Services;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;

namespace CMS_2026.Controllers
{
    [Route("admin/elfinder")]
    [ApiController]
    public class ElFinderController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".bmp", ".gif", ".jpg", ".jpeg", ".png" };
        private readonly string[] _allowedMimeTypes = { "image/bmp", "image/gif", "image/jpeg", "image/png" };

        public ElFinderController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("connector")]
        [HttpGet("connector")]
        public async Task<IActionResult> Connector()
        {
            // Check authentication
            if (!AuthenticationService.CheckAuthenticatedUser(HttpContext))
            {
                return JsonResponse(new { error = "Unauthorized" });
            }

            var cmd = Request.Query["cmd"].ToString();
            if (string.IsNullOrEmpty(cmd) && Request.HasFormContentType)
            {
                cmd = Request.Form["cmd"].ToString();
            }

            if (string.IsNullOrEmpty(cmd))
            {
                return JsonResponse(new { error = "Missing command" });
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}/upload/";
            var baseDir = Path.Combine(_env.WebRootPath, "upload");
            Directory.CreateDirectory(baseDir);

            try
            {
                switch (cmd)
                {
                    case "open":
                        return HandleOpen(baseDir, baseUrl);
                    case "file":
                        return HandleFile(baseDir, baseUrl);
                    case "tree":
                        return HandleTree(baseDir, baseUrl);
                    case "parents":
                        return HandleParents(baseDir, baseUrl);
                    case "tmb":
                        return HandleThumbnail(baseDir);
                    case "upload":
                        return await HandleUpload(baseDir, baseUrl);
                    case "mkdir":
                        return HandleMkdir(baseDir);
                    case "rm":
                        return HandleRm(baseDir);
                    case "rename":
                        return HandleRename(baseDir);
                    case "duplicate":
                        return HandleDuplicate(baseDir);
                    case "paste":
                        return HandlePaste(baseDir);
                    case "get":
                        return HandleGet(baseDir);
                    case "put":
                        return await HandlePut(baseDir);
                    case "archive":
                        return HandleArchive(baseDir);
                    case "extract":
                        return HandleExtract(baseDir);
                    case "search":
                        return HandleSearch(baseDir, baseUrl);
                    default:
                        return JsonResponse(new { error = $"Unknown command: {cmd}" });
                }
            }
            catch (Exception ex)
            {
                return JsonResponse(new { error = ex.Message });
            }
        }

        private IActionResult JsonResponse(object data)
        {
            return Content(JsonSerializer.Serialize(data), "application/json", System.Text.Encoding.UTF8);
        }

        private string GetRequestValue(string key)
        {
            var value = Request.Query[key].ToString();
            if (!string.IsNullOrEmpty(value))
                return value;

            if (Request.HasFormContentType)
            {
                value = Request.Form[key].ToString();
                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            return string.Empty;
        }

        private string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path) || path == "/")
                return "";
            return path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        }

        private string GetHash(string path, string baseDir)
        {
            if (string.IsNullOrEmpty(path) || path == baseDir)
                return "l1_Lw";
            
            var relativePath = path.Replace(baseDir, "").Replace(Path.DirectorySeparatorChar, '/').TrimStart('/');
            if (string.IsNullOrEmpty(relativePath) || relativePath == "/")
                return "l1_Lw";
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(relativePath);
            var base64 = Convert.ToBase64String(bytes);
            return "l1_" + base64.Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        private string GetPathFromHash(string hash, string baseDir)
        {
            if (hash == "l1_Lw" || hash == "l1_" || string.IsNullOrEmpty(hash))
                return baseDir;
            
            try
            {
                if (hash.Length < 4 || !hash.StartsWith("l1_"))
                    return baseDir;
                    
                var base64 = hash.Substring(3).Replace("-", "+").Replace("_", "/");
                var padding = 4 - (base64.Length % 4);
                if (padding != 4)
                    base64 += new string('=', padding);
                
                var pathBytes = Convert.FromBase64String(base64);
                var relativePath = System.Text.Encoding.UTF8.GetString(pathBytes);
                return Path.Combine(baseDir, relativePath.Replace('/', Path.DirectorySeparatorChar));
            }
            catch
            {
                return baseDir;
            }
        }

        private object GetFileInfo(string filePath, string baseDir, string baseUrl)
        {
            var fileInfo = new FileInfo(filePath);
            var relativePath = filePath.Replace(baseDir, "").Replace(Path.DirectorySeparatorChar, '/').TrimStart('/');
            var hash = GetHash(filePath, baseDir);
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            var mime = GetMimeType(ext);

            return new
            {
                hash = hash,
                name = fileInfo.Name,
                mime = mime,
                date = fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                size = fileInfo.Length,
                read = 1,
                write = 1,
                locked = 0,
                tmb = _allowedMimeTypes.Contains(mime) ? hash : null,
                url = $"{baseUrl}{relativePath}"
            };
        }

        private object GetDirInfo(string dirPath, string baseDir, string baseUrl)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            var hash = GetHash(dirPath, baseDir);
            var hasSubdirs = Directory.GetDirectories(dirPath).Length > 0;

            return new
            {
                hash = hash,
                name = dirInfo.Name,
                mime = "directory",
                date = dirInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                size = 0,
                dirs = hasSubdirs ? 1 : 0,
                read = 1,
                write = 1,
                locked = 0
            };
        }

        private string GetMimeType(string ext)
        {
            return ext.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
        }

        private IActionResult HandleOpen(string baseDir, string baseUrl)
        {
            var target = GetRequestValue("target");
            if (string.IsNullOrEmpty(target))
                target = "l1_Lw";

            var path = GetPathFromHash(target, baseDir);
            if (!Directory.Exists(path))
                return JsonResponse(new { error = "Directory not found" });

            var files = new List<object>();
            var dirs = new List<object>();

            // Get directories
            foreach (var dir in Directory.GetDirectories(path))
            {
                var dirName = Path.GetFileName(dir);
                if (dirName.StartsWith(".") || dirName == "_thumbs")
                    continue;

                dirs.Add(GetDirInfo(dir, baseDir, baseUrl));
            }

            // Get files
            foreach (var file in Directory.GetFiles(path))
            {
                var fileName = Path.GetFileName(file);
                if (fileName.StartsWith("."))
                    continue;

                var ext = Path.GetExtension(file).ToLowerInvariant();
                if (!_allowedExtensions.Contains(ext))
                    continue;

                files.Add(GetFileInfo(file, baseDir, baseUrl));
            }

            return JsonResponse(new
            {
                cwd = GetDirInfo(path, baseDir, baseUrl),
                files = dirs.Concat(files).ToList(),
                options = new
                {
                    path = path.Replace(baseDir, "").Replace(Path.DirectorySeparatorChar, '/').TrimStart('/') ?? "/",
                    url = baseUrl,
                    tmbUrl = baseUrl + "_thumbs/",
                    disabled = new string[0],
                    separate = new { thumbnails = "_thumbs" }
                }
            });
        }

        private IActionResult HandleFile(string baseDir, string baseUrl)
        {
            var target = GetRequestValue("target");
            if (string.IsNullOrEmpty(target))
                return JsonResponse(new { error = "Missing target" });

            var path = GetPathFromHash(target, baseDir);
            if (System.IO.File.Exists(path))
            {
                return JsonResponse(new { file = new[] { GetFileInfo(path, baseDir, baseUrl) } });
            }
            else if (Directory.Exists(path))
            {
                return JsonResponse(new { file = new[] { GetDirInfo(path, baseDir, baseUrl) } });
            }

            return JsonResponse(new { error = "File not found" });
        }

        private IActionResult HandleTree(string baseDir, string baseUrl)
        {
            var target = GetRequestValue("target");
            if (string.IsNullOrEmpty(target))
                target = "l1_Lw";

            var path = GetPathFromHash(target, baseDir);
            if (!Directory.Exists(path))
                return JsonResponse(new { tree = new object[0] });

            var tree = new List<object>();
            foreach (var dir in Directory.GetDirectories(path))
            {
                var dirName = Path.GetFileName(dir);
                if (dirName.StartsWith(".") || dirName == "_thumbs")
                    continue;

                var dirInfo = GetDirInfo(dir, baseDir, baseUrl);
                var hasSubdirs = Directory.GetDirectories(dir).Length > 0;
                tree.Add(new
                {
                    hash = ((dynamic)dirInfo).hash,
                    name = ((dynamic)dirInfo).name,
                    mime = "directory",
                    date = ((dynamic)dirInfo).date,
                    size = 0,
                    dirs = hasSubdirs ? 1 : 0,
                    read = 1,
                    write = 1,
                    locked = 0
                });
            }

            return JsonResponse(new { tree });
        }

        private IActionResult HandleParents(string baseDir, string baseUrl)
        {
            var target = GetRequestValue("target");
            if (string.IsNullOrEmpty(target))
                return JsonResponse(new { tree = new object[0] });

            var path = GetPathFromHash(target, baseDir);
            var parents = new List<object>();
            var current = new DirectoryInfo(path);

            while (current != null && current.FullName.StartsWith(baseDir))
            {
                parents.Insert(0, GetDirInfo(current.FullName, baseDir, baseUrl));
                current = current.Parent;
            }

            return JsonResponse(new { tree = parents });
        }

        private IActionResult HandleThumbnail(string baseDir)
        {
            var target = GetRequestValue("target");
            if (string.IsNullOrEmpty(target))
                return NotFound();

            var path = GetPathFromHash(target, baseDir);
            if (!System.IO.File.Exists(path))
                return NotFound();

            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (!_allowedExtensions.Contains(ext))
                return NotFound();

            var thumbsDir = Path.Combine(Path.GetDirectoryName(path)!, "_thumbs");
            Directory.CreateDirectory(thumbsDir);
            var thumbPath = Path.Combine(thumbsDir, Path.GetFileName(path));

            if (!System.IO.File.Exists(thumbPath))
            {
                using var img = SixLabors.ImageSharp.Image.Load(path);
                img.Mutate(x => x.Resize(new SixLabors.ImageSharp.Processing.ResizeOptions
                {
                    Size = new SixLabors.ImageSharp.Size(120, 120),
                    Mode = SixLabors.ImageSharp.Processing.ResizeMode.Max
                }));
                img.Save(thumbPath);
            }

            return PhysicalFile(thumbPath, GetMimeType(ext));
        }

        private async Task<IActionResult> HandleUpload(string baseDir, string baseUrl)
        {
            var target = GetRequestValue("target");
            var path = GetPathFromHash(target ?? "l1_Lw", baseDir);
            Directory.CreateDirectory(path);

            var uploaded = new List<object>();
            foreach (var file in Request.Form.Files)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(ext))
                    continue;

                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(path, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                    filePath = Path.Combine(path, fileName);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                uploaded.Add(GetFileInfo(filePath, baseDir, baseUrl));
            }

            return JsonResponse(new { added = uploaded });
        }

        private IActionResult HandleMkdir(string baseDir)
        {
            var target = GetRequestValue("target");
            var name = GetRequestValue("name");
            
            if (string.IsNullOrEmpty(name) || name.Contains("..") || name.Contains("/") || name.Contains("\\"))
                return JsonResponse(new { error = "Invalid folder name" });

            var path = GetPathFromHash(target ?? "l1_Lw", baseDir);
            var newPath = Path.Combine(path, name);

            if (Directory.Exists(newPath))
                return JsonResponse(new { error = "Folder already exists" });

            Directory.CreateDirectory(newPath);
            return JsonResponse(new { added = new[] { GetDirInfo(newPath, baseDir, $"{Request.Scheme}://{Request.Host}/upload/") } });
        }

        private IActionResult HandleRm(string baseDir)
        {
            var targets = GetRequestValue("targets").Split(',');
            var removed = new List<string>();

            foreach (var target in targets)
            {
                if (string.IsNullOrEmpty(target))
                    continue;

                var path = GetPathFromHash(target, baseDir);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    removed.Add(target);
                }
                else if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    removed.Add(target);
                }
            }

            return JsonResponse(new { removed });
        }

        private IActionResult HandleRename(string baseDir)
        {
            var target = GetRequestValue("target");
            var name = GetRequestValue("name");

            if (string.IsNullOrEmpty(name) || name.Contains("..") || name.Contains("/") || name.Contains("\\"))
                return JsonResponse(new { error = "Invalid name" });

            var path = GetPathFromHash(target, baseDir);
            var dir = Path.GetDirectoryName(path);
            var newPath = Path.Combine(dir!, name);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Move(path, newPath);
            }
            else if (Directory.Exists(path))
            {
                Directory.Move(path, newPath);
            }
            else
            {
                return JsonResponse(new { error = "File not found" });
            }

            return JsonResponse(new { added = new[] { System.IO.File.Exists(newPath) ? GetFileInfo(newPath, baseDir, $"{Request.Scheme}://{Request.Host}/upload/") : GetDirInfo(newPath, baseDir, $"{Request.Scheme}://{Request.Host}/upload/") } });
        }

        private IActionResult HandleDuplicate(string baseDir)
        {
            // Not implemented for now
            return JsonResponse(new { error = "Not implemented" });
        }

        private IActionResult HandlePaste(string baseDir)
        {
            // Not implemented for now
            return JsonResponse(new { error = "Not implemented" });
        }

        private IActionResult HandleGet(string baseDir)
        {
            var target = GetRequestValue("target");
            var path = GetPathFromHash(target, baseDir);

            if (System.IO.File.Exists(path))
            {
                return PhysicalFile(path, GetMimeType(Path.GetExtension(path)), Path.GetFileName(path));
            }

            return NotFound();
        }

        private async Task<IActionResult> HandlePut(string baseDir)
        {
            var target = GetRequestValue("target");
            var content = GetRequestValue("content");

            var path = GetPathFromHash(target, baseDir);
            await System.IO.File.WriteAllTextAsync(path, content);

            return JsonResponse(new { changed = new[] { GetFileInfo(path, baseDir, $"{Request.Scheme}://{Request.Host}/upload/") } });
        }

        private IActionResult HandleArchive(string baseDir)
        {
            // Not implemented for now
            return JsonResponse(new { error = "Not implemented" });
        }

        private IActionResult HandleExtract(string baseDir)
        {
            // Not implemented for now
            return JsonResponse(new { error = "Not implemented" });
        }

        private IActionResult HandleSearch(string baseDir, string baseUrl)
        {
            var q = GetRequestValue("q");
            if (string.IsNullOrEmpty(q))
                return JsonResponse(new { files = new object[0] });

            var results = new List<object>();
            SearchFiles(baseDir, q, baseDir, baseUrl, results);
            
            return JsonResponse(new { files = results });
        }

        private void SearchFiles(string searchDir, string query, string baseDir, string baseUrl, List<object> results)
        {
            try
            {
                foreach (var file in Directory.GetFiles(searchDir))
                {
                    var fileName = Path.GetFileName(file);
                    if (fileName.Contains(query, StringComparison.OrdinalIgnoreCase))
                    {
                        var ext = Path.GetExtension(file).ToLowerInvariant();
                        if (_allowedExtensions.Contains(ext))
                        {
                            results.Add(GetFileInfo(file, baseDir, baseUrl));
                        }
                    }
                }

                foreach (var dir in Directory.GetDirectories(searchDir))
                {
                    var dirName = Path.GetFileName(dir);
                    if (dirName.StartsWith(".") || dirName == "_thumbs")
                        continue;

                    SearchFiles(dir, query, baseDir, baseUrl, results);
                }
            }
            catch
            {
                // Ignore errors
            }
        }
    }
}

