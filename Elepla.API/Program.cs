using Elepla.API;
using Elepla.Repository.Data;
using Elepla.Repository.Interfaces;
using Elepla.Service.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");
builder.Services.AddHealthChecks();

var configuration = builder.Configuration.Get<AppConfiguration>();

builder.Services.AddInfrastructuresService(configuration.DatabaseConnection);
builder.Services.AddWebAPIService(configuration.JWT);
builder.Services.AddSingleton(configuration);

var app = builder.Build();

// Call this method to seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Lấy SeedData và các repository từ service container
    var seedData = services.GetRequiredService<SeedData>();
    var unitOfWork = services.GetRequiredService<IUnitOfWork>();

    // Khởi tạo dữ liệu ban đầu cho vai trò và tài khoản
    await seedData.Initialize(unitOfWork);
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint("/swagger/v1/swagger.json", "Elepla API");
	options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks("/health");

app.Run();
