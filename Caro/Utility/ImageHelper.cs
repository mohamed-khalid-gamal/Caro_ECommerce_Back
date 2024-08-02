namespace Caro.Utility
{
    public class ImageHelper
    {
        public static (string MimeType, string FileName, IFormFile FormFile) GetImageDetailsFromBase64(string imageDataURL, string fileNamePrefix = "image")
        {
            if (string.IsNullOrEmpty(imageDataURL))
            {
                return (null, null, null);
            }

            // Extract MIME type
            var dataParts = imageDataURL.Split(';');
            string mimeType = dataParts[0].Substring(5); // Get "image/png"

            // Get the file extension
            string extension = mimeType.Split('/')[1]; // Get "png"

            // Generate a logical file name
            string fileName = $"{fileNamePrefix}.{extension}";

            // Extract the base64 encoded data
            var base64Data = dataParts[1].Split(',')[1];
            byte[] fileBytes = Convert.FromBase64String(base64Data);

            // Create an IFormFile instance
            IFormFile formFile = new FormFileFromByteArray(fileBytes, fileName, mimeType);

            return (mimeType, fileName, formFile);
        }
    }

}
