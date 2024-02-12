using NAudio.Wave;
using System;

class Program {
    public static string appdata_location = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\temp_hermes_mic_playback\\";
    static void Main(string[] args) {
        // Create new folder
        System.IO.Directory.CreateDirectory(appdata_location);
        emptyDir();
        Program audioplayer = new();
    }

    static void emptyDir(){
        // Empty on start
        System.IO.DirectoryInfo di = new(appdata_location);
        foreach (FileInfo file in di.EnumerateFiles()){
            file.Delete(); 
        }
    }

    private static WaveInEvent recorder;
    private static BufferedWaveProvider bufferedWaveProvider;
    private static SavingWaveProvider savingWaveProvider;
    private static WaveOutEvent player;

    private static string help_message = "Press '1' to start, '2' to stop, '3' to exit.";
    public Program(){
        Console.WriteLine(help_message);
        while (true){
            var key = Console.ReadKey().Key;
            switch (key){
                case ConsoleKey.D1:
                    StartRecording();
                    break;
                case ConsoleKey.D2:
                    StopRecording();
                    emptyDir();
                    break;
                case ConsoleKey.D3:
                    Environment.Exit(0);
                    emptyDir();
                    break;
                default:
                    Console.WriteLine("Invalid input. " + help_message);
                    break;
            }
        }
    }

    public static void StartRecording(){
        // set up the recorder
        recorder = new WaveInEvent(){
            BufferMilliseconds = 50, // Adjust the buffer size as needed
            NumberOfBuffers = 10, // Adjust the number of buffers as needed
        };
        recorder.DataAvailable += RecorderOnDataAvailable;

        // set up our signal chain
        bufferedWaveProvider = new BufferedWaveProvider(recorder.WaveFormat){
            BufferLength = 8192, // Adjust the buffer size as needed
            DiscardOnBufferOverflow = true,
        };
        savingWaveProvider = new SavingWaveProvider(bufferedWaveProvider, appdata_location + Guid.NewGuid() + ".wav");

        // set up playback
        player = new WaveOutEvent()
        {
            DesiredLatency = 100, // Adjust the latency as needed
        };
        player.Init(savingWaveProvider);
        player.Volume = 0.05F;

        // begin playback & record
        player.Play();
        recorder.StartRecording();
    }

    public static void StopRecording(){
        // stop recording
        recorder.StopRecording();

        // stop playback
        player.Stop();

        // finalise the WAV file
        savingWaveProvider.Dispose();
    }

    private static void RecorderOnDataAvailable(object sender, WaveInEventArgs waveInEventArgs){
        bufferedWaveProvider.AddSamples(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);
        if(recorder.GetPosition() / 8096 > 100){
            StopRecording();
            emptyDir();
            StartRecording();
        }
    }
}

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
