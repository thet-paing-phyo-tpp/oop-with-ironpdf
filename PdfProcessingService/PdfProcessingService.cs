namespace PdfProcessingService;

public class PdfProcessingService : IDisposable
{
    private readonly PdfDocument _document;
    private bool _disposed;

    public PdfProcessingService(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        _document = PdfDocument.FromFile(filePath);
    }

    public int PageCount { 
        get
        {
            ThrowIfDisposed();
            return _document.PageCount;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _document.Dispose();
        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public void RemovePage(int pageIndex)
    {
        ThrowIfDisposed();

        if (pageIndex < 0 || pageIndex >= PageCount)
        {
            throw new ArgumentOutOfRangeException(nameof(pageIndex));
        }

        if (PageCount == 1)
        {
            throw new InvalidOperationException("Cannot remove the last remaining page.");
        }

        _document.RemovePage(pageIndex);
    }

    public void Save(string outputPath, bool overwrite = false)
    {
        ThrowIfDisposed();

        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        if (File.Exists(outputPath) && !overwrite)
        {
            throw new IOException($"The destination file already exists: {outputPath}");
        }

        _document.SaveAs(outputPath);
    }
}
