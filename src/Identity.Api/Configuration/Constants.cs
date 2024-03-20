
namespace Identity.Api.Configuration;

public static class Constants
{

    // Roles
    public const string AdminRole = "Admin";
    public const string UserRole = "User";
    public const string ViewerRole = "Viewer";

    // Resource Roles
    public const string FileOwnerRole = "FileOwner";
    public const string FileEditorRole = "FileEditor";
    public const string FileViewerRole = "FileViewer";

    // Allowed Operation requirements

    public const string CreateOperation = "Create";
    public const string ReadOperation = "Read";
    public const string UpdateOperation = "Update";
    public const string DeleteOperation = "Delete";
    public const string ShareOperation = "Share";

    // Policy names
    public const string FileOperation = "FileOperation";
    public const string CanCreateFile = "CanCreateFile";
    public const string IsFileOwner = "IsFileOwner";
    public const string IsFilesApi = "IsFilesApi";

    public static class AllowedOperations
    {
        public static string Create { get; } = CreateOperation;
        public static string Read { get; } = ReadOperation;
        public static string Update { get; } = UpdateOperation;
        public static string Delete { get; } = DeleteOperation;
        public static string Share { get; } = ShareOperation;
    }
}