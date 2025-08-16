using Hangfire;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Services.Authentication;
using SurveyBasket.Api.Services.Mail;
using SurveyBasket.Api.Services.Polls;
using SurveyBasket.Api.Services.Questions;
using SurveyBasket.Api.Services.Results;
using SurveyBasket.Api.Services.Roles;
using SurveyBasket.Api.Services.User;
using SurveyBasket.Api.Services.Votes;
using SurveyBasket.Api.Settings;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Api.Extensions;
using SurveyBasket.Api.HealthCheck;


namespace SurveyBasket.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddAuthConfig(configuration);
        var allowOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();
        services.AddCors(options =>
           {
               options.AddPolicy("AllowAll", builder =>
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                );
               options.AddPolicy("MyPolicy", builder =>
                   builder
                   .WithOrigins(allowOrigins!)
                   .AllowAnyMethod()       // or specify methode: .WithMethods("Get","Put")
                   .AllowAnyHeader()      //  or specify headers: .WithHeaders(HeaderNames.ContentType, "")

                );
           }
        );

        //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddSwaggerServices()
            .AddMapsterConfig()
            .AddFluentValidationConfig();

        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not found");


        services.AddHangfireConfig(configuration);

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IResultService, ResultService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();


        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionRequirementHandler>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddHttpContextAccessor();
        services.AddRateLimiting();

        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
        
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>(name : "database")
            .AddHangfire(options =>
            {
                options.MinimumAvailableServers = 1;
                
            })
            .AddCheck<MailProviderHealthCheck>(name :"mail provider");

        

        return services;
    }

    private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        return services;
    }

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfig));
        return services;
    }

    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services
           .AddFluentValidationAutoValidation()
           .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


        services.AddSingleton<IJwtProvider, JwtProvider>();

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();


        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        })
        .AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                ValidIssuer = jwtSettings?.Issuer,
                ValidAudience = jwtSettings?.Audience,
            };
        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        });


        return services;
    }
    private static IServiceCollection AddHangfireConfig(this IServiceCollection services, IConfiguration configuration)
    {
        // Hangfire services.
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        // Add the processing server as IHostedService
        services.AddHangfireServer();
        return services;
    }

    private static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // rateLimiterOptions.AddPolicy(RateLimitPolicies.IpLimiter, httpContext =>
            //     RateLimitPartition.GetFixedWindowLimiter(
            //         partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            //         factory: _ => new FixedWindowRateLimiterOptions
            //         {
            //             PermitLimit = 5,
            //             Window = TimeSpan.FromSeconds(30)
            //         }
            //     )
            // );
            
            rateLimiterOptions.AddPolicy(RateLimitPolicies.UserLimiter, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.GetUserId(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromSeconds(30)
                    }
                )
            );
            
            rateLimiterOptions.AddConcurrencyLimiter(RateLimitPolicies.Concurrency, options =>
            {
                options.PermitLimit = 3;
                options.QueueLimit = 1;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            // rateLimiterOptions.AddTokenBucketLimiter(RateLimitPolicies.TokenBucket,
            //     options =>
            //     {
            //         options.TokenLimit = 2;
            //         options.QueueLimit = 1;
            //         options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //         options.TokensPerPeriod = 2;
            //         options.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
            //         options.AutoReplenishment = true;
            //     });

            // rateLimiterOptions.AddFixedWindowLimiter(RateLimitPolicies.FixedWindow,options =>
            // {
            //     options.PermitLimit = 2;
            //     options.QueueLimit = 1;
            //     options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //     options.Window =  TimeSpan.FromSeconds(30);
            // });

            // rateLimiterOptions.AddSlidingWindowLimiter(RateLimitPolicies.SlidingWindow, options =>
            // {
            //     options.PermitLimit = 2;
            //     options.Window =  TimeSpan.FromSeconds(30);
            //     options.QueueLimit = 1;
            //     options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //     options.SegmentsPerWindow = 2;
            // });


        });
        return services;
    }
}
