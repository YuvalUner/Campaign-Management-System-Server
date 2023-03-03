using API.Middleware;
using DAL.DbAccess;
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

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", optionsBuilder => {
        optionsBuilder.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
            //.WithOrigins(builder.Configuration["AllowedOrigins:React"],
            //builder.Configuration["AllowedOrigins:WebApi"], builder.Configuration["AllowedOrigins:WebServer"])
            .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
    });
});


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        // Horrible workaround for making the server return 401 when user is not authorized, instead of 404.
        options.LoginPath = "/api/AccessDenied";
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Adding serilog for logging
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger()
);

var app = builder.Build();

app.UseMiddleware<AntiXssMiddleware>();

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
    MinimumSameSitePolicy = SameSiteMode.Strict,
});

app.UseCors("AllowAll");

app.MapControllers();

app.UseSession();

app.Run();