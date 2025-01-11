namespace EtisiqueApi.Repositiories.Interfaces
{
	public interface IFileService
	{
		public Task<(bool Succeeded, string[] Errors , string imagePath)> SaveImage(string Location,  IFormFile imageFile);
		public (bool Succeeded, string[] Errors) DeleteImage(string Location , string imageFileName);
        public Task<(bool Succeeded, string[] Errors, string imagePath)> SaveImage2(string Location, IFormFile imageFile, string baseurl);

        public Task<(bool Succeeded, string[] Errors, string imagePath)> ChangeImage(string Location, IFormFile imageFile, string OldImage);
	}
}
