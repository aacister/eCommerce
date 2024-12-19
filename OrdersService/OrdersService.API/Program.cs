using FluentValidation.AspNetCore;
using OrdersService.API.Middleware;
using OrdersService.Business;
using OrdersService.Business.HttpClients;
using OrdersService.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessDependencies(builder.Configuration);
builder.Services.AddBusinessDependencies(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();

//Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add cors services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

builder.Services.AddHttpClient<UsersMicroserviceClient>(client => {
    client.BaseAddress = new Uri($"http://{builder.Configuration["UsersMicroserviceName"]}:{builder.Configuration["UsersMicroservicePort"]}");
});

builder.Services.AddHttpClient<ProductsMicroserviceClient>(client => {
    client.BaseAddress = new Uri($"http://{builder.Configuration["ProductsMicroserviceName"]}:{builder.Configuration["ProductsMicroservicePort"]}");
});


var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseRouting();
app.UseCors();

//Swagger
app.UseSwagger(); //Adds endpoints that serve swagger.json
app.UseSwaggerUI();  //Adds swagger ui => interactive page to explore and test API endpoints 


//Auth
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

//Endpoints
app.MapControllers();

app.Run();
