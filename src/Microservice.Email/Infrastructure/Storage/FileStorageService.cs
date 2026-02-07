using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microservice.Email.Core.Configuration;
using Microservice.Email.Core.Exceptions;
using Microservice.Email.Core.Interfaces;

using Minio;
using Minio.DataModel.Args;

namespace Microservice.Email.Infrastructure.Storage;

/// <summary>
/// MinIO implementation of the file storage service.
/// </summary>
public sealed class FileStorageService : IFileStorageService
{
    private readonly IMinioClient minioClient;
    private readonly MinioSettings settings;
    private readonly ILogger<FileStorageService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileStorageService"/> class.
    /// </summary>
    public FileStorageService(
        IMinioClient minioClient,
        IOptions<MinioSettings> settings,
        ILogger<FileStorageService> logger)
    {
        this.minioClient = minioClient;
        this.settings = settings.Value;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> UploadAsync(Stream stream, string fileName, string bucketName, string? contentType = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await this.EnsureBucketExistsAsync(bucketName, cancellationToken);

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType ?? "application/octet-stream");

            await this.minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            this.logger.LogInformation("Successfully uploaded file {FileName} to bucket {BucketName}", fileName, bucketName);

            return $"{bucketName}/{fileName}";
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to upload file {FileName} to bucket {BucketName}", fileName, bucketName);
            throw new FileStorageException($"Failed to upload file '{fileName}' to bucket '{bucketName}'.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadAsync(string fileName, string bucketName, CancellationToken cancellationToken = default)
    {
        try
        {
            var memoryStream = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithCallbackStream(stream =>
                {
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                });

            await this.minioClient.GetObjectAsync(getObjectArgs, cancellationToken);

            this.logger.LogInformation("Successfully downloaded file {FileName} from bucket {BucketName}", fileName, bucketName);

            return memoryStream;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to download file {FileName} from bucket {BucketName}", fileName, bucketName);
            throw new FileStorageException($"Failed to download file '{fileName}' from bucket '{bucketName}'.", ex);
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string fileName, string bucketName, CancellationToken cancellationToken = default)
    {
        try
        {
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName);

            await this.minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);

            this.logger.LogInformation("Successfully removed file {FileName} from bucket {BucketName}", fileName, bucketName);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to remove file {FileName} from bucket {BucketName}", fileName, bucketName);
            throw new FileStorageException($"Failed to remove file '{fileName}' from bucket '{bucketName}'.", ex);
        }
    }

    /// <inheritdoc />
    public async Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
            bool exists = await this.minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

            if (!exists)
            {
                var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
                await this.minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
                this.logger.LogInformation("Created bucket {BucketName}", bucketName);
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to ensure bucket {BucketName} exists", bucketName);
            throw new FileStorageException($"Failed to ensure bucket '{bucketName}' exists.", ex);
        }
    }
}
