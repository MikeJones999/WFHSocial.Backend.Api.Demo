using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WFHSocial.Shared.FileUploads;

namespace WFHSocial.Api.Controllers.FileUploadControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly ILogger<FileUploadController> _logger;
        private readonly IWebHostEnvironment _env;

        public FileUploadController(ILogger<FileUploadController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        [HttpPost]
        public async Task<ActionResult<List<UploadResult>>> UploadFile(List<IFormFile> files)
        {
            List<UploadResult> uploadResults = new List<UploadResult>();

            foreach (IFormFile file in files)
            {
                var uploadResult = new UploadResult();
                string trustedFileNameForFileStorage;
                string untrustedFileName = file.FileName; //original file name
                uploadResult.FileName = untrustedFileName;
                var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

                trustedFileNameForFileStorage = Path.GetRandomFileName();
                var path = Path.Combine(_env.ContentRootPath, "UploadedFiles", trustedFileNameForFileStorage); //uploads must exist //config

                await using FileStream fs = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(fs);

                uploadResult.StoredFileNamed = trustedFileNameForDisplay;

                uploadResults.Add(uploadResult);
            }

            return Ok(uploadResults);
        }
    }
}
