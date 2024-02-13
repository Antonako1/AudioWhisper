using System.Text.Json;
using System;
using System.IO;
using System.Text;
using NAudio.Wave.SampleProviders;

class File_reader{
    private readonly string source_path = "save.json";
    private readonly string help_message = "Press '1' to edit values, '2' for help, '3' to return";
    private readonly string help_message_edit = "Enter setting number and give value. press 'enter' to save, 'esc' to return";
    private readonly string info_message = "Changes are saved automatically. They will be applied once you return to 'Main | Main'";
    public JsonObject? data;
    public File_reader(){
        Read_file();
    }
    /*
    Show mute on/off @ Main | Main
    On background play (oikee alakulma)
    */
    public void Cli(){     
        bool run_settings = true;
        Read_file(); 
        Console_writing("main");

        while(run_settings){
            var key = Console.ReadKey().Key;
            switch (key){
                case ConsoleKey.D1:
                    Console_writing("editing");
                    while(true){
                        key = Console.ReadKey().Key;
                        (int result, ReturnObject placehold) = Handle_setting(key);
                        if(result == 0){
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
                    break;
            }
        }
    }

    public void Read_file(){
        string json_data_as_string = File.ReadAllText(source_path);
        this.data = JsonSerializer.Deserialize<JsonObject>(json_data_as_string);
    }
    private void Write_file(){
        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };
        string json_data_as_string = JsonSerializer.Serialize(data, options);
        File.WriteAllText(source_path, json_data_as_string);;
    }
    private int Save_changes(int what, string? line){
        switch (what){
            case 1: if(bool.TryParse(line, out bool parse1)){data.On = parse1;return 0;}else{Console.WriteLine($"Failed to parse '{line}' as bool. Given value may be incorrect."); return 1;}
            case 2: if(int.TryParse(line, out int parse2)){ if(parse2 <= 100){data.Volume = parse2; return 0;}else{return 1;}}else{Console.WriteLine($"Failed to parse '{line}' as int. Given value may be incorrect."); return 1;}
            case 3: if(int.TryParse(line, out int parse3)){data.BufferMilliseconds = parse3;return 0;}else{Console.WriteLine($"Failed to parse '{line}' as int. Given value may be incorrect."); return 1;}
            case 4: if(int.TryParse(line, out int parse4)){data.NumberOfBuffers = parse4;return 0;}else{Console.WriteLine($"Failed to parse '{line}' as int. Given value may be incorrect."); return 1;}
            case 5: if(int.TryParse(line, out int parse5)){data.BufferLength = parse5;return 0;}else{Console.WriteLine($"Failed to parse '{line}' as int. Given value may be incorrect."); return 1;}
            case 6: if(bool.TryParse(line, out bool parse6)){data.DiscardOnBufferOverflow = parse6;return 0;}else{Console.WriteLine($"Failed to parse '{line}' as bool. Given value may be incorrect."); return 1;}
            case 7: if(int.TryParse(line, out int parse7)){data.DesiredLatency = parse7;return 0;}else{Console.WriteLine($"Failed to parse '{line}' as int. Given value may be incorrect."); return 1;}
            case 8: if(int.TryParse(line, out int parse8)){data.EmptyCacheSeconds = parse8;return 0;}else{Console.WriteLine($"Failed to parse '{line}' as int. Given value may be incorrect."); return 1;}
            default: Console.WriteLine("Error parsing"); return 1;
        }
    }

    private Tuple<int, ReturnObject> Handle_setting(System.ConsoleKey setting_number){
        ReturnObject return_object = new()
        {
            Values = "none",
            Name = "nothing"
        };
        switch (setting_number){
            case ConsoleKey.D1:
                // On off
                return_object.Values = "true | false";
                return_object.Name = "On/Off";
                return Tuple.Create(1, return_object);
            case ConsoleKey.D2:
                // Playback volume
                return_object.Values = "Integer 0-100";
                return_object.Name = "Volume";
                return Tuple.Create(2, return_object);
            case ConsoleKey.D3:
                // Chunk length
                return_object.Values = "Integer 0<";
                return_object.Name = "Chunk length";
                return Tuple.Create(3, return_object);
            case ConsoleKey.D4:
                // Chunk overlap
                return_object.Values = "Integer 0<";
                return_object.Name = "Chunk overlap";
                return Tuple.Create(4, return_object);
            case ConsoleKey.D5:
                // Chunk overlap
                return_object.Values = "Integer 0<";
                return_object.Name = "Buffer size";
                return Tuple.Create(5, return_object);
            case ConsoleKey.D6:
                // Buffer overflow
                return_object.Values = "true | false";
                return_object.Name = "Buffer overflow";
                return Tuple.Create(6, return_object);
            case ConsoleKey.D7:
                // Latency
                return_object.Values = "Integer 0<";
                return_object.Name = "Latency";
                return Tuple.Create(7, return_object);
            case ConsoleKey.D8:
                // Cache emptyrate
                return_object.Values = "Integer 0<";
                return_object.Name = "Cache emptier";
                return Tuple.Create(8, return_object);
            case ConsoleKey.Escape:
                return Tuple.Create(0, return_object);
            default:
                Console_writing("editing");
                return Tuple.Create(0, return_object);
        }
    }

    private void Console_writing(string what){
        Console.Clear();
        Console.WriteLine("Hermes | Currently in:");
        switch(what){
            case "editing":
                Console.WriteLine("Settings | Editing");
                Console.WriteLine(info_message);
                Console.WriteLine(help_message_edit);
                Console.WriteLine("|1| " + data?.OnHelpText + ". Current: " + data?.On);
                Console.WriteLine("|2| " + data?.VolumeHelpText + ". Current: " + data?.Volume);
                Console.WriteLine("|3| " + data?.BufferMillisecondsHelpText + ". Current: " + data?.BufferMilliseconds);
                Console.WriteLine("|4| " + data?.NumberOfBuffersHelpText + ". Current: " + data?.NumberOfBuffers);
                Console.WriteLine("|5| " + data?.BufferLengthHelpText + ". Current: " + data?.BufferLength);
                Console.WriteLine("|6| " + data?.DiscardOnBufferOverflowHelpText + ". Current: " + data?.DiscardOnBufferOverflow);
                Console.WriteLine("|7| " + data?.DesiredLatencyHelpText + ". Current: " + data?.DesiredLatency);
                Console.WriteLine("|8| " + data?.EmptyCacheSecondsHelpText + ". Current: " + data?.EmptyCacheSeconds);
                break;
            case "main":
                Console.WriteLine("Settings | Main");
                Console.WriteLine(info_message);
                Console.WriteLine(help_message);
                break;
            case "error":
                Console.WriteLine("Settings | Main");
                Console.WriteLine(info_message);
                Console.WriteLine("Invalid input. " + help_message);
                break;
            case "help":
                Console.WriteLine("Settings | Main");
                Console.WriteLine(info_message);
                Console.WriteLine(help_message);
                Console.WriteLine("|1| " + data.OnHelpText + ". Current: " + data.On);
                Console.WriteLine("|2| " + data.VolumeHelpText + ". Current: " + data.Volume);
                Console.WriteLine("|3| " + data.BufferMillisecondsHelpText + ". Current: " + data.BufferMilliseconds);
                Console.WriteLine("|4| " + data.NumberOfBuffersHelpText + ". Current: " + data.NumberOfBuffers);
                Console.WriteLine("|5| " + data.BufferLengthHelpText + ". Current: " + data.BufferLength);
                Console.WriteLine("|6| " + data.DiscardOnBufferOverflowHelpText + ". Current: " + data.DiscardOnBufferOverflow);
                Console.WriteLine("|7| " + data.DesiredLatencyHelpText + ". Current: " + data.DesiredLatency);
                Console.WriteLine("|8| " + data.EmptyCacheSecondsHelpText + ". Current: " + data.EmptyCacheSeconds);
                break;
        }
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

public class JsonObject{
    public bool On { get; set; }
    public string? OnHelpText { get; set; }

    public int Volume { get; set; }
    public string? VolumeHelpText { get; set; }

    public int BufferMilliseconds { get; set; }
    public string? BufferMillisecondsHelpText { get; set; }

    public int NumberOfBuffers { get; set; }
    public string? NumberOfBuffersHelpText { get; set; }

    public int BufferLength { get; set; }
    public string? BufferLengthHelpText { get; set; }

    public bool DiscardOnBufferOverflow { get; set; }
    public string? DiscardOnBufferOverflowHelpText { get; set; }

    public int DesiredLatency { get; set; }
    public string? DesiredLatencyHelpText { get; set; }

    public int EmptyCacheSeconds { get; set; }
    public string? EmptyCacheSecondsHelpText { get; set; }
}
