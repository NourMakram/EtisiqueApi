using EcommercePro.Models;
using EtisiqueApi.Models;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface IRequestImage
    {
        public Task<(bool Succeeded, string[] Errors)> Add(List<IFormFile> files, int typeService, int requestId, string Locations);
        public List<string> GetImages(int requestId,int typeService);
        public Task<(bool Succeeded, string[] Errors)> Add(List<IFormFile> files, int ManangmentId, string Locations);
    }
    public class RequestImage : IRequestImage
    {
        private Context _dbcontext;
        private IFileService _fileService;
        private IHttpContextAccessor _contextAccessor;

        public RequestImage(Context dbcontext, IFileService fileService, IHttpContextAccessor contextAccessor)
        {
            _dbcontext = dbcontext;
            _fileService = fileService;
            _contextAccessor = contextAccessor;
        }
        public async Task<(bool Succeeded, string[] Errors)> Add(List<IFormFile> files, int typeService, int requestId,string Locations)
        {
            try
            {
                var context = _contextAccessor.HttpContext.Request;
                var baseurl = context.Scheme + "://" + context.Host;

                List<RequestsImages> images = new List<RequestsImages>();
                foreach (IFormFile file in files)
                {
                    var result = await _fileService.SaveImage2(Locations, file,baseurl);
                    if (result.Succeeded)
                    {
                        images.Add(new RequestsImages()
                        {
                            RequestId = requestId,
                            imagepath = result.imagePath,
                            ServiceType = typeService
                        });
                    }
                    else
                    {
                        return (false, result.Errors);

                    }

                }
                _dbcontext.RequestsImages.AddRange(images);
                _dbcontext.SaveChanges();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false,new string[] { ex.Message });
            }

        }

        public async Task<(bool Succeeded, string[] Errors)> Add(List<IFormFile> files, int ManangmentId, string Locations)
        {
            try
            {
                var context = _contextAccessor.HttpContext.Request;
                var baseurl = context.Scheme + "://" + context.Host;

                List<Attchments> images = new List<Attchments>();
                foreach (IFormFile file in files)
                {
                    var result = await _fileService.SaveImage2(Locations, file, baseurl);
                    if (result.Succeeded)
                    {
                        images.Add(new Attchments()
                        {
                            RequestManagementId=ManangmentId,
                            imagePath=result.imagePath
                        });
                    }
                    else
                    {
                        return (false, result.Errors);

                    }

                }
                _dbcontext.Attchments.AddRange(images);
                _dbcontext.SaveChanges();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new string[] { ex.Message });
            }

        }

        public List<string> GetImages(int requestId, int typeService)
        {
            return _dbcontext.RequestsImages.Where(I => I.RequestId == requestId && I.ServiceType == typeService).
                Select(n => n.imagepath).ToList();
        }
    }
}
