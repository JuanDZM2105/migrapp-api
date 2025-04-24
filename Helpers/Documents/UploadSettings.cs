public class UploadSettings
{
  public string StoragePath { get; set; }
  public long MaxFileSizeBytes { get; set; }
  public string[] AllowedExtensions { get; set; }
}
