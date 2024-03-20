
namespace Files.Api.Services;

public class IdentityHttpClientService(HttpClient httpClient): IIdentityHttpClientService
{
    public async Task<HttpResponseMessage> PutFile(Guid id)
    {
        var httpResponseMessage = await httpClient.PutAsync($"file/{id}", null);

        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> PutFiles(IEnumerable<Guid> fileIds)
    {
        var httpResponseMessage = await httpClient.PutAsJsonAsync("file", fileIds);

        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> GetFiles(string? fileIds)
    {
        HttpResponseMessage httpResponseMessage;

        if (!string.IsNullOrEmpty(fileIds))
        {
            httpResponseMessage = await httpClient.GetAsync($"file?ids={fileIds}");
        }
        else
        {
            httpResponseMessage = await httpClient.GetAsync("file");
        }

        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteFiles(string fileIds)
    {
        var httpResponseMessage = await httpClient.DeleteAsync($"file?ids={fileIds}");

        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> CanCreate()
    {
        var httpResponseMessage = await httpClient.GetAsync("file/can-create");

        return httpResponseMessage;
    }

    public void Dispose() => httpClient.Dispose();
}

public interface IIdentityHttpClientService
{
    Task<HttpResponseMessage> PutFile(Guid id);
    Task<HttpResponseMessage> PutFiles(IEnumerable<Guid> fileIds);
    Task<HttpResponseMessage> GetFiles(string? fileIds);
    Task<HttpResponseMessage> DeleteFiles(string fileIds);
    Task<HttpResponseMessage> CanCreate();

    void Dispose();
}