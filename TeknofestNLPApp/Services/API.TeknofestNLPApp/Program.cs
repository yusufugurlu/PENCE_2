using API.TeknofestNLPApp.Concretes;
using API.TeknofestNLPApp.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    var cors = builder.Configuration.GetSection("AllowCorsUrl").Get<string[]>();
    options.AddPolicy("_testPolicy", builder =>
    builder
    .WithOrigins(cors)
    .WithMethods("GET", "POST", "OPTIONS")
    .AllowAnyHeader()
    .AllowCredentials()
    .SetPreflightMaxAge(TimeSpan.FromSeconds(500)));

});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//Custom depency injection
builder.Services.AddScoped<ICommentService,CommentManager>();
builder.Services.AddScoped<ITrendyolModelService, TrendyolModelManager>();
builder.Services.AddScoped<IN11Service, N11Manager>();
builder.Services.AddScoped<IModelService, ModelManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("_testPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
