using Elepla.API;
using Elepla.Repository.Data;
using Elepla.Repository.Interfaces;
using Elepla.Service.Common;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration.Get<AppConfiguration>();

builder.Services.AddInfrastructuresService(configuration.DatabaseConnection);
builder.Services.AddWebAPIService(configuration.JWT);
builder.Services.AddSingleton(configuration);

var app = builder.Build();

//using (var connection = new SqlConnection(configuration.DatabaseConnection))
//{
//    try
//    {
//        connection.Open();
//        Console.WriteLine("Connection successful.");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Connection failed: {ex.Message}");
//    }
//}

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

////Get swagger.json following root directory 
//app.UseSwagger(options => { options.RouteTemplate = "{documentName}/swagger.json"; });
////Load swagger.json following root directory 
//app.UseSwaggerUI(c => { c.SwaggerEndpoint("/v1/swagger.json", "Elepla API V1"); c.RoutePrefix = string.Empty; });

// Configure the HTTP request pipeline.
////if (app.Environment.IsDevelopment())
////{
////    app.UseSwagger();
////    app.UseSwaggerUI();
////}

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
