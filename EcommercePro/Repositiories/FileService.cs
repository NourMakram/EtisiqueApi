using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Repositiories.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace EtisiqueApi.Repositiories
{
	public class FileService : IFileService
	{
		private IWebHostEnvironment environment;
		private IHttpContextAccessor _contextAccessor;
		 


        public FileService(IWebHostEnvironment env,IHttpContextAccessor contextAccessor
)
		{
			environment = env;
			_contextAccessor = contextAccessor;
         
        }

		public  async Task<(bool Succeeded, string[] Errors, string imagePath)> ChangeImage(string Location, IFormFile imageFile, string OldImage)
		{
			var resultimage = await SaveImage(Location, imageFile);
			
			if (!resultimage.Succeeded)
			{
				return(false ,resultimage.Errors , "");
			}

			//if(OldImage != null )
			//{
			//	var Result = DeleteImage(Location ,OldImage);
			//	if (!Result.Succeeded)
			//	{
			//		return (false, Result.Errors, "");
			//	}
			//}
			var imagepath =resultimage.imagePath;
			return (true ,new string[] { }, imagepath);

		}

		public (bool Succeeded, string[] Errors) DeleteImage(string Location,string imageFileName)
		{
			try
			{
				var ContentPath = environment.WebRootPath ;
                var context = _contextAccessor.HttpContext.Request;
                var baseurl = context.Scheme + "://" + context.Host;
				var LastSlahIndex = imageFileName.LastIndexOf("/");
				var ImageUrl = imageFileName.Substring(LastSlahIndex+1);


                var path = ContentPath +"/" + Location +"/"+ ImageUrl;
				if (File.Exists(path))
				{
					File.Delete(path);
					return (true, new string[] { });
				}
				return (false, new string[]{"The Image Not Deleted"});
			}
			catch (Exception ex)
			{
				return (false, new string[] { ex.Message });

			}
		}

		public async Task<(bool Succeeded, string[] Errors, string imagePath)> SaveImage(string Location, IFormFile imageFile)
		{
			try { 

			var ContentPath = environment.WebRootPath + "/" + Location;
			var Extention = Path.GetExtension(imageFile.FileName);
            var context = _contextAccessor.HttpContext.Request;
            var baseurl = context.Scheme + "://" + context.Host;

                //var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg", ".pdf" };

                //	if (!allowedExtensions.Contains(Extention))
                //{
                //	string msg = string.Format("Only {0} extensions are allowed", string.Join(",", allowedExtensions));
                //	return (false, new string[] { msg }, "");
                //}
                var FileName = Guid.NewGuid().ToString().Replace("-", string.Empty) + Extention;

			if (!Directory.Exists(ContentPath))
			{
				Directory.CreateDirectory(ContentPath);

			}
			using (FileStream fileStream = File.Create(ContentPath +"/"+ FileName))
			{
				await imageFile.CopyToAsync(fileStream);
				await fileStream.FlushAsync();
				var imagePath = $"/be/{Location}/{FileName}";
				return (true, new string[] { }, baseurl+imagePath);
			}
		}

			catch (Exception ex)
			{
				return (false, new string[] { ex.Message },"");

			}


		}
        public async Task<(bool Succeeded, string[] Errors, string imagePath)> SaveImage2(string Location, IFormFile imageFile, string baseurl)
        {
            try
            {

                var ContentPath = environment.WebRootPath + "/" + Location;
                var Extention = Path.GetExtension(imageFile.FileName);
                
                var FileName = Guid.NewGuid().ToString().Replace("-", string.Empty) + Extention;

                if (!Directory.Exists(ContentPath))
                {
                    Directory.CreateDirectory(ContentPath);

                }
                using (FileStream fileStream = File.Create(ContentPath + "/" + FileName))
                {
                    await imageFile.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                    var imagePath = $"/be/{Location}/{FileName}";
                    return (true, new string[] { }, baseurl + imagePath);
                }
            }

            catch (Exception ex)
            {
                return (false, new string[] { ex.Message }, "");

            }


        }
    }
}
