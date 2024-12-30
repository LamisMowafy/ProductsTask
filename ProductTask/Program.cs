using Application;
using Infrastructure;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // This enables the API documentation endpoint
builder.Services.AddSwaggerGen(); // Registers Swagger services
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Register application services from the Application Layer
builder.Services.AddApplicationServices(); // Calling the method from Application Layer
builder.Services.AddInfrastructureServices(builder.Configuration);

WebApplication app = builder.Build();
app.UseRouting();
if (app.Environment.IsDevelopment())
{ 
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();
