using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microservice.Email.Core.Configuration;
using Microservice.Email.Core.Exceptions;
using Microservice.Email.Core.Interfaces;
using Microservice.Email.Core.Metrics;

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
            await EnsureBucketExistsAsync(bucketName, cancellationToken);

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType ?? "application/octet-stream");

            await minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            EmailMetrics.RecordStorageOperation("upload", true);

            logger.LogInformation("Successfully uploaded file {FileName} to bucket {BucketName}", fileName, bucketName);

            return $"{bucketName}/{fileName}";
        }
        catch (Exception ex)
        {
            EmailMetrics.RecordStorageOperation("upload", false);

            logger.LogError(ex, "Failed to upload file {FileName} to bucket {BucketName}", fileName, bucketName);
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

            await minioClient.GetObjectAsync(getObjectArgs, cancellationToken);

            EmailMetrics.RecordStorageOperation("download", true);

            logger.LogInformation("Successfully downloaded file {FileName} from bucket {BucketName}", fileName, bucketName);

            return memoryStream;
        }
        catch (Exception ex)
        {
            EmailMetrics.RecordStorageOperation("download", false);

            logger.LogError(ex, "Failed to download file {FileName} from bucket {BucketName}", fileName, bucketName);
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

            await minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);

            EmailMetrics.RecordStorageOperation("delete", true);

            logger.LogInformation("Successfully removed file {FileName} from bucket {BucketName}", fileName, bucketName);
        }
        catch (Exception ex)
        {
            EmailMetrics.RecordStorageOperation("delete", false);

            logger.LogError(ex, "Failed to remove file {FileName} from bucket {BucketName}", fileName, bucketName);
            throw new FileStorageException($"Failed to remove file '{fileName}' from bucket '{bucketName}'.", ex);
        }
    }

    /// <inheritdoc />
    public async Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
            bool exists = await minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

            if (!exists)
            {
                var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
                await minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
                logger.LogInformation("Created bucket {BucketName}", bucketName);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to ensure bucket {BucketName} exists", bucketName);
            throw new FileStorageException($"Failed to ensure bucket '{bucketName}' exists.", ex);
        }
    }
}
