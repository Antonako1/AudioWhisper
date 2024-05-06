using NAudio.Wave;
using NAudio.Dsp;
using System;
using SharpHook;
class Program {
    public static string appdata_location = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp\\temp_AudioWhisper_mic_playback\\";
    private static File_reader? json_file_reader = new();
    private JsonObject settings = json_file_reader?.data;
    
    static void Main(string[] args) {
        
        // Create new folder
        System.IO.Directory.CreateDirectory(appdata_location);
        try{
            EmptyDir();
        } catch (Exception e){
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Error. Audio file may be in use or the executable doesn't have permissions to read/write it: " + e.Message);
            Console.ResetColor();
            Environment.Exit(1);
        }
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
    private SharpHook.Native.KeyCode? hookKeyPress;

    private static readonly string help_message = "Press '1' to start, '2' to stop, '3' to edit settings, 'F7' to toggle mic playback while out of focus, 'Q' to exit.";
    public Program(){
        // CHECK IF old_save.json EXISTS
        if(settings.QuietStartMessage == false){
            if(File.Exists(Global.dir_path + "\\old_save.json")){
                // if agree
                string _temp_quick_message = "'old_save.json' file found in base directory. Change current settings to old ones? (y/n) \nThis message can be disabled in the settings"; 
                bool running = true;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(_temp_quick_message);
                Console.ResetColor();
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
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(_temp_quick_message);
                            Console.ResetColor();
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
        // Init hook
        InitializeSharpHook();
        while (true){
            // First try hook presses
            // switch(hookKeyPress){
            //     case SharpHook.Native.KeyCode.VcF7:
            //         if(playing){
            //             playing = true;
            //             Global.playing = playing;
            //             StopRecording();
            //             EmptyDir();
            //             StartRecording();
            //             Console_writing("main");
            //         }
            //         else{
            //             playing = false;
            //             Global.playing = playing;
            //             StopRecording();
            //             EmptyDir();
            //             Console_writing("main");
            //         }
            //         continue;
            // }

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
                    json_file_reader.Read_file();
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
                case ConsoleKey.F7: break; // fallthrough
                default:
                    Console_writing("error");
                    break;
            }
            // hookKeyPress = null;
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
        Console.WriteLine($"AudioWhisper 1.1.0.4. Active: {Global.playing}");
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
    private void OnKeyReleased(object? sender, KeyboardHookEventArgs e){
        switch(e.Data.KeyCode){
            case SharpHook.Native.KeyCode.VcF7:
                if(!playing){
                    playing = true;
                    Global.playing = playing;
                    StopRecording();
                    EmptyDir();
                    StartRecording();
                    Console_writing("main");
                }
                else{
                    playing = false;
                    Global.playing = playing;
                    StopRecording();
                    EmptyDir();
                    Console_writing("main");
                }
                break;
        }
    }
    private async void InitializeSharpHook(){
        var hook = new TaskPoolGlobalHook();
        hook.KeyReleased += OnKeyReleased;     // EventHandler<KeyboardHookEventArgs>
        await hook.RunAsync();
    }  
}

