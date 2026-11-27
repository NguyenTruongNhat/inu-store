using INuStore.Contract.Abstractions.Share;
using INuStore.UseCase.UseCases.V1.FileService;
using Microsoft.AspNetCore.Mvc;

namespace INuStore.API.Controllers;

[ApiVersion("1")]
public sealed class FilesController(FileService fileService) : ApiController
{
    [HttpPost("buffered")]
    [Consumes("multipart/form-data")] // this request only allows content-type multipart/form-data
    public async Task<IActionResult> UploadBuffered(IFormFile? file, CancellationToken cancellationToken)
    {
        var form = await Request.ReadFormAsync(cancellationToken);

        if (file is null || form.Files.Count != 1 || form.Count != 0)
        {
            return BadRequest("Send exactly one file in the 'file' field.");
        }

        await using var content = file.OpenReadStream();

        var result = await fileService.UploadAsync(content, file.FileName, file.ContentType, cancellationToken);

        return Ok(Result.Success(result));
    }
}
