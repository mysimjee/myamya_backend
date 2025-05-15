using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
app.UseStaticFiles();

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

    if (file == null || file.Length == 0)
        return Results.BadRequest("No file uploaded.");

    var uploadPath = Path.Combine(env.WebRootPath, "uploads");
    Directory.CreateDirectory(uploadPath);

    var hlsOutputPath = Path.Combine(env.WebRootPath, "hls");
    Directory.CreateDirectory(hlsOutputPath);

    var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(file.FileName);
    var inputFilePath = Path.Combine(uploadPath, fileName);

    using (var stream = new FileStream(inputFilePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    var hlsFolderName = Path.GetFileNameWithoutExtension(fileName);
    var outputFolder = Path.Combine(hlsOutputPath, hlsFolderName);
    Directory.CreateDirectory(outputFolder);

    var outputPlaylist = Path.Combine(outputFolder, "output.m3u8");

    var ffmpegArgs =
        $"-i \"{inputFilePath}\" -codec: copy -start_number 0 -hls_time 10 -hls_list_size 0 -f hls \"{outputPlaylist}\"";

    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = ffmpegArgs,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };

    process.Start();
    var stdOut = await process.StandardOutput.ReadToEndAsync();
    var stdErr = await process.StandardError.ReadToEndAsync();
    process.WaitForExit();

    if (process.ExitCode != 0)
    {
        return Results.InternalServerError($"FFmpeg failed: {stdErr}");
    }

    var playlistUrl = $"/hls/{hlsFolderName}/output.m3u8";
    return Results.Ok(new { message = "Video uploaded and converted to HLS successfully", url = playlistUrl });
}).DisableAntiforgery();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


public class UploadVideoForm
{
    public IFormFile UploadedVideo { get; set; }
}