using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ToDoList.Data.DatabaseContext;

var builder = WebApplication.CreateBuilder(args);

//stardew((

builder.Services.AddDbContext<ToDoListDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToDoList API",
        Version = "v1",
        Description = "API дл€ управлени€ задачами"
    });

    // ¬ключаем аннотации дл€ более подробной документации
    c.EnableAnnotations();
});

//stardew))

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

//stardew((

app.UseSwagger();

app.UseSwaggerUI();

//stardew))

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
