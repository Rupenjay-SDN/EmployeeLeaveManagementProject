

var builder = WebApplication.CreateBuilder(args);

// ✅ Register RabbitMQ listener as background service
builder.Services.AddHostedService<RabbitMqListener>();


// Optional: Swagger support (not needed unless you're exposing APIs)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger UI (optional)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// (Optional) If you want a test endpoint
app.MapGet("/", () => "📢 Notification Service is Running!");

app.Run();
