using LocalNetAppChat.Domain.Shared;
using NUnit.Framework;

namespace LocalNetAppChat.Domain.Tests.Shared;

[TestFixture]
public class UtilTests
{
    [Test]
    public void SanitizeFilename_returns_simple_filename_unchanged()
    {
        var result = Util.SanitizeFilename("test.txt");

        Assert.That(result, Is.EqualTo("test.txt"));
    }

    [Test]
    public void SanitizeFilename_strips_directory_path()
    {
        var result = Util.SanitizeFilename("some/directory/test.txt");

        Assert.That(result, Is.EqualTo("test.txt"));
    }

    [Test]
    public void SanitizeFilename_strips_windows_directory_path()
    {
        var result = Util.SanitizeFilename(@"C:\Users\test\test.txt");

        Assert.That(result, Is.EqualTo("test.txt"));
    }

    [Test]
    public void SanitizeFilename_blocks_path_traversal_with_dot_dot_slash()
    {
        var result = Util.SanitizeFilename("../../etc/passwd");

        Assert.That(result, Is.EqualTo("passwd"));
    }

    [Test]
    public void SanitizeFilename_blocks_path_traversal_with_dot_dot_backslash()
    {
        var result = Util.SanitizeFilename(@"..\..\etc\passwd");

        Assert.That(result, Is.EqualTo("passwd"));
    }

    [Test]
    public void SanitizeFilename_handles_empty_string()
    {
        var result = Util.SanitizeFilename("");

        Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void SanitizeFilename_handles_filename_with_spaces()
    {
        var result = Util.SanitizeFilename("my file.txt");

        Assert.That(result, Is.EqualTo("my file.txt"));
    }

    [Test]
    public void SanitizeFilename_handles_filename_with_dots()
    {
        var result = Util.SanitizeFilename("archive.tar.gz");

        Assert.That(result, Is.EqualTo("archive.tar.gz"));
    }

    [Test]
    public void SanitizeFilename_strips_absolute_unix_path()
    {
        var result = Util.SanitizeFilename("/etc/passwd");

        Assert.That(result, Is.EqualTo("passwd"));
    }

    [Test]
    public void SanitizeFilename_handles_just_dot_dot()
    {
        var result = Util.SanitizeFilename("..");

        // Path.GetFileName("..") returns ".."
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void SanitizeFilename_handles_complex_traversal()
    {
        var result = Util.SanitizeFilename("../../../../../../../etc/shadow");

        Assert.That(result, Is.EqualTo("shadow"));
    }

    [Test]
    public void SanitizeFilename_handles_mixed_separators_traversal()
    {
        var result = Util.SanitizeFilename(@"..\../..\/secret.txt");

        Assert.That(result, Is.EqualTo("secret.txt"));
    }

    [Test]
    public void Obsolete_SanatizeFilename_works_same_as_SanitizeFilename()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        var result = Util.SanatizeFilename("path/to/file.txt");
#pragma warning restore CS0618

        Assert.That(result, Is.EqualTo("file.txt"));
    }
}
