using Configuration;
using Configuration.Extensions;
using DbContext.Extensions;
using DbRepos;
using Services;

var builder = WebApplication.CreateBuilder(args);

// NOTE: global cors policy needed for JS and React frontends
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder
    .Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
            .Json
            .ReferenceLoopHandling
            .Ignore
    );
builder.Services.AddEndpointsApiExplorer();

#region Initializing the standard sw stack using extensions
builder.Configuration.AddSecrets(builder.Environment);
builder.Services.AddEncryptions(builder.Configuration);
builder.Services.AddDatabaseConnections(builder.Configuration);
builder.Services.AddVersionInfo();
builder.Services.AddInMemoryLogger();
builder.Services.AddUserBasedDbContext();
#endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new()
        {
            Title = "TravelGuide API v1.0",
            Version = "v1.0",
            Description =
                "This is an API created for a school project involving ASP.NET, Entity Framework Core & Architecture."
                + $"<br>DataSet: {builder.Configuration["DatabaseConnections:UseDataSetWithTag"]}"
                + $"<br>DefaultDataUser: {builder.Configuration["DatabaseConnections:DefaultDataUser"]}",
        }
    );
});

//Add InMemoryLoggerProvider logger
builder.Services.AddInMemoryLogger();

//Inject DbRepos and Services
builder.Services.AddScoped<AdminDbRepos>();

builder.Services.AddScoped<IAdminService, AdminServiceDb>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// for the purpose of this example, we will use Swagger also in production
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelGuide API v1.0");
    });
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();
app.MapControllers();

app.Run();
