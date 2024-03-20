
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Models;

[PrimaryKey(nameof(UserId), nameof(FileId))]
[Index(nameof(FileId))]
    public class UserFile
    {
        public Guid UserId { get; set; }
        public Guid FileId { get; set; }
        public Guid ResourceRoleId { get; set; }
        public bool CanShare { get; set; }

        [JsonIgnore]
        public User User { get; set; } = null!;
        [JsonIgnore]
        public ResourceRole ResourceRole { get; set; } = null!;
    }