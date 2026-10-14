namespace PdfProcessingService.Tests;

public class PdfProcessingServiceTests
{
    public PdfProcessingServiceTests()
    {
        IronPdf.License.LicenseKey = "YOUR_LICENSE_KEY";
    }

    [Fact]
    public void RemovePage_WhenIndexIsNegative_ThrowsArgumentOutOfRangeException()
    {
        using var merged = new PdfProcessingService(GetFixture("merged.pdf"));

        Assert.Throws<ArgumentOutOfRangeException>(() => merged.RemovePage(-1));
    }

    [Fact]
    public void RemovePage_WhenOnlyOnePageRemains_ThrowsInvalidOperationException()
    {
        using var merged = new PdfProcessingService(GetFixture("sample.pdf"));

        Assert.Equal(1, merged.PageCount);

        Assert.Throws<InvalidOperationException>(() => merged.RemovePage(0));
    }

    [Fact]
    public void RemovePage_WhenIndexIsValid_DecreasesPageCountByOne()
    {
        using var merged = new PdfProcessingService(GetFixture("merged.pdf"));

        var originalPageCount = merged.PageCount;

        merged.RemovePage(0);

        Assert.Equal(originalPageCount - 1, merged.PageCount);
    }

    [Fact]
    public void Save_WhenDestinationExistsAndOverwriteIsFalse_ThrowsIOException()
    {
        var destination = Path.GetTempFileName();

        try
        {
            using var merged = new PdfProcessingService(GetFixture("merged.pdf"));

            Assert.Throws<IOException>(() => merged.Save(destination));
        }
        finally
        {
            File.Delete(destination);
        }
    }

    [Fact]
    public void Save_WhenDestinationExistsAndOverwriteIsTrue_OverwritesFile()
    {
        var destination = Path.GetTempFileName();

        try
        {
            using (var merged = new PdfProcessingService(GetFixture("merged.pdf")))
            {
                merged.Save(destination, true);
            }

            using var savedPdf = new PdfProcessingService(destination);

            Assert.Equal(4, savedPdf.PageCount);
        }
        finally
        {
            File.Delete(destination);
        }
    }

    [Fact]
    public void RemovePage_WhenServiceIsDisposed_ThrowsObjectDisposedException()
    {
        var merged = new PdfProcessingService(GetFixture("merged.pdf"));

        merged.Dispose();

        Assert.Throws<ObjectDisposedException>(() => merged.RemovePage(0));
    }

    [Fact]
    public void Dispose_WhenCalledMultipleTimes_DoesNotThrow()
    {
        var merged = new PdfProcessingService(GetFixture("merged.pdf"));

        merged.Dispose();

        var exception = Record.Exception(() => merged.Dispose());

        Assert.Null(exception);
    }

    private string GetFixture(string filename)
    {
        return Path.Combine(AppContext.BaseDirectory, "Fixtures", filename);
    }
}