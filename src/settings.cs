using System;
using System.IO;

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

    public int HighPassFrequency { get; set; }
    public string? HighPassFrequencyHelpText { get; set; }

    public int HighPassQualityFactor { get; set; }
    public string? HighPassQualityFactorHelpText { get; set; }

    public bool HighPassOn {get;set;}
    public string? HighPassOnHelpText {get;set;}

    public int LowPassFrequency { get; set; }
    public string? LowPassFrequencyHelpText { get; set; }

    public int LowPassQualityFactor { get; set; }
    public string? LowPassQualityFactorHelpText { get; set; }

    public bool LowPassOn {get;set;}
    public string? LowPassOnHelpText {get;set;}

    public bool RevertChanges {get;set;}
    public string? RevertChangesHelpText {get;set;}

    public bool QuietStartMessage {get;set;}
    public string? QuietStartMessageHelpText {get;set;}

}

public class Reset_Save{
    // Summary:
    // old | backup
    public Reset_Save(string from_what_file){
        string currentSaveJsonPath = Global.dir_path + "\\save.json";
        string oldFilePath = Global.dir_path + $"\\{from_what_file}_save.json";

        // make copy of old / backup_save.json as save.json
        try{
            if (File.Exists(oldFilePath)){
                File.Copy(oldFilePath, currentSaveJsonPath, true); // true overwrites old file
            }
            else{
                Console.WriteLine($"Source file '{oldFilePath}' does not exist.");
            }
        }
        catch (Exception ex){
            Console.WriteLine($"Error copying file: {ex.Message}");
        }
    }
}
