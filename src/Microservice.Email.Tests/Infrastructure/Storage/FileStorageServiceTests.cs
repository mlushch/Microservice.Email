using System.Net;

using Microservice.Email.Core.Configuration;
using Microservice.Email.Core.Exceptions;
using Microservice.Email.Infrastructure.Storage;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Response;

namespace Microservice.Email.Tests.Infrastructure.Storage;

/// <summary>
/// Unit tests for the FileStorageService class.
/// </summary>
public sealed class FileStorageServiceTests
{
    private readonly Mock<IMinioClient> mockMinioClient;
    private readonly Mock<ILogger<FileStorageService>> mockLogger;
    private readonly IOptions<MinioSettings> minioOptions;

    public FileStorageServiceTests()
    {
        this.mockMinioClient = new Mock<IMinioClient>();
        this.mockLogger = new Mock<ILogger<FileStorageService>>();
        this.minioOptions = Options.Create(new MinioSettings
        {
            Endpoint = "localhost:9000",
            AccessKey = "test",
            SecretKey = "test",
            TemplatesBucket = "templates",
            AttachmentsBucket = "attachments"
        });
    }

    private FileStorageService CreateService()
    {
        return new FileStorageService(
            this.mockMinioClient.Object,
            this.minioOptions,
            this.mockLogger.Object);
    }

    [Fact]
    public async Task UploadAsync_WithValidStream_UploadsAndReturnsPath()
    {
        // Arrange
        var service = this.CreateService();
        var fileContent = "Test content"u8.ToArray();
        using var stream = new MemoryStream(fileContent);

        this.mockMinioClient
            .Setup(c => c.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        this.mockMinioClient
            .Setup(c => c.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutObjectResponse(HttpStatusCode.OK, "etag", new Dictionary<string, string>(), 0, ""));

        // Act
        var result = await service.UploadAsync(stream, "test.txt", "test-bucket", "text/plain");

        // Assert
        result.Should().Be("test-bucket/test.txt");

        this.mockMinioClient.Verify(
            c => c.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UploadAsync_WhenBucketDoesNotExist_CreatesBucketAndUploads()
    {
        // Arrange
        var service = this.CreateService();
        var fileContent = "Test content"u8.ToArray();
        using var stream = new MemoryStream(fileContent);

        this.mockMinioClient
            .Setup(c => c.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.mockMinioClient
            .Setup(c => c.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        this.mockMinioClient
            .Setup(c => c.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutObjectResponse(HttpStatusCode.OK, "etag", new Dictionary<string, string>(), 0, ""));

        // Act
        var result = await service.UploadAsync(stream, "test.txt", "new-bucket", "text/plain");

        // Assert
        result.Should().Be("new-bucket/test.txt");

        this.mockMinioClient.Verify(
            c => c.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UploadAsync_WhenMinioFails_ThrowsFileStorageException()
    {
        // Arrange
        var service = this.CreateService();
        var fileContent = "Test content"u8.ToArray();
        using var stream = new MemoryStream(fileContent);

        this.mockMinioClient
            .Setup(c => c.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        this.mockMinioClient
            .Setup(c => c.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("MinIO connection failed"));

        // Act
        var act = async () => await service.UploadAsync(stream, "test.txt", "test-bucket", "text/plain");

        // Assert
        await act.Should().ThrowAsync<FileStorageException>()
            .WithMessage("*Failed to upload file*");
    }

    [Fact]
    public async Task UploadAsync_WithoutContentType_UsesOctetStream()
    {
        // Arrange
        var service = this.CreateService();
        var fileContent = "Test content"u8.ToArray();
        using var stream = new MemoryStream(fileContent);

        this.mockMinioClient
            .Setup(c => c.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        this.mockMinioClient
            .Setup(c => c.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutObjectResponse(HttpStatusCode.OK, "etag", new Dictionary<string, string>(), 0, ""));

        // Act
        var result = await service.UploadAsync(stream, "binary.dat", "test-bucket");

        // Assert
        result.Should().Be("test-bucket/binary.dat");
    }

    [Fact]
    public async Task DownloadAsync_WithExistingFile_ReturnsStream()
    {
        // Arrange
        var service = this.CreateService();

        this.mockMinioClient
            .Setup(c => c.GetObjectAsync(It.IsAny<GetObjectArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(default(Minio.DataModel.ObjectStat)!));

        // Act
        var result = await service.DownloadAsync("test.txt", "test-bucket");

        // Assert
        result.Should().NotBeNull();
        this.mockMinioClient.Verify(
            c => c.GetObjectAsync(It.IsAny<GetObjectArgs>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DownloadAsync_WhenFileMissing_ThrowsFileStorageException()
    {
        // Arrange
        var service = this.CreateService();

        this.mockMinioClient
            .Setup(c => c.GetObjectAsync(It.IsAny<GetObjectArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Object not found"));

        // Act
        var act = async () => await service.DownloadAsync("missing.txt", "test-bucket");

        // Assert
        await act.Should().ThrowAsync<FileStorageException>()
            .WithMessage("*Failed to download file*");
    }

    [Fact]
    public async Task RemoveAsync_WithExistingFile_RemovesFile()
    {
        // Arrange
        var service = this.CreateService();

        this.mockMinioClient
            .Setup(c => c.RemoveObjectAsync(It.IsAny<RemoveObjectArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await service.RemoveAsync("test.txt", "test-bucket");

        // Assert
        this.mockMinioClient.Verify(
            c => c.RemoveObjectAsync(It.IsAny<RemoveObjectArgs>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_WhenMinioFails_ThrowsFileStorageException()
    {
        // Arrange
        var service = this.CreateService();

        this.mockMinioClient
            .Setup(c => c.RemoveObjectAsync(It.IsAny<RemoveObjectArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("MinIO error"));

        // Act
        var act = async () => await service.RemoveAsync("test.txt", "test-bucket");

        // Assert
        await act.Should().ThrowAsync<FileStorageException>()
            .WithMessage("*Failed to remove file*");
    }

    [Fact]
    public async Task EnsureBucketExistsAsync_WhenBucketExists_DoesNotCreateBucket()
    {
        // Arrange
        var service = this.CreateService();

        this.mockMinioClient
            .Setup(c => c.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await service.EnsureBucketExistsAsync("existing-bucket");

        // Assert
        this.mockMinioClient.Verify(
            c => c.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task EnsureBucketExistsAsync_WhenBucketDoesNotExist_CreatesBucket()
    {
        // Arrange
        var service = this.CreateService();

        this.mockMinioClient
            .Setup(c => c.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.mockMinioClient
            .Setup(c => c.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await service.EnsureBucketExistsAsync("new-bucket");

        // Assert
        this.mockMinioClient.Verify(
            c => c.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task EnsureBucketExistsAsync_WhenMinioFails_ThrowsFileStorageException()
    {
        // Arrange
        var service = this.CreateService();

        this.mockMinioClient
            .Setup(c => c.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("MinIO connection error"));

        // Act
        var act = async () => await service.EnsureBucketExistsAsync("test-bucket");

        // Assert
        await act.Should().ThrowAsync<FileStorageException>()
            .WithMessage("*Failed to ensure bucket*");
    }
}
