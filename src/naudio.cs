using NAudio.Wave;
using NAudio.Dsp;

class SavingWaveProvider : IWaveProvider, IDisposable{
    private readonly IWaveProvider sourceWaveProvider;
    private readonly WaveFileWriter writer;
    private bool isWriterDisposed;
    public readonly string wavFilePath;

    public SavingWaveProvider(IWaveProvider sourceWaveProvider, string _wavFilePath)
    {
        this.sourceWaveProvider = sourceWaveProvider;
        wavFilePath = _wavFilePath;
        writer = new WaveFileWriter(wavFilePath, sourceWaveProvider.WaveFormat);
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        var read = sourceWaveProvider.Read(buffer, offset, count);

        if (count > 0 && !isWriterDisposed)
        {
            writer.Write(buffer, offset, read);
        }
        if (count == 0)
        {
            Dispose(); // auto-dispose in case users forget
        }
        return read;
    }

    public WaveFormat WaveFormat { get { return sourceWaveProvider.WaveFormat; } }

    public void Dispose(){
        if (!isWriterDisposed){
            isWriterDisposed = true;
            writer.Dispose();
        }
    }
}

public class HighPassFilter : ISampleProvider
{
    private readonly ISampleProvider source;
    private readonly BiQuadFilter highPassFilter;

    public WaveFormat WaveFormat => source.WaveFormat;

    public HighPassFilter(ISampleProvider source, float cutoffFrequency, float q)
    {
        this.source = source;
        highPassFilter = BiQuadFilter.HighPassFilter(source.WaveFormat.SampleRate, cutoffFrequency, q);
    }

    public int Read(float[] buffer, int offset, int count)
    {
        int bytesRead = source.Read(buffer, offset, count);

        for (int n = 0; n < bytesRead; n++)
        {
            buffer[offset + n] = highPassFilter.Transform(buffer[offset + n]);
        }

        return bytesRead;
    }
}

public class LowPassFilter : ISampleProvider
{
    private readonly ISampleProvider source;
    private readonly BiQuadFilter lowPassFilter;

    public WaveFormat WaveFormat => source.WaveFormat;

    public LowPassFilter(ISampleProvider source, float cutoffFrequency, float q)
    {
        this.source = source;
        lowPassFilter = BiQuadFilter.LowPassFilter(source.WaveFormat.SampleRate, cutoffFrequency, q);
    }

    public int Read(float[] buffer, int offset, int count)
    {
        int bytesRead = source.Read(buffer, offset, count);

        for (int n = 0; n < bytesRead; n++)
        {
            buffer[offset + n] = lowPassFilter.Transform(buffer[offset + n]);
        }

        return bytesRead;
    }
}
