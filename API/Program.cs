using API.ExternalProcesses;
using API.Middleware;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using RestAPIServices;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adding all the many, many services
builder.Services.AddScoped<IGenericDbAccess, GenericDbAccess>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IVotersLedgerService, VotersLedgerService>();
builder.Services.AddScoped<ICampaignsService, CampaignsService>();
builder.Services.AddScoped<IInvitesService, InvitesService>();
builder.Services.AddScoped<IPermissionsService, PermissionsService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<INotificationsService, NotificationsService>();
builder.Services.AddScoped<IEmailSendingService, EmailSendingService>();
builder.Services.AddScoped<ISmsMessageSendingService, SmsMessageSendingService>();
builder.Services.AddScoped<IVerificationCodeService, VerificationCodeService>();
builder.Services.AddScoped<IJobsService, JobsService>();
builder.Services.AddScoped<IJobTypesService, JobTypesService>();
builder.Services.AddScoped<IJobTypeAssignmentCapabilityService, JobTypeAssignmentCapabilityService>();
builder.Services.AddScoped<IJobAssignmentCapabilityService, JobAssignmentCapabilityService>();
builder.Services.AddScoped<IJobPreferencesService, JobPreferencesService>();
builder.Services.AddScoped<ISmsMessageService, SmsMessageService>();
builder.Services.AddScoped<IEventsService, EventsService>();
builder.Services.AddScoped<IScheduleManagersService, ScheduleManagersService>();
builder.Services.AddScoped<IPublishingService, PublishingService>();
builder.Services.AddScoped<IPublicBoardService, PublicBoardService>();
builder.Services.AddScoped<IFinancialTypesService, FinancialTypesService>();
builder.Services.AddScoped<IFinancialDataService, FinancialDataService>();
builder.Services.AddScoped<IElectionDayService, ElectionDayService>();
builder.Services.AddScoped<ICitiesService, CitiesService>();
builder.Services.AddScoped<ICustomVotersLedgerService, CustomVotersLedgerService>();
builder.Services.AddScoped<IPythonMlRunner, PythonMlRunner>();

// CORS policy - for now, just allow all. Change as needed
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", optionsBuilder => {
        optionsBuilder.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
            //.WithOrigins(builder.Configuration["AllowedOrigins:React"],
            //builder.Configuration["AllowedOrigins:WebApi"], builder.Configuration["AllowedOrigins:WebServer"])
            .SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrWhiteSpace(origin)) return false;
                // Only add this to allow testing with localhost, remove this line in production!
                if (origin.ToLower().StartsWith("http://localhost")) return true;
                // Insert your production domain here.
                if (origin.ToLower().StartsWith("https://dev.mydomain.com")) return true;
                return false;
            });
    });
});


builder.Services.AddDistributedMemoryCache();

// Session and cookie authentication configuration - do not remove cookie authentication, as the app constantly
// uses HttpContext.Session to store data, and that requires cookie authentication.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        // Horrible workaround for making the server return 401 when user is not authorized, instead of 404.
        options.LoginPath = "/api/AccessDenied";
        options.Cookie.SameSite = SameSiteMode.None;
    });

// builder.Services.ConfigureApplicationCookie(options =>
// {
//     options.Events.OnRedirectToLogin = context =>
//     {
//         context.Response.StatusCode = 401;
//         return Task.CompletedTask;
//     };
// });

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Adding serilog for logging
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger()
);

var app = builder.Build();

// // Anti-XSS middleware to prevent XSS attacks
// app.UseMiddleware<AntiXssMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always,
});

app.UseCors("AllowAll");

app.MapControllers();

app.UseSession();

// Exclude the import endpoint from the anti-xss middleware, as it is used to import data from a file and 
// the middleware will prevent that from happening.
app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/api/CustomVotersLedger/import"),
    appBuilder =>
    {
        appBuilder.UseMiddleware<AntiXssMiddleware>();
    }
);

app.Run();