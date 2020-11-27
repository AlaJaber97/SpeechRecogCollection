using BLL.Sql.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UploadFile
    {
        public static async Task<(HttpStatusCode statusCode, string message)> UploadAudioAsync(Guid id, string token, Stream Audio, string fileName)
        {
            try
            {
                StreamContent fileContent = new StreamContent(Audio);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") 
                { Name = "file", FileName = fileName };
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    content.Add(fileContent);
                    var response = await new BLL.Services.HttpExtension<AudioFile>().UploadFileReturnStatusCodeAndString($"TransDatas/audio/{id}", content, token);
                    return (response.statusCode, response.message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
