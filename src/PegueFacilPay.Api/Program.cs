using System.Reflection;
using Microsoft.OpenApi.Models;
using PegueFacilPay.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PegueFácil Pay API",
        Version = "v1",
        Description = "API de pagamentos do PegueFácil",
        Contact = new OpenApiContact
        {
            Name = "Equipe PegueFácil",
            Email = "contato@peguefacil.com.br"
        }
    });

    // Configura o Swagger para usar os comentários XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Configure Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PegueFácil Pay API v1");
        c.DefaultModelsExpandDepth(-1); // Oculta os modelos por padrão
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); // Expande a lista de endpoints
    });
}

// Enable CORS
app.UseCors("AllowAll");

// Configure HTTP and HTTPS
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Configure URLs
var urls = new List<string> { "http://localhost:5058", "https://localhost:7058" };
app.Urls.Clear();
foreach (var url in urls)
{
    app.Urls.Add(url);
}

app.Run();