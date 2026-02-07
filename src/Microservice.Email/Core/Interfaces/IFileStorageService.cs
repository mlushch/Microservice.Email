namespace Microservice.Email.Core.Interfaces;

/// <summary>
/// Service interface for file storage operations.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file to storage.
    /// </summary>
    /// <param name="stream">The file stream to upload.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="bucketName">The storage bucket name.</param>
    /// <param name="contentType">The MIME content type of the file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The path to the uploaded file.</returns>
    Task<string> UploadAsync(Stream stream, string fileName, string bucketName, string? contentType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from storage.
    /// </summary>
    /// <param name="fileName">The name of the file to download.</param>
    /// <param name="bucketName">The storage bucket name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The file stream.</returns>
    Task<Stream> DownloadAsync(string fileName, string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a file from storage.
    /// </summary>
    /// <param name="fileName">The name of the file to remove.</param>
    /// <param name="bucketName">The storage bucket name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task RemoveAsync(string fileName, string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensures a bucket exists, creating it if necessary.
    /// </summary>
    /// <param name="bucketName">The bucket name to ensure exists.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken = default);
}
