using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// skapar metod för swagger 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

// Registrera EncryptionService som en singleton-tjänst
builder.Services.AddSingleton<EncryptionService>();

var app = builder.Build();

// Aktivera middleware för att betjäna Swagger som JSON-endpoint och Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

// Definiera dina endpoints här
app.MapPost("/encrypt", (EncryptionService encryptionService, string plaintext) =>
{
    return Results.Ok(new { encryptedText = encryptionService.Encrypt(plaintext) });
});

app.MapPost("/decrypt", (EncryptionService encryptionService, string encryptedText) =>
{
    return Results.Ok(new { decryptedText = encryptionService.Decrypt(encryptedText) });
});

app.Run();

public class EncryptionService
{
    private readonly string[] summaries = new[]
    {
        "mascara", "lipgloss", "bronzer", "blush", "consealer", "foundation"
    };

    public string Encrypt(string plaintext)
    {
        if (!summaries.Contains(plaintext, StringComparer.OrdinalIgnoreCase))
        {
            return "Unauthorized word";
        }

        byte[] textAsBytes = Encoding.UTF8.GetBytes(plaintext);
        return Convert.ToBase64String(textAsBytes);
    }

    public string Decrypt(string encryptedText)
    {
        byte[] textAsBytes = Convert.FromBase64String(encryptedText);
        string decodedText = Encoding.UTF8.GetString(textAsBytes);

        if (!summaries.Contains(decodedText, StringComparer.OrdinalIgnoreCase))
        {
            return "Unauthorized word";
        }

        return decodedText;
    }
}
