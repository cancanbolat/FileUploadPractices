using ApiMultipleUploadDb.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMultipleUploadDb.Repositories
{
    public interface IImageRepo : IGenericRepo<ImageUpload2>
    {
        Task<UploadResult> UploadImage(IEnumerable<IFormFile> formFiles);
    }
}
