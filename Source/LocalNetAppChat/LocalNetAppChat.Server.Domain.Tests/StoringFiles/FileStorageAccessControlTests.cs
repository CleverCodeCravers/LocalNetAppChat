using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Server.Domain.StoringFiles;
using LocalNetAppChat.Server.Domain.Tests.Security;
using NUnit.Framework;

namespace LocalNetAppChat.Server.Domain.Tests.StoringFiles;

[TestFixture]
public class FileStorageAccessControlTests
{
    [Test]
    public void When_access_is_not_allowed_getting_the_filelist_fails_appropriatly()
    {
        var storage = GetStorageServiceProvider();

        var result = storage.GetFiles("1234");

        AssertAccessDenied(result.IsSuccess, result.Error);
    }

    [Test]
    public void When_access_is_not_allowed_deleting_a_file_fails_appropriatly()
    {
        var storage = GetStorageServiceProvider();

        var result = storage.Delete("1234", "Somefile");

        AssertAccessDenied(result.IsSuccess, result.Error);
    }

    [Test]
    public async Task When_access_is_not_allowed_downloading_a_file_fails_appropriatly()
    {
        var storage = GetStorageServiceProvider();

        var result = await storage.Download("1234", "Somefile");

        AssertAccessDenied(result.IsSuccess, result.Error);
    }
    
    [Test]
    public async Task When_access_is_not_allowed_uploading_a_file_fails_appropriatly()
    {
        var storage = GetStorageServiceProvider();

        var result = await storage.Upload("1234", "Somefile", new MemoryStream(Array.Empty<byte>()));

        AssertAccessDenied(result.IsSuccess, result.Error);
    }
    
    private void AssertAccessDenied(bool isSuccess, string error)
    {
        Assert.IsFalse(isSuccess);
        Assert.AreEqual("Access denied", error);
    }

    private static StorageServiceProvider GetStorageServiceProvider()
    {
        var denyAccess = new AccessControlMock(false);
        var storage = new StorageServiceProvider(denyAccess, "data");
        return storage;
    }
}