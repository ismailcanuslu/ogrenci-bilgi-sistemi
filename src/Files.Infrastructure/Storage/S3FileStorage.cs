using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Files.Application.Abstractions.Storage;
using Files.Domain.ValueObjects;
using Files.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Files.Infrastructure.Storage;

public sealed class S3FileStorage : IFileStorage, IFileStorageReader, IDisposable
{
    private readonly S3StorageOptions _options;
    private readonly IAmazonS3 _s3Client;
    private bool _bucketChecked;

    public S3FileStorage(IOptions<FileStorageOptions> options)
    {
        _options = options.Value.S3;

        var config = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(_options.Region),
            ForcePathStyle = _options.ForcePathStyle
        };

        if (!string.IsNullOrWhiteSpace(_options.ServiceUrl))
        {
            config.ServiceURL = _options.ServiceUrl;
        }

        var credentials = new BasicAWSCredentials(
            _options.AccessKey,
            _options.SecretKey);

        _s3Client = new AmazonS3Client(credentials, config);
    }

    public async Task<StorageKey> UploadAsync(
        Stream contentStream,
        FileName fileName,
        ContentType contentType,
        CancellationToken cancellationToken = default)
    {
        await EnsureBucketExistsAsync(cancellationToken);

        var safeFileName = Path.GetFileName(fileName.Value);
        var extension = Path.GetExtension(safeFileName).ToLowerInvariant();
        var datePrefix = DateTime.UtcNow.ToString("yyyy/MM/dd");
        var uniqueId = Guid.NewGuid().ToString("N");
        var keyString = $"{datePrefix}/{uniqueId}{extension}";

        var storageKey = StorageKey.Create(keyString).Value!;

        var putRequest = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = storageKey.Value,
            InputStream = contentStream,
            ContentType = contentType.Value
        };

        await _s3Client.PutObjectAsync(putRequest, cancellationToken);

        return storageKey;
    }

    public async Task<Stream> OpenReadStreamAsync(
        StorageKey key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key.Value
            };

            var response = await _s3Client.GetObjectAsync(request, cancellationToken);

            var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;

            return memoryStream;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new FileNotFoundException(
                "S3/MinIO nesnesi depolamada bulunamadı.",
                key.Value,
                ex);
        }
    }

    public async Task DeleteAsync(
        StorageKey key,
        CancellationToken cancellationToken = default)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key.Value
        };

        await _s3Client.DeleteObjectAsync(request, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        StorageKey key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _options.BucketName,
                Key = key.Value
            };

            await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        if (_bucketChecked)
        {
            return;
        }

        var exists = await AmazonS3Util.DoesS3BucketExistV2Async(
            _s3Client,
            _options.BucketName);

        if (!exists)
        {
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = _options.BucketName,
                UseClientRegion = true
            };

            await _s3Client.PutBucketAsync(putBucketRequest, cancellationToken);
        }

        _bucketChecked = true;
    }

    public void Dispose()
    {
        _s3Client.Dispose();
    }
}
