using Elepla.API;
using Elepla.Repository.Data;
using Elepla.Repository.Interfaces;
using Elepla.Service.Common;

var builder = WebApplication.CreateBuilder(args);

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
