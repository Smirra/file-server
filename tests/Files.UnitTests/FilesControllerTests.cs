
namespace Files.Api.UnitTests;

public class FilesControllerTests
{
    private readonly List<Guid> guidList = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];

    [Fact]
    public async Task GetFilesInformation_ReturnsOk_WithListOfInfo()
    {
        // Arrange
        var idsStr = string.Join(',', guidList);

        var expectedFileItems = GetTestFileItems();
        var expectedHttpResponseJson = GetTestHttpResponseJson();

        var mockFSRepo = new Mock<IFilesApiRepository>();
        mockFSRepo.Setup(repo => repo.GetFileItems(guidList))
                .ReturnsAsync(expectedFileItems);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, "https://localhost/api/identity/file?ids=" + idsStr)
                .Respond(HttpStatusCode.OK, new StringContent(expectedHttpResponseJson));

        var client = mockHttp.ToHttpClient();
        var httpResponseResult = await client.GetAsync("https://localhost/api/identity/file?ids=" + idsStr);
        var httpResponseJson = await httpResponseResult.Content.ReadAsStringAsync();

        var mockIdentityHttpClientSvc = new Mock<IIdentityHttpClientService>();
        mockIdentityHttpClientSvc.Setup(svc => svc.GetFiles(idsStr))
            .ReturnsAsync(httpResponseResult);

        var controller = new FilesController(null!, mockFSRepo.Object, mockIdentityHttpClientSvc.Object);

        // Act
        var actionResult = await controller.GetFilesInformation(string.Join(',', guidList));

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnValue = Assert.IsType<List<FileItem>>(okResult.Value);

        Assert.Equal(expectedFileItems, returnValue);
        Assert.Equal(expectedHttpResponseJson, httpResponseJson);
    }

    private List<FileItem> GetTestFileItems()
    {
        var fileItems = new List<FileItem>
        {
            new() {
                Id = guidList[0],
                Name = "test1.txt",
                // RandomFileName = "d2v4od1z.fbo",
                ContentType = "text/plain",
                Size = 1000000,
                LastModifiedAt = new DateTime(2021, 10, 2, 15, 31, 1, DateTimeKind.Utc),
                UploadedAt = new DateTime(2019, 10, 2, 15, 31, 1, DateTimeKind.Utc),
                CheckSum = "2d74b80235513d5cf8e823ca7d7408c4",
            },
            new() {
                Id = guidList[1],
                Name = "test2.pdf",
                // RandomFileName = "d2erwd1z.fde",
                ContentType = "application/pdf",
                Size = 2000000,
                LastModifiedAt = new DateTime(2021, 9, 2, 15, 31, 1, DateTimeKind.Utc),
                UploadedAt = new DateTime(2019, 3, 2, 12, 31, 1, DateTimeKind.Utc),
                CheckSum = "2ab634bebd100fbddc3c3fee9b8eff29",
            },
            new() {
                Id = guidList[2],
                Name = "test3.png",
                // RandomFileName = "5nthvh1o.id5",
                ContentType = "image/png",
                Size = 3000000,
                LastModifiedAt = new DateTime(2002, 9, 2, 15, 31, 1, DateTimeKind.Utc),
                UploadedAt = new DateTime(2018, 3, 2, 12, 31, 1, DateTimeKind.Utc),
                CheckSum = "fc161da24963ae1f2f223b8e35ddcc47",
            }
        };

        return fileItems;
    }

    private string GetTestHttpResponseJson()
    {
        var fileRspDto = new FilesController.FileRspDto
        {
            FileIds = guidList,
            FileIdsNotFound = null
        };

        return JsonSerializer.Serialize(fileRspDto);
    }
}