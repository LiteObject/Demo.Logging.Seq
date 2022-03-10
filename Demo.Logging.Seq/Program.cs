using Demo.Logging.Seq.Filters;
using Demo.Logging.Seq.Options;
using Demo.Logging.Seq.Services;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddSeq(config.GetSection("Seq"));
});

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.Configure<MailOptions>(builder.Configuration.GetSection(MailOptions.Mail));

builder.Services.AddControllers(options =>
{
    // https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-6.0
    options.Filters.Add<HttpResponseExceptionFilter>();
}).ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(context.ModelState)
    {
        ContentTypes =
                {
                    // using static System.Net.Mime.MediaTypeNames;
                    Application.Json,
                    Application.Xml
                }
    };

});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error-development");
}
else
{
    app.UseExceptionHandler("/error");
}

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
