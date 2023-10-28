using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LicenseUploaderWebApp.Pages;

[DisableRequestSizeLimit]
[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
	private readonly ILogger<IndexModel> _logger;
	private readonly IConfiguration _configuration;

	public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
	{
		_logger = logger;
		_configuration = configuration;
	}

	public async Task<IActionResult> OnPostUploadAsync(IFormFile file)
	{
		if (file == null
			|| file.Length == 0)
		{
			return new OkResult();
		}

		_logger.LogInformation("Uploading file {FileName} ({Length} bytes)", file.FileName, file.Length);

		var section = _configuration.GetSection("Upload");
		var path = section.GetValue<string>("Path")!;

		var fileName = Path.Combine(path, file.FileName);
		var folder = Path.GetDirectoryName(fileName);
		if (!Directory.Exists(folder))
		{
			Directory.CreateDirectory(folder!);
		}

		using (var stream = new FileStream(fileName, FileMode.Create))
		{
			await file.CopyToAsync(stream);
		}

		return new OkResult();
	}
}
