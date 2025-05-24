using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using myamyafansite_back_end.Database;

var builder = WebApplication.CreateBuilder(args);



// Remove payload limit
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});

// (Optional) Increase Kestrel request size limit
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = long.MaxValue;
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddDbContext<MyamyaFanSiteDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bsc API",
        Version = "v1",
        Description = "This is a simple implementation of a backend API for a fan site dedicated to Mya Mya, a popular adult content creator in Myanmar."
    });
});




var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapSwagger();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        var value = builder.Configuration.GetValue<string>("Testing");
        Console.WriteLine($"name: {value}");
        return forecast;
    })
    .WithName("GetWeatherForecast");



app.MapPost("/upload-video", async (HttpRequest request, [FromForm] UploadVideoForm model, IWebHostEnvironment env) =>
{
    var file = model.UploadedVideo;

    if (file.Length == 0)
        return Results.BadRequest("No file uploaded.");

    var uploadPath = Path.Combine(env.WebRootPath, "uploads");
    Directory.CreateDirectory(uploadPath);

    var hlsOutputPath = Path.Combine(env.WebRootPath, "hls");
    Directory.CreateDirectory(hlsOutputPath);

    var fileName = Path.GetFileNameWithoutExtension(file.FileName) + Path.GetExtension(file.FileName);
    var inputFilePath = Path.Combine(uploadPath, fileName);

    await using (var stream = new FileStream(inputFilePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    var hlsFolderName = Path.GetFileNameWithoutExtension(fileName);
    var outputFolder = Path.Combine(hlsOutputPath, hlsFolderName);
    Directory.CreateDirectory(outputFolder);

    // Create variant streams
    var variants = new[]
    {
        new { Bitrate = "500k", Width = 640, Height = 360 },
        new { Bitrate = "1000k", Width = 854, Height = 480 },
        new { Bitrate = "1500k", Width = 1280, Height = 720 }
    };

    var variantTasks = variants.Select(async variant =>
    {
        var variantName = $"{variant.Height}p";
        var variantFolder = Path.Combine(outputFolder, variantName);
        Directory.CreateDirectory(variantFolder);

        var variantPlaylistPath = Path.Combine(variantFolder, "index.m3u8");

        var args = $"-i \"{inputFilePath}\" " +
                   $"-vf scale={variant.Width}:{variant.Height} " +
                   $"-c:a aac -ar 48000 -c:v h264 -profile:v main -crf 20 -sc_threshold 0 " +
                   $"-g 48 -keyint_min 48 -b:v {variant.Bitrate} -maxrate {variant.Bitrate} -bufsize 1000k " +
                   $"-hls_time 10 -hls_playlist_type vod -hls_segment_filename \"{variantFolder}/segment_%03d.ts\" " +
                   $"\"{variantPlaylistPath}\"";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var err = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception($"FFmpeg failed for {variantName}: {err}");
        }

        return new
        {
            Resolution = variant.Height,
            Path = $"/hls/{hlsFolderName}/{variantName}/index.m3u8",
            Bandwidth = int.Parse(variant.Bitrate.Replace("k", "")) * 1000
        };
    });

    var results = await Task.WhenAll(variantTasks);

    // Create master playlist
    var masterPlaylistPath = Path.Combine(outputFolder, "master.m3u8");
    await using var masterWriter = new StreamWriter(masterPlaylistPath);

    masterWriter.WriteLine("#EXTM3U");

    foreach (var stream in results)
    {
        masterWriter.WriteLine($"#EXT-X-STREAM-INF:BANDWIDTH={stream.Bandwidth},RESOLUTION={stream.Resolution}x{(stream.Resolution * 16 / 9)}");
        masterWriter.WriteLine($"./{stream.Resolution}p/index.m3u8");
    }

    var playlistUrl = $"/hls/{hlsFolderName}/master.m3u8";
    return Results.Ok(new { url = playlistUrl });
}).DisableAntiforgery();


string GetContentType(string filename)
{
    var ext = Path.GetExtension(filename).ToLowerInvariant();
    return ext switch
    {
        ".m3u8" => "application/vnd.apple.mpegurl",
        ".ts" => "video/mp2t",
        _ => "application/octet-stream"
    };
}





app.MapGet("/api/hls/{**filePath}", (HttpContext context, string filePath) =>
{
    var hlsRoot = Path.Combine(builder.Environment.WebRootPath, "hls");
    var fullPath = Path.Combine(hlsRoot, filePath);

    if (!System.IO.File.Exists(fullPath))
        return Results.NotFound("File not found.");

    var contentType = GetContentType(fullPath);

    var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    return Results.Stream(stream, contentType);
});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


public class UploadVideoForm
{
    public IFormFile UploadedVideo { get; set; }
}