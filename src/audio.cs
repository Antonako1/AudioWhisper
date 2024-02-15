using NAudio.Wave;
using NAudio.Dsp;
using System;

class Program {
    public static string appdata_location = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp\\temp_AudioWhisper_mic_playback\\";
    private static File_reader? json_file_reader = new();
    private JsonObject settings = json_file_reader?.data;
    static void Main(string[] args) {
        // Create new folder
        System.IO.Directory.CreateDirectory(appdata_location);
        EmptyDir();
        Console.Clear();
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
    private static SavingWaveProvider? filterSampleProvider;

    private static readonly string help_message = "Press '1' to start, '2' to stop, '3' to edit settings, 'Q' to exit.";
    public Program(){
        // CHECK IF old_save.json EXISTS
        if(settings.QuietStartMessage == false){
            if(File.Exists(Global.dir_path + "\\old_save.json")){
                // if agree
                string _temp_quick_message = "'old_save.json' file found in base directory. Change current settings to old ones? (y/n) \nThis message can be disabled in the settings"; 
                bool running = true;
                Console.WriteLine(_temp_quick_message);
                while (running){
                    var key = Console.ReadKey().Key;
                    switch (key){
                        case ConsoleKey.Y:
                            Reset_Save resetSaveInstance = new("old");
                            json_file_reader.Read_file();
                            json_file_reader.data.QuietStartMessage = true;
                            json_file_reader.Write_file();
                            this.settings = json_file_reader?.data;
                            running = false;
                            break;
                        case ConsoleKey.N:
                            running = false;
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine(_temp_quick_message);
                            break;
                    }
                }
            }
        }
        playing = settings.On;
        Global.playing = playing;
        Console_writing("main");
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
                    Global.playing = playing;
                    StopRecording();
                    EmptyDir();
                    StartRecording();
                    Console_writing("main");
                    break;
                case ConsoleKey.D2:
                    playing = false;
                    Global.playing = playing;
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
                case ConsoleKey.Q:
                    StopRecording();
                    EmptyDir();
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

        // apply filter(s)
        if(settings.HighPassOn){
            var highPassFilter = new HighPassFilter(bufferedWaveProvider.ToSampleProvider(), settings.HighPassFrequency, settings.HighPassQualityFactor);
            savingWaveProvider = new SavingWaveProvider(highPassFilter.ToWaveProvider(), appdata_location + Guid.NewGuid() + ".wav");
        }
        else{
        // if(settings.LowPassOn){
            var lowPassFilter = new LowPassFilter(bufferedWaveProvider.ToSampleProvider(), settings.LowPassFrequency, settings.LowPassQualityFactor);
            savingWaveProvider = new SavingWaveProvider(lowPassFilter.ToWaveProvider(), appdata_location + Guid.NewGuid() + ".wav");
        }


        // savingWaveProvider = new SavingWaveProvider(bufferedWaveProvider, appdata_location + Guid.NewGuid() + ".wav");




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
        // todo: seconds ->
        if(recorder?.GetPosition() / 8096 > settings.EmptyCacheSeconds){
            StopRecording();
            EmptyDir();
            StartRecording();
        }
    }
    private static void Console_writing(string what){
        Console.ResetColor(); 
        Console.Clear(); 
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"AudioWhisper 1.0.2. Active: {Global.playing}");
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Currently in:");
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Main | Main");
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;
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
        Console.ResetColor();
    }
}

