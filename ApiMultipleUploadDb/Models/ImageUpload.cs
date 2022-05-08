using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMultipleUploadDb.Models
{
    public class ImageUpload
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public DateTime InsertedDate { get; set; }
    }
}
