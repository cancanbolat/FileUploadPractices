using ApiMultipleUploadDb.Data;
using ApiMultipleUploadDb.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ApiMultipleUploadDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger<ImageController> logger;
        private readonly IMediator mediator;

        public ImageController(ApplicationDbContext dbContext, 
            IHostingEnvironment hostingEnvironment, 
            ILogger<ImageController> logger,
            IMediator mediator)
        {
            this.dbContext = dbContext;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpPost("[action]")]
        public async Task<string> Upload1()
        {
            try
            {
                var files = HttpContext.Request.Form.Files;

                if (files is not null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file.FileName);

                        var newFileName = "Image-" + DateTime.Now.TimeOfDay.Milliseconds + fileInfo.Extension;
                        var path = Path.Combine("", hostingEnvironment.ContentRootPath + "\\Images\\" + newFileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        ImageUpload imageUpload = new ImageUpload();
                        imageUpload.ImagePath = path;
                        imageUpload.InsertedDate = DateTime.Now;

                        dbContext.Images.Add(imageUpload);
                        await dbContext.SaveChangesAsync();
                    }
                    return "Saved successfully";
                }
                else
                {
                    return "Error";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Upload2([FromForm(Name = "files")] IEnumerable<IFormFile> files)
        {
            var maxAllowedFiles = 4;
            long maxFileSize = 1024 * 1024 * 15;
            var filesProcessed = 0;
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            List<UploadResult> uploadResults = new();

            foreach (IFormFile file in files)
            {
                FileInfo fileInfo = new FileInfo(file.FileName);
                UploadResult uploadResult = new UploadResult();

                if (filesProcessed < maxAllowedFiles)
                {
                    if (file.Length == 0)
                    {
                        uploadResult.Error = $"{file.FileName} length is 0";
                    }
                    else if (file.Length > maxFileSize)
                    {
                        uploadResult.Error = $"{file.FileName} of {file.Length} bytes is " +
                            $"larger than the limit of {maxFileSize} bytes.";
                    }
                    else
                    {
                        try
                        {
                            var newFileName = "Image-" + DateTime.Now.TimeOfDay.Milliseconds + fileInfo.Extension;
                            var path = Path.Combine("", hostingEnvironment.ContentRootPath + "\\Images\\" + newFileName);

                            await using var stream = new FileStream(path, FileMode.Create);
                            await file.CopyToAsync(stream);

                            //Results
                            var displayPath = WebUtility.HtmlEncode(path);
                            uploadResult.Uploaded = true;
                            uploadResult.FileName = newFileName;
                            uploadResult.StoredFileName = displayPath;

                            //Database
                            ImageUpload imageUpload = new ImageUpload();
                            imageUpload.ImagePath = path;
                            imageUpload.InsertedDate = DateTime.Now;
                            dbContext.Images.Add(imageUpload);
                            await dbContext.SaveChangesAsync();

                        }
                        catch (IOException ex)
                        {
                            uploadResult.Error = $"{file.FileName} error on upload: {ex.Message}";
                        }
                    }
                    uploadResults.Add(uploadResult);
                }

                filesProcessed++;
            }

            return new CreatedResult(resourcePath, uploadResults);
        }

    }
}