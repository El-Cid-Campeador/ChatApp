using System.Text.Json;
using ApiServer.Repositories;
using ApiServer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy(name: "AllowSpecificOrigins", policy =>{
        policy.WithOrigins("http://localhost:5173")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(options => { 
        options.DefaultScheme = "Cookies"; 
    }).AddCookie("Cookies", options => {
        options.Cookie.Name = "auth_cookie";
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.Cookie.HttpOnly = true;
        // options.ExpireTimeSpan = new TimeSpan(0, 20, 0);
        // options.Cookie.MaxAge = options.ExpireTimeSpan;

        options.Events = new CookieAuthenticationEvents {                          
            OnRedirectToLogin = redirectContext => {
                redirectContext.HttpContext.Response.StatusCode = 401;

                return Task.CompletedTask;
            }
        };                
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<ChatService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
