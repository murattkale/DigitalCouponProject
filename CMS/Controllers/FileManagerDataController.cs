using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class FileManagerDataController : Controller
    {
        IWebHostEnvironment _IWebHostEnvironment { get; set; }
        protected readonly IHostingEnvironment HostingEnvironment;
        private readonly FileContentBrowser directoryBrowser;
        //
        // GET: /FileManager/
        private const string contentFolderRoot = "fileupload";
        private const string prettyName = "Folders";
        private static readonly string[] foldersToCopy = new[] { "fileupload/filemanager" };
        //string FileRoot = "fileupload/UserFiles/Folders";


        public FileManagerDataController(IHostingEnvironment hostingEnvironment, IWebHostEnvironment _IWebHostEnvironment, IHttpClientWrapper client)
        {
            this.directoryBrowser = new FileContentBrowser();
            this.HostingEnvironment = hostingEnvironment;
            this._IWebHostEnvironment = _IWebHostEnvironment;
            _client = client;
        }

        IHttpClientWrapper _client;

        public ActionResult OnPostSave(IEnumerable<IFormFile> files, int Id, string type)
        {
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    var virtualPath = Path.Combine(contentFolderRoot, "UserFiles", prettyName);
                    var path = Path.GetFullPath(Path.Combine(this.HostingEnvironment.WebRootPath, virtualPath));

                    var fileName = SaveFile(file, path);
                    if (type == "PartnerDocument")
                    {
                        var resultSave = _client.Post<PartnerDocument>(new PartnerDocument().GetType().Name + "/InsertOrUpdate", new PartnerDocument()
                        {
                            PartnerId = Id,
                            Link = fileName,
                            Name = fileName,
                            DocType = file.ContentType.Contains("image") ? DocType.Image : DocType.Document,
                            FileSize = file.Length.ToStr(),
                            Desc = fileName,
                        });
                    }
                    else
                    {
                        var result = _client.Post<Documents>(new Documents().GetType().Name + "/InsertOrUpdate",
                             new Documents()
                             {
                                 GalleryId = type == "Gallery" ? Id : null,
                                 DocumentId = type == "Documents" ? Id : null,
                                 ThumbImageId = type == "ThumbImage" ? Id : null,
                                 PictureId = type == "Picture" ? Id : null,
                                 BannerImageId = type == "BannerImage" ? Id : null,
                                 Link = fileName,
                                 Name = fileName,

                             }
                            );

                    }

                    // The files are not actually saved in this demo
                    //file.SaveAs(physicalPath);
                }
            }

            // Return an empty string to signify success
            return Json(Id);
        }

        //public async Task<ActionResult> SaveAsync(IEnumerable<IFormFile> files, int Id)
        //{
        //    var virtualPath = Path.Combine(contentFolderRoot, "UserFiles", prettyName);
        //    var path = Path.GetFullPath(Path.Combine(this.HostingEnvironment.WebRootPath, virtualPath));
        //    foreach (var file in files)
        //    {
        //        FileManagerEntry newEntry;
        //        path = NormalizePath(path);
        //        if (AuthorizeUpload(path, file))
        //        {
        //            var fileName = SaveFile(file, path);
        //            newEntry = directoryBrowser.GetFile(Path.Combine(path, fileName));

        //            //return Json(VirtualizePath(newEntry));
        //        }

        //    }
        //    return Json("ok");
        //}


        public virtual JsonResult Read(string target)
        {
            var path = NormalizePath(target);

            if (Authorize(path))
            {
                try
                {
                    var files = directoryBrowser.GetFiles(path, Filter);
                    var directories = directoryBrowser.GetDirectories(path);
                    var result = files.Concat(directories).Select(VirtualizePath);

                    return Json(result.ToArray());
                }
                catch (DirectoryNotFoundException)
                {
                    throw new Exception("File Not Found");
                }
            }

            throw new Exception("Forbidden");
        }


        string SaveFile(IFormFile file, string pathToSave)
        {
            try
            {
                var IdName = file.FileName;

                var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                var fileName = fileContent.FileName.clearFileName("_", "");
                var physicalPath = Path.Combine(_IWebHostEnvironment.WebRootPath, pathToSave, fileName);

                if (file.ContentType == "image/svg+xml")
                {
                    using (Stream fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    return fileName;
                    //var image = Image.Load(file.OpenReadStream());

                }
                else if (file.ContentType.Contains("image"))
                {

                }
                else
                {
                    using (Stream fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    return fileName;
                }
                //var stream = file.OpenReadStream();

                var image = Image.Load(file.OpenReadStream());

                var newWidth = image.Width;
                var newHeight = image.Height;

                int sourceWidth = image.Width;
                int sourceHeight = image.Height;

                //Consider vertical pics
                if (sourceWidth < sourceHeight)
                {
                    int buff = newWidth;

                    newWidth = newHeight;
                    newHeight = buff;
                }

                int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
                float nPercent = 0, nPercentW = 0, nPercentH = 0;

                nPercentW = ((float)newWidth / (float)sourceWidth);
                nPercentH = ((float)newHeight / (float)sourceHeight);
                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentH;
                    destX = System.Convert.ToInt16((newWidth -
                              (sourceWidth * nPercent)) / 2);
                }
                else
                {
                    nPercent = nPercentW;
                    destY = System.Convert.ToInt16((newHeight -
                              (sourceHeight * nPercent)) / 2);
                }

                var destWidth = (sourceWidth * nPercent).ToInt();
                var destHeight = (sourceHeight * nPercent).ToInt();


                image.Mutate(x => x.Resize(destWidth, destHeight));
                image.Save(physicalPath);



                //ISupportedImageFormat format = new JpegFormat { Quality = 72 };
                //Size size = new Size(1200, 0);
                //using (ImageFactory imageFactory = new ImageFactory())
                //    imageFactory.Load(stream).Resize(size).Format(format)
                //        //.Watermark(new ImageProcessor.Imaging.TextLayer() { Text = "test pilz", DropShadow = true })
                //        .Save(physicalPath);








                //.Resize(physicalPath, "", filigranText);
                //res.Dispose();
                //if (!string.IsNullOrEmpty(filigranImagePath))
                //{
                //    filigranImagePath = Path.Combine(_IWebHostEnvironment.WebRootPath, "images", filigranImagePath);
                //    var fImage = new FileStream(filigranImagePath, FileMode.Open);
                //    try
                //    {
                //        var resFilig = fImage.Resize(physicalPath, filigranImagePath, filigranText);
                //        resFilig.Dispose();
                //        fImage.Dispose();
                //    }
                //    catch (Exception ex)
                //    {
                //        fImage.Dispose();
                //    }
                //}




                return fileName;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Gets the base paths from which content will be served.
        /// </summary>
        public string ContentPath
        {
            get
            {
                return CreateUserFolder();
            }
        }
        public IActionResult FileManager()
        {
            return View();
        }


        /// <summary>
        /// Gets the valid file extensions by which served files will be filtered.
        /// </summary>
        public string Filter
        {
            get
            {
                return "*.*";
            }
        }

        private string CreateUserFolder()
        {
            var virtualPath = Path.Combine(contentFolderRoot, "UserFiles", prettyName);
            var path = Path.GetFullPath(Path.Combine(this.HostingEnvironment.WebRootPath, virtualPath));
            //var path = HostingEnvironment.WebRootFileProvider.GetFileInfo(virtualPath).PhysicalPath;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                foreach (var sourceFolder in foldersToCopy)
                {
                    CopyFolder(HostingEnvironment.WebRootFileProvider.GetFileInfo(sourceFolder).PhysicalPath, path);
                }
            }
            return virtualPath;
        }

        private void CopyFolder(string source, string destination)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            foreach (var file in Directory.EnumerateFiles(source))
            {

                var dest = Path.Combine(destination, Path.GetFileName(file));
                System.IO.File.Copy(file, dest);
            }

            foreach (var folder in Directory.EnumerateDirectories(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(folder));
                CopyFolder(folder, dest);
            }
        }

        /// <summary>
        /// Determines if content of a given path can be browsed.
        /// </summary>
        /// <param name="path">The path which will be browsed.</param>
        /// <returns>true if browsing is allowed, otherwise false.</returns>
        public virtual bool Authorize(string path)
        {
            return CanAccess(path);
        }

        protected virtual bool CanAccess(string path)
        {
            var rootPath = Path.GetFullPath(Path.Combine(this.HostingEnvironment.WebRootPath, ContentPath));
            return path.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase);
        }

        protected virtual string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return Path.GetFullPath(Path.Combine(this.HostingEnvironment.WebRootPath, ContentPath));
            }
            else
            {
                return Path.GetFullPath(Path.Combine(this.HostingEnvironment.WebRootPath, ContentPath, path));
            }
        }

        protected virtual FileManagerEntry VirtualizePath(FileManagerEntry entry)
        {
            entry.Path = entry.Path.Replace(Path.Combine(this.HostingEnvironment.WebRootPath, ContentPath), "").Replace(@"\", "/").TrimStart('/');
            return entry;
        }

        public virtual ActionResult Create(string target, FileManagerEntry entry)
        {
            FileManagerEntry newEntry;

            if (!Authorize(NormalizePath(target)))
            {
                throw new Exception("Forbidden");
            }


            if (String.IsNullOrEmpty(entry.Path))
            {
                newEntry = CreateNewFolder(target, entry);
            }
            else
            {
                newEntry = CopyEntry(target, entry);
            }

            return Json(VirtualizePath(newEntry));
        }

        protected virtual FileManagerEntry CopyEntry(string target, FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);
            var physicalPath = path;
            var physicalTarget = EnsureUniqueName(NormalizePath(target), entry);

            FileManagerEntry newEntry;

            if (entry.IsDirectory)
            {
                CopyDirectory(new DirectoryInfo(physicalPath), Directory.CreateDirectory(physicalTarget));
                newEntry = directoryBrowser.GetDirectory(physicalTarget);
            }
            else
            {
                System.IO.File.Copy(physicalPath, physicalTarget);
                newEntry = directoryBrowser.GetFile(physicalTarget);
            }



            return newEntry;
        }

        protected virtual void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (FileInfo fi in source.GetFiles())
            {

                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectory(diSourceSubDir, nextTargetSubDir);
            }
        }

        protected virtual FileManagerEntry CreateNewFolder(string target, FileManagerEntry entry)
        {
            FileManagerEntry newEntry;
            var path = NormalizePath(target);
            string physicalPath = EnsureUniqueName(path, entry);

            Directory.CreateDirectory(physicalPath);

            newEntry = directoryBrowser.GetDirectory(physicalPath);

            return newEntry;
        }

        protected virtual string EnsureUniqueName(string target, FileManagerEntry entry)
        {
            entry.Name = entry.Name;
            var tempName = entry.Name + entry.Extension;
            int sequence = 0;
            var physicalTarget = Path.Combine(NormalizePath(target), tempName);

            if (!Authorize(NormalizePath(physicalTarget)))
            {
                throw new Exception("Forbidden");
            }

            if (entry.IsDirectory)
            {
                while (Directory.Exists(physicalTarget))
                {
                    tempName = entry.Name + String.Format("({0})", ++sequence);
                    physicalTarget = Path.Combine(NormalizePath(target), tempName);
                }
            }
            else
            {
                while (System.IO.File.Exists(physicalTarget))
                {
                    tempName = entry.Name + String.Format("({0})", ++sequence) + entry.Extension;
                    physicalTarget = Path.Combine(NormalizePath(target), tempName);
                }
            }

            return physicalTarget;
        }

        public virtual ActionResult Destroy(FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);



            if (!string.IsNullOrEmpty(path))
            {
                if (entry.IsDirectory)
                {
                    DeleteDirectory(path);
                }
                else
                {
                    DeleteFile(path);
                }

                return Json(new object[0]);
            }
            throw new Exception("File Not Found");
        }

        protected virtual void DeleteFile(string path)
        {
            if (!Authorize(path))
            {
                throw new Exception("Forbidden");
            }

            var physicalPath = NormalizePath(path);

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
        }

        protected virtual void DeleteDirectory(string path)
        {
            if (!Authorize(path))
            {
                throw new Exception("Forbidden");
            }

            var physicalPath = NormalizePath(path);

            if (Directory.Exists(physicalPath))
            {
                Directory.Delete(physicalPath, true);
            }
        }


        /// <summary>
        /// Updates an entry with a given entry.
        /// </summary>
        /// <param name="path">The path to the parent folder in which the folder should be created.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>An empty <see cref="ContentResult"/>.</returns>
        /// <exception cref="HttpException">Forbidden</exception>
        public virtual ActionResult Update(string target, FileManagerEntry entry)
        {
            FileManagerEntry newEntry;

            if (!Authorize(NormalizePath(entry.Path)) && !Authorize(NormalizePath(target)))
            {
                throw new Exception("Forbidden");
            }

            newEntry = RenameEntry(entry);


            return Json(VirtualizePath(newEntry));
        }

        protected virtual FileManagerEntry RenameEntry(FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);
            var physicalPath = path;
            var physicalTarget = EnsureUniqueName(Path.GetDirectoryName(path), entry);
            FileManagerEntry newEntry;

            if (entry.IsDirectory)
            {
                Directory.Move(physicalPath, physicalTarget);
                newEntry = directoryBrowser.GetDirectory(physicalTarget);
            }
            else
            {
                var file = new FileInfo(physicalPath);
                System.IO.File.Move(file.FullName, physicalTarget);
                newEntry = directoryBrowser.GetFile(physicalTarget);
            }

            return newEntry;
        }

        /// <summary>
        /// Determines if a file can be uploaded to a given path.
        /// </summary>
        /// <param name="path">The path to which the file should be uploaded.</param>
        /// <param name="file">The file which should be uploaded.</param>
        /// <returns>true if the upload is allowed, otherwise false.</returns>
        public virtual bool AuthorizeUpload(string path, IFormFile file)
        {
            if (!CanAccess(path))
            {
                throw new DirectoryNotFoundException(String.Format("The specified path cannot be found - {0}", path));
            }

            if (!IsValidFile(GetFileName(file)))
            {
                throw new InvalidDataException(String.Format("The type of file is not allowed. Only {0} extensions are allowed.", Filter));
            }

            return true;
        }

        private bool IsValidFile(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var allowedExtensions = Filter.Split(',');

            return allowedExtensions.Any(e => e.Equals("*.*") || e.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Uploads a file to a given path.
        /// </summary>
        /// <param name="path">The path to which the file should be uploaded.</param>
        /// <param name="file">The file which should be uploaded.</param>
        /// <returns>A <see cref="JsonResult"/> containing the uploaded file's size and name.</returns>
        /// <exception cref="HttpException">Forbidden</exception>
        [AcceptVerbs("POST")]
        public virtual ActionResult Upload(string path, IFormFile file)
        {
            FileManagerEntry newEntry;
            path = NormalizePath(path);
            if (AuthorizeUpload(path, file))
            {
                var fileName = SaveFile(file, path);
                newEntry = directoryBrowser.GetFile(Path.Combine(path, fileName));

                return Json(VirtualizePath(newEntry));
            }

            throw new Exception("Forbidden");
        }


        public virtual string GetFileName(IFormFile file)
        {
            var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            return fileContent.FileName;
        }

        //[AcceptVerbs("GET")]
        //public FileResult Download(string path)
        //{
        //    var virtualPath = "~/Content/" + path;
        //    var filePath = HostingEnvironment.MapPath(virtualPath);
        //    FileInfo file = new FileInfo(filePath);

        //    System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
        //    {
        //        FileName = file.Name,
        //        Inline = false
        //    };
        //    Response.Headers.Add("Content-Disposition", cd.ToString());
        //    Response.Headers.Add("X-Content-Type-Options", "nosniff");

        //    string contentType = MimeMapping.GetMimeMapping(file.Name);
        //    var readStream = System.IO.File.ReadAllBytes(filePath);
        //    return File(readStream, contentType);
        //}

    }

    public class FileContentBrowser
    {
#pragma warning disable CS0618 // 'IHostingEnvironment' artık kullanılmıyor: 'This type is obsolete and will be removed in a future version. The recommended alternative is Microsoft.AspNetCore.Hosting.IWebHostEnvironment.'
        public virtual IHostingEnvironment HostingEnvironment { get; set; }
#pragma warning restore CS0618 // 'IHostingEnvironment' artık kullanılmıyor: 'This type is obsolete and will be removed in a future version. The recommended alternative is Microsoft.AspNetCore.Hosting.IWebHostEnvironment.'
        public IEnumerable<FileManagerEntry> GetFiles(string path, string filter)
        {
            var directory = new DirectoryInfo(path);

            var extensions = (filter ?? "*").Split(new string[] { ", ", ",", "; ", ";" }, System.StringSplitOptions.RemoveEmptyEntries);

            return extensions.SelectMany(directory.GetFiles)
                .Select(file => new FileManagerEntry
                {
                    Name = Path.GetFileNameWithoutExtension(file.Name),
                    Size = file.Length,
                    Path = file.FullName,
                    Extension = file.Extension,
                    IsDirectory = false,
                    HasDirectories = false,
                    Created = file.CreationTime,
                    CreatedUtc = file.CreationTimeUtc,
                    Modified = file.LastWriteTime,
                    ModifiedUtc = file.LastWriteTimeUtc
                });
        }

        public IEnumerable<FileManagerEntry> GetDirectories(string path)
        {
            var directory = new DirectoryInfo(path);

            return directory.GetDirectories()
                .Select(subDirectory => new FileManagerEntry
                {
                    Name = subDirectory.Name,
                    Path = subDirectory.FullName,
                    Extension = subDirectory.Extension,
                    IsDirectory = true,
                    HasDirectories = subDirectory.GetDirectories().Length > 0,
                    Created = subDirectory.CreationTime,
                    CreatedUtc = subDirectory.CreationTimeUtc,
                    Modified = subDirectory.LastWriteTime,
                    ModifiedUtc = subDirectory.LastWriteTimeUtc
                });
        }

        public FileManagerEntry GetDirectory(string path)
        {
            var directory = new DirectoryInfo(path);

            return new FileManagerEntry
            {
                Name = directory.Name,
                Path = directory.FullName,
                Extension = directory.Extension,
                IsDirectory = true,
                HasDirectories = directory.GetDirectories().Length > 0,
                Created = directory.CreationTime,
                CreatedUtc = directory.CreationTimeUtc,
                Modified = directory.LastWriteTime,
                ModifiedUtc = directory.LastWriteTimeUtc
            };
        }

        public FileManagerEntry GetFile(string path)
        {
            var file = new FileInfo(path);

            return new FileManagerEntry
            {
                Name = Path.GetFileNameWithoutExtension(file.Name),
                Path = file.FullName,
                Size = file.Length,
                Extension = file.Extension,
                IsDirectory = false,
                HasDirectories = false,
                Created = file.CreationTime,
                CreatedUtc = file.CreationTimeUtc,
                Modified = file.LastWriteTime,
                ModifiedUtc = file.LastWriteTimeUtc
            };
        }


    }
}