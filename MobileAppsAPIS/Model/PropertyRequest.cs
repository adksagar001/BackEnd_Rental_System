// PropertyRequest.cs

namespace MobileAppsAPIS.Model
{
    public class PropertyRequest
    {
        public Property Property { get; set; }
        public List<ImageData> Images { get; set; }
    }

    public class ImageData
    {
        public string Base64Data { get; set; }
        public string FileName { get; set; }

        internal async Task CopyToAsync(FileStream fileStream)
        {
            throw new NotImplementedException();
        }
    }

    public class PropertyApproval
    {
        public int PropertyId { get; set; }
        public string Status { get; set; }
        public int Active { get; set; }
    }
}