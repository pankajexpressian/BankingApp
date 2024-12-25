using RabbitMQHelper;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 5001; // The port to redirect HTTP traffic to HTTPS (adjust if necessary)
});
builder.Services.AddControllers();
builder.Services.AddSingleton<MQHelper>();
builder.Services.AddGrpc();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(5001, listenOptions =>
//    {
//        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
//    });
//});

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
