using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Files.Api.Models;
public class FileItem
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public string RandomFileName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long Size { get; set; }

    public string CheckSum { get; set; } = null!;

    public DateTime UploadedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }
}