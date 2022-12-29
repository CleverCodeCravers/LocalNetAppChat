using LocalNetAppChat.Server.Domain.StoringFiles;
using NUnit.Framework;

namespace LocalNetAppChat.Server.Domain.Tests.StoringFiles;

[TestFixture]
public class SimpleFileStorageTests
{
    private const string Key = "1234";

    private StorageServiceProvider GetTestStorage()
    {
        var storageTestDirectory =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "TestStorage");

        // clear old storage space
        if (Directory.Exists(storageTestDirectory))
            Directory.Delete(storageTestDirectory, true);

        var storage = new StorageServiceProvider(Key, storageTestDirectory);
        
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
        
        CollectionAssert.AreEqual(fileContent, downloadResult.Value);
    }
}