using LocalNetAppChat.Server.Domain.StoringFiles;
using LocalNetAppChat.Server.Domain.Tests.Security;
using NUnit.Framework;

namespace LocalNetAppChat.Server.Domain.Tests.StoringFiles;

[TestFixture]
public class SimpleFileStorageTests
{
    private const string Key = "1234";

    private StorageServiceProvider GetTestStorage()
    {
        var accessControl = new AccessControlMock(true);
        
        var storageTestDirectory =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "TestStorage");

        // clear old storage space
        if (Directory.Exists(storageTestDirectory))
            Directory.Delete(storageTestDirectory, true);

        var storage = new StorageServiceProvider(accessControl, storageTestDirectory);
        
        return storage;
    }
    
    [Test]
    public void When_a_new_storage_is_created_it_is_empty()
    {
        var storage = GetTestStorage();
        
        var list = storage.GetFiles(Key);
        
        Assert.AreEqual(0, list.Value.Length);
    }
    
    [Test]
    public async Task When_a_file_is_created_it_pops_up_in_the_file_list()
    {
        var storage = GetTestStorage();
        
        var result = await storage.Upload(Key, "test.txt", 
            new MemoryStream(new byte[] { 1, 2, 3 }));
        Assert.IsTrue(result.IsSuccess, "File upload should work");
        
        var list = storage.GetFiles(Key);
        
        Assert.AreEqual(1, list.Value.Length);
        Assert.AreEqual("test.txt", list.Value[0]);
    }
    
    [Test]
    public async Task When_I_create_a_file_and_then_delete_it_the_storage_is_empty_again()
    {
        var storage = GetTestStorage();
        
        var result = await storage.Upload(Key, "test.txt", 
            new MemoryStream(new byte[] { 1, 2, 3 }));
        Assert.IsTrue(result.IsSuccess, "File upload should work");
        
        var deleteResult = storage.Delete(Key, "test.txt");
        Assert.IsTrue(deleteResult.IsSuccess, "delete should work");

        var list = storage.GetFiles(Key);
        
        Assert.AreEqual(0, list.Value.Length, "file list should be empty again");
    }
    
    [Test]
    public async Task Downloading_a_file_should_work()
    {
        var storage = GetTestStorage();
        var fileContent = new byte[] {1, 2, 3};
        
        var result = await storage.Upload(Key, "test.txt", 
            new MemoryStream(fileContent));
        Assert.IsTrue(result.IsSuccess, "File upload should work");

        var downloadResult = await storage.Download(Key, "test.txt");
        Assert.IsTrue(downloadResult.IsSuccess, "download should work");
        
        Assert.That(downloadResult.Value, Is.EqualTo(fileContent));
    }
    
    [Test]
    public async Task Downloading_a_non_existing_file_should_fail()
    {
        var storage = GetTestStorage();
        
        var downloadResult = await storage.Download(Key, "test.txt");
        Assert.IsFalse(downloadResult.IsSuccess, "download should fail");
        Assert.AreEqual(downloadResult.Error, "File does not exist!");
    }
    
    [Test]
    public void Deleting_a_non_existing_file_should_fail()
    {
        var storage = GetTestStorage();

        var deleteResult = storage.Delete(Key, "test.txt");
        Assert.IsFalse(deleteResult.IsSuccess, "delete should fail");
        Assert.AreEqual(deleteResult.Error, "File does not exist!");
    }

    [Test]
    public async Task Upload_with_path_traversal_should_sanitize_filename()
    {
        var storage = GetTestStorage();

        var result = await storage.Upload(Key, "../../evil.txt",
            new MemoryStream(new byte[] { 1, 2, 3 }));

        Assert.That(result.IsSuccess, Is.True, "Upload should succeed with sanitized filename");

        var list = storage.GetFiles(Key);
        Assert.That(list.Value.Length, Is.EqualTo(1));
        Assert.That(list.Value[0], Is.EqualTo("evil.txt"));
    }

    [Test]
    public async Task Upload_with_absolute_path_should_sanitize_filename()
    {
        var storage = GetTestStorage();

        var result = await storage.Upload(Key, "/etc/passwd",
            new MemoryStream(new byte[] { 1, 2, 3 }));

        Assert.That(result.IsSuccess, Is.True, "Upload should succeed with sanitized filename");

        var list = storage.GetFiles(Key);
        Assert.That(list.Value.Length, Is.EqualTo(1));
        Assert.That(list.Value[0], Is.EqualTo("passwd"));
    }

    [Test]
    public async Task Download_with_path_traversal_should_not_escape_storage_directory()
    {
        var storage = GetTestStorage();

        // Upload a normal file first
        await storage.Upload(Key, "test.txt",
            new MemoryStream(new byte[] { 1, 2, 3 }));

        // Try to download with a traversal path
        var downloadResult = await storage.Download(Key, "../../etc/passwd");

        // Should either fail or return the sanitized file
        if (downloadResult.IsSuccess)
        {
            // If it succeeded, it should have been sanitized to just "passwd" and that file doesn't exist
            Assert.Fail("Download with path traversal should not succeed unless the sanitized filename exists");
        }
        else
        {
            Assert.That(downloadResult.Error, Is.Not.Empty);
        }
    }

    [Test]
    public void Delete_with_path_traversal_should_not_escape_storage_directory()
    {
        var storage = GetTestStorage();

        var deleteResult = storage.Delete(Key, "../../etc/passwd");

        Assert.That(deleteResult.IsSuccess, Is.False, "Delete with traversal path should fail");
    }

    [Test]
    public async Task Upload_with_denied_access_should_fail()
    {
        var accessControl = new AccessControlMock(false);
        var storageTestDirectory =
            Path.Combine(Directory.GetCurrentDirectory(), "TestStorageDenied");
        if (Directory.Exists(storageTestDirectory))
            Directory.Delete(storageTestDirectory, true);

        var storage = new StorageServiceProvider(accessControl, storageTestDirectory);

        var result = await storage.Upload(Key, "test.txt",
            new MemoryStream(new byte[] { 1, 2, 3 }));

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Access denied"));
    }

    [Test]
    public void GetFiles_with_denied_access_should_fail()
    {
        var accessControl = new AccessControlMock(false);
        var storageTestDirectory =
            Path.Combine(Directory.GetCurrentDirectory(), "TestStorageDenied2");
        if (Directory.Exists(storageTestDirectory))
            Directory.Delete(storageTestDirectory, true);

        var storage = new StorageServiceProvider(accessControl, storageTestDirectory);

        var result = storage.GetFiles(Key);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Access denied"));
    }

    [Test]
    public async Task Download_with_denied_access_should_fail()
    {
        var accessControl = new AccessControlMock(false);
        var storageTestDirectory =
            Path.Combine(Directory.GetCurrentDirectory(), "TestStorageDenied3");
        if (Directory.Exists(storageTestDirectory))
            Directory.Delete(storageTestDirectory, true);

        var storage = new StorageServiceProvider(accessControl, storageTestDirectory);

        var result = await storage.Download(Key, "test.txt");

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Access denied"));
    }

    [Test]
    public void Delete_with_denied_access_should_fail()
    {
        var accessControl = new AccessControlMock(false);
        var storageTestDirectory =
            Path.Combine(Directory.GetCurrentDirectory(), "TestStorageDenied4");
        if (Directory.Exists(storageTestDirectory))
            Directory.Delete(storageTestDirectory, true);

        var storage = new StorageServiceProvider(accessControl, storageTestDirectory);

        var result = storage.Delete(Key, "test.txt");

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Access denied"));
    }
}