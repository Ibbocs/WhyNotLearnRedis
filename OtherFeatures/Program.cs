using MediatR;
using Microsoft.Extensions.Configuration;
using OtherFeatures.Controllers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var assembly = Assembly.GetExecutingAssembly();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RedisCacheBehevior<,>));
builder.Services.Configure<RedisCacheSettings>(builder.Configuration.GetSection("RedisCacheSettings"));
builder.Services.AddTransient<IRedisCacheService, RedisCacheService>();

//cach ede bilmek ucun Microsoft.Extensions.Caching.StackExchangeRedis yukleyirik
builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = builder.Configuration["RedisCacheSettings:ConnectionString"];
    opt.InstanceName = builder.Configuration["RedisCacheSettings:Instance"];
});


var app = builder.Build();

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
