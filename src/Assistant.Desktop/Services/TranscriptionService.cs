using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Assistant.Desktop.Services;

public class TranscriptionService(HttpClient httpClient) : ITranscriptionService
{
    public async Task SaveAsync(string dataUrl)
    {
        // Extract the base64 string from the data URL (remove the "data:audio/wav;base64," part)
        var base64Data = dataUrl.Split(',')[1];

        // Convert the base64 string to a byte array
        byte[] fileData = Convert.FromBase64String(base64Data);

        // Save the byte array as a file (e.g., a .wav file)
        await File.WriteAllBytesAsync("audio.mp3", fileData);
    }
 
    public async Task<string> SendAudioFileAsync(byte[] audioData)
    {
        var myBytes = File.ReadAllBytes("I:\\source\\python\\asr-demos\\audio.webm");
        using var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(audioData);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/webm");
        form.Add(fileContent, "file", "audio.webm");
    
        var response = await httpClient.PostAsync("/transcribe/", form);
        response.EnsureSuccessStatusCode();
    
        return await response.Content.ReadAsStringAsync();
    }
}

public interface ITranscriptionService
{
    Task<string> SendAudioFileAsync(byte[] audioData);
    Task SaveAsync(string dataUrl);
}