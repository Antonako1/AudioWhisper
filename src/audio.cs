using NAudio.Wave;
using System;

class Program {
    public static string appdata_location = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp\\temp_hermes_mic_playback\\";
    private static File_reader? json_file_reader = new();
    private JsonObject settings = json_file_reader?.data;
    static void Main(string[] args) {
        // Create new folder
        System.IO.Directory.CreateDirectory(appdata_location);
        EmptyDir();
        Program audioplayer = new();
    }

    static void EmptyDir(){
        // Empty on start
        System.IO.DirectoryInfo di = new(appdata_location);
        foreach (FileInfo file in di.EnumerateFiles()){
            file.Delete(); 
        }
    }
    private static WaveInEvent? recorder;
    private static BufferedWaveProvider? bufferedWaveProvider;
    private static SavingWaveProvider? savingWaveProvider;
    private static WaveOutEvent? player;
    private static bool playing = false;

    private static readonly string help_message = "Press '1' to start, '2' to stop, '3' to edit settings, '4' to exit.";
    public Program(){
        playing = settings.On;
        Console_writing("main");
        // Automatic start
        if(playing){
            StopRecording();
            EmptyDir();
            StartRecording();
        }
        while (true){
            var key = Console.ReadKey().Key;
            switch (key){
                case ConsoleKey.D1:
                    playing = true;
                    StopRecording();
                    EmptyDir();
                    StartRecording();
                    Console_writing("main");
                    break;
                case ConsoleKey.D2:
                    playing = false;
                    StopRecording();
                    EmptyDir();
                    Console_writing("main");
                    break;
                case ConsoleKey.D3:
                    json_file_reader?.Cli();
                    settings = json_file_reader?.data;
                    Console_writing("main");
                    if(playing){
                        StopRecording();
                        EmptyDir();
                        StartRecording();
                    }
                    break;
                case ConsoleKey.D4:
                    Environment.Exit(0);
                    break;
                default:
                    Console_writing("error");
                    break;
            }
        }
    }

    public void StartRecording(){
        // set up the recorder
        recorder = new WaveInEvent(){
            BufferMilliseconds = settings.BufferMilliseconds,
            NumberOfBuffers = settings.NumberOfBuffers,
        };
        recorder.DataAvailable += RecorderOnDataAvailable;

        // set up our signal chain
        bufferedWaveProvider = new BufferedWaveProvider(recorder.WaveFormat){
            BufferLength = settings.BufferLength,
            DiscardOnBufferOverflow = settings.DiscardOnBufferOverflow,
        };
        savingWaveProvider = new SavingWaveProvider(bufferedWaveProvider, appdata_location + Guid.NewGuid() + ".wav");

        // set up playback
        player = new WaveOutEvent()
        {
            DesiredLatency = settings.DesiredLatency,
        };
        player.Init(savingWaveProvider);
        player.Volume = (float)settings.Volume / 100;

        // begin playback & record
        player.Play();
        recorder.StartRecording();
    }

    public static void StopRecording(){
        // stop recording
        recorder?.StopRecording();

        // stop playback
        player?.Stop();

        // finalise the .WAV file
        savingWaveProvider?.Dispose();
    }

    private void RecorderOnDataAvailable(object sender, WaveInEventArgs waveInEventArgs){
        bufferedWaveProvider?.AddSamples(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);
        if(recorder?.GetPosition() / 8096 > settings.EmptyCacheSeconds){
            StopRecording();
            EmptyDir();
            StartRecording();
        }
    }
    private void Console_writing(string what){
        Console.Clear();
        Console.WriteLine("Currently in:");
        Console.WriteLine("Main | Main");
        switch(what){
            case "main":
                // Console.WriteLine("Main | Main");
                Console.WriteLine(help_message);
                break;
            case "error":
                // Console.Clear();
                // Console.WriteLine("Main | Main");
                Console.WriteLine("Invalid input. " + help_message);
                break;
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
