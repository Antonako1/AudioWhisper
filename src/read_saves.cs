using System.Text.Json;
using System;
using System.IO;
using System.Text;
using NAudio.Wave.SampleProviders;



class File_reader{
    private string dir_path = "";
    private string source_path = "\\save.json";
    private readonly string help_message = "Press '1' to edit values, '2' for help, '3' to return";
    private readonly string help_message_edit = "Enter setting number and give value. press 'enter' to save, 'esc' to return";
    private readonly string info_message = "Changes are saved automatically. They will be applied once you return to 'Main | Main'";
    public JsonObject? data;
    public File_reader(){
        string exePath = Environment.ProcessPath;

        string exeDirectory = Path.GetDirectoryName(exePath);
        this.dir_path = exeDirectory;
        Global.dir_path = this.dir_path;
        this.source_path = exeDirectory + source_path;

        Read_file();
    }

    public void Cli(){     
        bool run_settings = true;
        Read_file(); 
        Console_writing("main");
        Console_writing("main");

        while(run_settings){
            var key = Console.ReadKey().Key;
            switch (key){
                case ConsoleKey.D1:
                    Console_writing("editing");
                    Console_writing("editing");
                    while(true){
                        key = Console.ReadKey().Key;
                        (int result, ReturnObject placehold) = Handle_setting(key);
                        if(result == 777){
                            break;
                        }
                        Console.WriteLine($"\nEditing '{placehold.Name}'. Possible values: '{placehold.Values}'");
                        bool idk = CancelableReadLine(out string line);
                        if(!idk){
                            break;
                        }
                        try{
                            if(Save_changes(result, line) == 0){
                                Write_file();
                                break;
                            } 
                        } catch {
                            Console.WriteLine("Possible null values");
                        }
                    }
                    Console_writing("main");
                    Console_writing("main");
                    break;
                case ConsoleKey.D2:
                    Console_writing("help");
                    break;
                case ConsoleKey.D3:
                    run_settings = false;
                    break;
                default:
                    Console_writing("error");
                    Console_writing("error");
                    break;
            }
        }
    }

    public void Read_file(){
        string json_data_as_string = File.ReadAllText(source_path);
        this.data = JsonSerializer.Deserialize<JsonObject>(json_data_as_string);
    }
    public void Write_file(){
        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };
        string json_data_as_string = JsonSerializer.Serialize(data, options);
        File.WriteAllText(source_path, json_data_as_string);;
    }
    private int Save_changes(int what, string? line){
        switch (what){
            case 0: 
                if(bool.TryParse(line, out bool parse0)){
                    data.On = parse0; 
                    return 0;
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;
            case 1: 
                if(int.TryParse(line, out int parse1)){
                    if(Errorchecks.Errorcheck_volume(parse1)){
                        data.Volume = parse1; 
                        return 0;
                    }
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;
            case 2: 
                if(int.TryParse(line, out int parse2)){
                    if(Errorchecks.Errorcheck_over0(parse2)){
                        data.BufferMilliseconds = parse2;
                        return 0;
                    }
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;
            case 3: 
                if(int.TryParse(line, out int parse3)){
                    if(Errorchecks.Errorcheck_over0(parse3)){
                        data.NumberOfBuffers = parse3;
                        return 0;
                    }
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;
            case 4: 
                if(int.TryParse(line, out int parse4)){
                    if(Errorchecks.Errorcheck_over0(parse4)){
                        data.BufferLength = parse4;
                        return 0;
                    }
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;
            case 5: 
                if(bool.TryParse(line, out bool parse5)){
                    data.DiscardOnBufferOverflow = parse5;
                    return 0;
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;
            case 6: 
                if(int.TryParse(line, out int parse6)){
                    if(Errorchecks.Errorcheck_over0(parse6)){
                        data.DesiredLatency = parse6;
                        return 0;
                    }
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;
            case 7: 
                if(int.TryParse(line, out int parse7)){
                    if(Errorchecks.Errorcheck_over0(parse7)){
                        data.EmptyCacheSeconds = parse7;
                        return 0;
                    }
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;
            case 8: 
                if(bool.TryParse(line, out bool parse8)){
                    if(data.LowPassOn){
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("WARNING: Currently only one filter can be active. Would you like to turn off low-pass filter? (y/n)");
                        Console.ResetColor();
                        while(true){
                            var key = Console.ReadKey().Key;
                            switch (key){
                                case ConsoleKey.Y:
                                    data.LowPassOn = false;
                                    data.HighPassOn = parse8;
                                    return 0;
                                case ConsoleKey.N:
                                    return 0;
                                default:
                                    Console_writing("editing");
                                    Console_writing("editing");
                                    Console.BackgroundColor = ConsoleColor.Red;
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.WriteLine("WARNING: Currently only one filter can be active. Would you like to turn off low-pass filter? (y/n)");
                                    Console.ResetColor();
                                    break;
                            }
                        }
                    } else {
                        data.HighPassOn = parse8;
                        return 0;
                    }
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;
            case 9:
                if(int.TryParse(line, out int parse9)){
                    if(Errorchecks.Errorcheck_over0(parse9)){
                        data.HighPassFrequency = parse9;
                        return 0;
                    }
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;               
            case 10: 
                if(int.TryParse(line, out int parse10)){
                    if(Errorchecks.Errorcheck_over0(parse10)){

                        // ! Give warning if too large
                        if(parse10 > 10){
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("WARNING: Too high values may cause damage to headphones or ears, do you want to proceed? (y/n)");
                            Console.ResetColor();
                            while(true){
                                var key = Console.ReadKey().Key;
                                switch (key){
                                    case ConsoleKey.Y:
                                        data.HighPassQualityFactor = parse10;
                                        return 0;
                                    case ConsoleKey.N:
                                        return 0;
                                    default:
                                        Console_writing("editing");
                                        Console_writing("editing");
                                        Console.BackgroundColor = ConsoleColor.Red;
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.WriteLine("WARNING: Too high values may cause damage to headphones or ears, do you want to proceed? (y/n)");
                                        Console.ResetColor();
                                        break;

                                }
                            }
                        } else {
                            data.HighPassQualityFactor = parse10;
                            return 0;
                        }
                    }
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;          
            case 11: 
                if(bool.TryParse(line, out bool parse11)){
                    if(data.HighPassOn){
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("WARNING: Currently only one filter can be active. Would you like to turn off high-pass filter? (y/n)");
                        Console.ResetColor();
                        while(true){
                            var key = Console.ReadKey().Key;
                            switch (key){
                                case ConsoleKey.Y:
                                    data.HighPassOn = false;
                                    data.LowPassOn = parse11;
                                    return 0;
                                case ConsoleKey.N:
                                    return 0;
                                default:
                                    Console_writing("editing");
                                    Console_writing("editing");
                                    Console.BackgroundColor = ConsoleColor.Red;
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.WriteLine("WARNING: Currently only one filter can be active. Would you like to turn off high-pass filter? (y/n)");
                                    Console.ResetColor();
                                    break;
                            }
                        }
                    } else {
                        data.LowPassOn = parse11;
                        return 0;
                    }
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;
            case 12:
                if(int.TryParse(line, out int parse12)){
                    if(Errorchecks.Errorcheck_over0(parse12)){
                        data.LowPassFrequency = parse12;
                        return 0;
                    }
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;               
            case 13: 
                if(int.TryParse(line, out int parse13)){
                    if(Errorchecks.Errorcheck_over0(parse13)){

                        // ! Give warning if too large
                        if(parse13 > 10){
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("WARNING: Too high values may cause damage to headphones or ears, do you want to proceed? (y/n)");
                            Console.ResetColor();
                            while(true){
                                var key = Console.ReadKey().Key;
                                switch (key){
                                    case ConsoleKey.Y:
                                        data.LowPassQualityFactor = parse13;
                                        return 0;
                                    case ConsoleKey.N:
                                        return 0;
                                    default:
                                        Console_writing("editing");
                                        Console_writing("editing");
                                        Console.BackgroundColor = ConsoleColor.Red;
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.WriteLine("WARNING: Too high values may cause damage to headphones or ears, do you want to proceed? (y/n)");
                                        Console.ResetColor();
                                        break;

                                }
                            }
                        } else {
                            data.LowPassQualityFactor = parse13;
                            return 0;
                        }
                    }
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;
            case 14:
                if(bool.TryParse(line, out bool parse14)){
                    data.RevertChanges = parse14; 
                    // new save.json file from backup.json
                    Reset_Save resetSaveInstance = new("backup");
                    Console.WriteLine("Settings have been reverted");
                    return 1;
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;
            case 15:
                if(bool.TryParse(line, out bool parse15)){
                    data.QuietStartMessage = parse15; 
                    return 0;
                }
                Console.WriteLine($"Given value '{line}' may be incorrect.");
                return 1;
            default: Console.WriteLine("Error parsing"); return 1;
        }
    }

    /*
    q = 10
    w = 11
    e = 12
    r = 13
    t = 14
    y = 15
    jne.
    */
    private Tuple<int, ReturnObject> Handle_setting(ConsoleKey setting_number){
        ReturnObject return_object = new()
        {
            Values = "none",
            Name = "nothing"
        };
        switch (setting_number){
            case ConsoleKey.D0:
                // On off
                return_object.Values = "true | false";
                return_object.Name = "On/Off";
                return Tuple.Create(0, return_object);
            case ConsoleKey.D1:
                // Playback volume
                return_object.Values = "Integer 0-100";
                return_object.Name = "Volume";
                return Tuple.Create(1, return_object);
            case ConsoleKey.D2:
                // Chunk length
                return_object.Values = "Integer 0<=";
                return_object.Name = "Chunk length";
                return Tuple.Create(2, return_object);
            case ConsoleKey.D3:
                // Chunk overlap
                return_object.Values = "Integer 0<=";
                return_object.Name = "Chunk overlap";
                return Tuple.Create(3, return_object);
            case ConsoleKey.D4:
                // Chunk overlap
                return_object.Values = "Integer 0<=";
                return_object.Name = "Buffer size";
                return Tuple.Create(4, return_object);
            case ConsoleKey.D5:
                // Buffer overflow
                return_object.Values = "true | false";
                return_object.Name = "Buffer overflow";
                return Tuple.Create(5, return_object);
            case ConsoleKey.D6:
                // Latency
                return_object.Values = "Integer 0<=";
                return_object.Name = "Latency";
                return Tuple.Create(6, return_object);
            case ConsoleKey.D7:
                // Cache emptyrate
                return_object.Values = "Integer 0<=";
                return_object.Name = "Cache emptier";
                return Tuple.Create(7, return_object);
            case ConsoleKey.D8:
                // High-pass filter
                return_object.Values = "true | false";
                return_object.Name = "High-pass filter";
                return Tuple.Create(8, return_object);
            case ConsoleKey.D9:
                // High-pass filter frequency
                return_object.Values = "Integer 0<=";
                return_object.Name = "High-pass filter frequency";
                return Tuple.Create(9, return_object);
            case ConsoleKey.Q:
                // Wualti factor
                return_object.Values = "Integer 0<=";
                return_object.Name = "High-pass quality factor";
                return Tuple.Create(10, return_object);
            case ConsoleKey.W:
                // Low-pass filter
                return_object.Values = "true | false";
                return_object.Name = "Low-pass filter";
                return Tuple.Create(11, return_object);
            case ConsoleKey.E:
                // Low-pass filter frequency
                return_object.Values = "Integer 0<=";
                return_object.Name = "Low-pass filter frequency";
                return Tuple.Create(12, return_object);
            case ConsoleKey.R:
                // Wualti factor
                return_object.Values = "Integer 0<=";
                return_object.Name = "Low-pass quality factor";
                return Tuple.Create(13, return_object);
            case ConsoleKey.T:
                // Revert setting changes
                return_object.Values = "true | false";
                return_object.Name = "Revert settings";
                return Tuple.Create(14, return_object);
                break;
            case ConsoleKey.Y:
                // Revert setting changes
                return_object.Values = "true | false";
                return_object.Name = "Quiet startup";
                return Tuple.Create(15, return_object);
    
            case ConsoleKey.Escape:
                return Tuple.Create(777, return_object);
            default:
                Console_writing("editing");
                return Tuple.Create(777, return_object);
        }
    }

    private void Console_writing(string what){
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
        switch(what){
            case "editing":
                Console.WriteLine("Settings | Editing");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                HelpTextInfo();
                break;
            case "main":
                Console.WriteLine("Settings | Main");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(info_message);
                Console.WriteLine(help_message);
                break;
            case "error":
                Console.WriteLine("Settings | Main");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(info_message);
                Console.WriteLine("Invalid input. " + help_message);
                break;
            case "help":
                Console.WriteLine("Settings | Main");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                HelpTextInfo();
                break;
        }
        Console.ResetColor();
    }
    private void HelpTextInfo(){
        Console.WriteLine(info_message);
        Console.WriteLine(help_message);
        Console.WriteLine("When editing values, enter setting number/letter and enter value.");
        Console.WriteLine("===========================================================================================================");
        Console.WriteLine("|0| " + data.OnHelpText + ". Current: " + data.On);
        Console.WriteLine("|1| " + data.VolumeHelpText + ". Current: " + data.Volume);
        Console.WriteLine("|2| " + data.BufferMillisecondsHelpText + ". Current: " + data.BufferMilliseconds);
        Console.WriteLine("|3| " + data.NumberOfBuffersHelpText + ". Current: " + data.NumberOfBuffers);
        Console.WriteLine("|4| " + data.BufferLengthHelpText + ". Current: " + data.BufferLength);
        Console.WriteLine("|5| " + data.DiscardOnBufferOverflowHelpText + ". Current: " + data.DiscardOnBufferOverflow);
        Console.WriteLine("|6| " + data.DesiredLatencyHelpText + ". Current: " + data.DesiredLatency);
        Console.WriteLine("|7| " + data.EmptyCacheSecondsHelpText + ". Current: " + data.EmptyCacheSeconds);
        Console.WriteLine("|8| " + data.HighPassOnHelpText + ". Current: " + data.HighPassOn);
        Console.WriteLine("|9| " + data.HighPassFrequencyHelpText + ". Current: " + data.HighPassFrequency);
        Console.WriteLine("|q| " + data.HighPassQualityFactorHelpText + ". Current: " + data.HighPassQualityFactor);
        Console.WriteLine("|w| " + data.LowPassOnHelpText + ". Current: " + data.LowPassOn);
        Console.WriteLine("|e| " + data.LowPassFrequencyHelpText + ". Current: " + data.LowPassFrequency);
        Console.WriteLine("|r| " + data.LowPassQualityFactorHelpText + ". Current: " + data.LowPassQualityFactor);
        Console.WriteLine("|t| " + data.RevertChangesHelpText + ". Current: " + data.RevertChanges);
        Console.WriteLine("|y| " + data.QuietStartMessageHelpText + ". Current: " + data.QuietStartMessage);
        Console.WriteLine("===========================================================================================================");
    }

    public static bool CancelableReadLine(out string value){
        value = string.Empty;
        var buffer = new StringBuilder();
        var key = Console.ReadKey(true);
        while (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Escape)
        {
            if (key.Key == ConsoleKey.Backspace && Console.CursorLeft > 0)
            {
                var cli = --Console.CursorLeft;
                buffer.Remove(cli, 1);
                Console.CursorLeft = 0;
                Console.Write(new String(Enumerable.Range(0, buffer.Length + 1).Select(o => ' ').ToArray()));
                Console.CursorLeft = 0;
                Console.Write(buffer.ToString());
                Console.CursorLeft = cli;
                key = Console.ReadKey(true);
            }
            else if (Char.IsLetterOrDigit(key.KeyChar) || Char.IsWhiteSpace(key.KeyChar))
            {
                var cli = Console.CursorLeft;
                buffer.Insert(cli, key.KeyChar);
                Console.CursorLeft = 0;
                Console.Write(buffer.ToString());
                Console.CursorLeft = cli + 1;
                key = Console.ReadKey(true);
            }
            else if (key.Key == ConsoleKey.LeftArrow && Console.CursorLeft > 0)
            {
                Console.CursorLeft--;
                key = Console.ReadKey(true);
            }
            else if (key.Key == ConsoleKey.RightArrow && Console.CursorLeft < buffer.Length)
            {
                Console.CursorLeft++;
                key = Console.ReadKey(true);
            }
            else
            {
                key = Console.ReadKey(true);
            }
        }

        if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            value = buffer.ToString();
            return true;
        }
        return false;
    }
}

public class ReturnObject{
    public string? Values { get; set; }
    public string? Name { get; set; }
}

