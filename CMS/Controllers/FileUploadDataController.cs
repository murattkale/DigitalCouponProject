using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public partial class FileUploadDataController : Controller
{
    public IWebHostEnvironment _IWebHostEnvironment { get; set; }
    static readonly string foldersToCopy = "fileupload/UserFiles/Folders";


    public FileUploadDataController(IWebHostEnvironment _IWebHostEnvironment)
    {
        this._IWebHostEnvironment = _IWebHostEnvironment;
    }

    public ActionResult CustomDropZone()
    {
        return View();
    }

    public async Task<ActionResult> CustomDropZone_Save(IEnumerable<IFormFile> files, string filigranImagePath, string filigranText, int? PartnerId)
    {
        //if (!string.IsNullOrEmpty(filigranImagePath))
        //    filigranImagePath = Path.Combine(_IWebHostEnvironment.WebRootPath, foldersToCopy, filigranImagePath);

        var IdName = PartnerId.ToStr();

        List<PartnerDocument> DocList = new List<PartnerDocument>();
        if (files != null)
        {
            foreach (var file in HttpContext.Request.Form.Files)
            {
                var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                var fileName = fileContent.FileName.clearFileName("_", IdName);

                var physicalPath = Path.Combine(_IWebHostEnvironment.WebRootPath, foldersToCopy, fileName);


                var row = new PartnerDocument();
                row.FileSize = file.Length.ToStr();
                row.DocType = file.ContentType.ToStr().Contains("svg") ||  file.ContentType.ToStr().Contains("jpg") || file.ContentType.ToStr().Contains("png") || file.ContentType.ToStr().Contains("ico") ? DocType.Image : DocType.Document;

                if (file.Length > (1024 * 1024 * HttpContext.Session.Get<SiteConfig>("config").MaxImageSize))
                {
                    row.FileSize = "Max size : " + fileName + "  - " + file.Length / (1024) + " KB.";
                }
                else
                {
                    row.FileSize = file.Length / (1024) + " KB.";
                }

                row.Link = fileName;
                DocList.Add(row);

                if (row.FileSize.Contains("Max"))
                    continue;

                if (row.DocType == DocType.Document)
                {
                    using (Stream fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    //FileStream fs = new FileStream(file.FilePath, FileMode.Open, FileAccess.Read);
                    //byte[] ImageData = new byte[fs.Length];
                    //fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));
                    //fs.Close();
                }
                else if (row.DocType == DocType.Image)
                {
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

                    int destWidth = (int)(sourceWidth * nPercent);
                    int destHeight = (int)(sourceHeight * nPercent);


                    image.Mutate(x => x.Resize(destWidth, destHeight));
                    image.Save(physicalPath);

                    //ISupportedImageFormat format = new JpegFormat { Quality = 72 };
                    //Size size = new Size(1200, 0);
                    //using (ImageFactory imageFactory = new ImageFactory())
                    //    imageFactory.Load(stream).Resize(size).Format(format)
                    //        //.Watermark(new ImageProcessor.Imaging.TextLayer() { Text = "test pilz", DropShadow = true })
                    //        .Save(physicalPath);
                }


            }
        }

        return Json(DocList);
    }

    public ActionResult CustomDropZone_Remove(string[] fileNames)
    {
        // The parameter of the Remove action must be called "fileNames"

        if (fileNames != null)
        {
            foreach (var fullName in fileNames)
            {
                var fileName = Path.GetFileName(fullName);
                var physicalPath = Path.Combine(_IWebHostEnvironment.WebRootPath, foldersToCopy, fileName);

                // TODO: Verify user permissions

                if (System.IO.File.Exists(physicalPath))
                {
                    // The files are not actually removed in this demo
                    System.IO.File.Delete(physicalPath);
                }
            }
        }

        // Return an empty string to signify success
        return Json(fileNames);
    }



}
