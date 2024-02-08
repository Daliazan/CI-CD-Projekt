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

// skapar middleware för Swagger (JSON-endpoint och Swagger UI)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

// Skapar endpoints 
app.MapPost("/encrypt", (EncryptionService encryptionService, string plaintext) =>
{
    return Results.Ok(new { encryptedText = encryptionService.Encrypt(plaintext) });
});

app.MapPost("/decrypt", (EncryptionService encryptionService, string encryptedText) =>
{
    return Results.Ok(new { decryptedText = encryptionService.Decrypt(encryptedText) });
});

app.Run();

// Ord/produkter som ska krypteras
public class EncryptionService
{
    private readonly string[] summaries = new[]
    {
        "mascara", "lipgloss", "bronzer", "blush", "consealer", "foundation"
    };
 // ifall annat ord än de angivna skrivs för kryptering, felmeddelande utmatas
    public string Encrypt(string plaintext)
    {
        if (!summaries.Contains(plaintext, StringComparer.OrdinalIgnoreCase))
        {
            return "Wrong word, try again!";
        }

        byte[] textAsBytes = Encoding.UTF8.GetBytes(plaintext);
        return Convert.ToBase64String(textAsBytes);
    }
 
    public string Decrypt(string encryptedText)
    {
        byte[] textAsBytes = Convert.FromBase64String(encryptedText);
        string decodedText = Encoding.UTF8.GetString(textAsBytes);
        
// ifall annat ord än de angivna skrivs för avkryptering, felmeddelande utmatas
        if (!summaries.Contains(decodedText, StringComparer.OrdinalIgnoreCase))
        {
            return "Wrong word, try again!";
        }

        return decodedText;
    }
}
