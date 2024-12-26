using AccountService.Services;
using RabbitMQHelper;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 5001; 
});

builder.Services.AddControllers();

builder.Services.AddSingleton<AccounManagementService>();

builder.Services.AddSingleton<MQHelper>();

builder.Services.AddGrpc();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();



var app = builder.Build();

app.MapGrpcService<AccountServiceImpl>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
