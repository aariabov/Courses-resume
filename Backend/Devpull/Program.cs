using System.Text;
using System.Text.Json.Serialization;
using Devpull;
using Devpull.Articles;
using Devpull.ClientCode;
using Devpull.Controllers;
using Devpull.Controllers.Models;
using Devpull.Course;
using Devpull.DbModels;
using Devpull.Exercises;
using Devpull.Logs;
using Devpull.Payments;
using Devpull.Users;
using Devpull.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Bind and validate application configuration
builder.Services.AddSingleton<AppConfig>();

builder.Services.AddHealthChecks();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<ClientCodeService>();
builder.Services.AddScoped<ClientCodeCacheService>();
builder.Services.AddScoped<IClientCodeService, ClientCodeService>();

var useCache = builder.Configuration.GetValue<bool>("UseCache");
if (useCache)
{
    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
}
else
{
    builder.Services.AddSingleton<ICacheService, FakeCacheService>();
}

builder.Services.AddScoped<ExecutionService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<LoginModelValidator>();
builder.Services.AddScoped<LogoutModelValidator>();
builder.Services.AddScoped<RegisterModelValidator>();
builder.Services.AddScoped<ConfirmEmailModelValidator>();
builder.Services.AddScoped<RefreshTokenModelValidator>();
builder.Services.AddScoped<FunctionTestModelValidator>();
builder.Services.AddScoped<ForgotPasswordModelValidator>();
builder.Services.AddScoped<ResetPasswordModelValidator>();
builder.Services.AddScoped<ClientCodeModelValidator>();
builder.Services.AddScoped<AnalyzeCodeModelValidator>();
builder.Services.AddScoped<IUserManagerService, UserManagerService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<TokenRepository>();
builder.Services.AddPayment();
builder.Services.AddCourse();
builder.Services.AddArticle();
builder.Services.AddExercise();

if (builder.Environment.IsEnvironment("Production"))
{
    builder.Services.AddScoped<IEmailSender, EmailSender>();
    builder.Services.AddScoped<IIdempotenceKeyGenerator, IdempotenceKeyGenerator>();
}
else
{
    builder.Services.AddScoped<IEmailSender, FakeEmailSender>();
    builder.Services.AddScoped<IIdempotenceKeyGenerator, FakeIdempotenceKeyGenerator>();
}

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddScoped<ICodeGenerator, FakeCodeGenerator>();
}
else
{
    builder.Services.AddScoped<ICodeGenerator, CodeGenerator>();
}

// 3. Добавить аутентификацию с JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(
    jwtSection["Key"] ?? throw new InvalidOperationException("Missing config Jwt:Key")
);
var issuer =
    jwtSection["Issuer"] ?? throw new InvalidOperationException("Missing config Jwt:Issuer");
var audience =
    jwtSection["Audience"] ?? throw new InvalidOperationException("Missing config Jwt:Audience");

var connectionString = builder.Configuration.GetConnectionString("Devpull");
builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connectionString));

var columnWriters = new Dictionary<string, ColumnWriterBase>
{
    { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
    { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
    { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
    { "raise_date", new UtcTimestampColumnWriter() },
    { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
    { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
    { "user_id", new SinglePropertyColumnWriter("UserId", PropertyWriteMethod.Raw) },
    { "request_id", new SinglePropertyColumnWriter("RequestId") },
    {
        "elapsed_ms",
        new SinglePropertyColumnWriter("ElapsedMs", PropertyWriteMethod.Raw, NpgsqlDbType.Integer)
    },
    // { "ip_address", new SinglePropertyColumnWriter("IpAddress") },
    // { "user_agent", new SinglePropertyColumnWriter("UserAgent") },
    // { "elapsed_ms", new SinglePropertyColumnWriter("ElapsedMs", dbType: NpgsqlDbType.Integer) }
};

// для дебага Serilog
// Serilog.Debugging.SelfLog.Enable(msg => Console.Error.WriteLine(msg));
Log.Logger = new LoggerConfiguration()
// Verbose, Debug, Information, Warning, Error, Fatal
// вынес в конфиг
// .MinimumLevel.Information()
// .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
// .MinimumLevel.Override("System", LogEventLevel.Warning)
.ReadFrom
    .Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}"
    )
    .WriteTo.PostgreSQL(
        connectionString: connectionString,
        tableName: "logs",
        columnOptions: columnWriters,
        needAutoCreateTable: false // true если хочешь, чтобы таблица создавалась автоматически
    )
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services
    .AddIdentity<AppUser, IdentityRole>(o =>
    {
        o.Password.RequireDigit = false;
        o.Password.RequireLowercase = false;
        o.Password.RequireUppercase = false;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequiredLength = 1;
        o.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero; // Проверять при каждом запросе
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<RequireValueTypePropertiesSchemaFilter>(true);
    options.SupportNonNullableReferenceTypes();
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

var app = builder.Build();
app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
await dbContext.Database.MigrateAsync();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

public partial class Program { }
