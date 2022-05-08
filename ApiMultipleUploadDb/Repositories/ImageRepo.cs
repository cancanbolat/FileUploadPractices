using ApiMultipleUploadDb.Data;
using ApiMultipleUploadDb.Models;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiMultipleUploadDb.Repositories
{
    public class ImageRepo : GenericRepo<ImageUpload2>, IImageRepo
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IHostingEnvironment hostingEnvironment;

        public ImageRepo(ApplicationDbContext dbContext,
            IHostingEnvironment hostingEnvironment) : base(dbContext)
        {
            this.dbContext = dbContext;
            this.hostingEnvironment = hostingEnvironment;
        }

        public async Task<UploadResult> UploadImage(IEnumerable<IFormFile> formFiles)
        {
            var maxAllowedFiles = 4;
            long maxFileSize = 1024 * 1024 * 15;
            var filesProcessed = 0;
            List<UploadResult> uploadResults = new();

            foreach (var file in formFiles)
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
                            var newFileName = "imageupload-2-" + DateTime.Now.TimeOfDay.Milliseconds + fileInfo.Extension;
                            var path = Path.Combine("", hostingEnvironment.ContentRootPath + "\\Images\\" + newFileName);

                            await using var stream = new FileStream(path, FileMode.Create);
                            await file.CopyToAsync(stream);

                            //Results
                            var displayPath = WebUtility.HtmlEncode(path);
                            uploadResult.Uploaded = true;
                            uploadResult.FileName = newFileName;
                            uploadResult.StoredFileName = displayPath;

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

            return new UploadResult();
        }
    }
}
