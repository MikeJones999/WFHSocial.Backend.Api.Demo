namespace WFHSocial.Shared.FileUploads
{
    public class UploadResult
    {
        public bool IsSuccessful { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public string? FileName { get; set; }
        public string? StoredFileNamed { get; set; }
    }
}
