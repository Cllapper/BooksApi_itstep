var builder = WebApplication.CreateBuilder(args);

// Додавання сервісів до контейнера.
builder.Services.AddControllers();

// === 1. РЕЄСТРАЦІЯ СЕРВІСІВ КЕШУВАННЯ ===

// Реєструємо сервіс для кешування в пам'яті (In-Memory Caching), доступний через IMemoryCache
builder.Services.AddMemoryCache();

// Реєструємо сервіс для кешування відповідей (Response Caching), який використовує атрибут [ResponseCache]
builder.Services.AddResponseCaching();


// === КІНЕЦЬ ЗМІН У СЕРВІСАХ ===

// Сервіси для Swagger/OpenAPI (залиште як є для зручності тестування)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Налаштування конвеєра HTTP-запитів.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// === 2. ПІДКЛЮЧЕННЯ MIDDLEWARE ДЛЯ КЕШУВАННЯ ===

// Middleware для кешування відповідей.
// Він має бути розміщений перед middleware, відповіді яких ми хочемо кешувати (напр., MapControllers)
app.UseResponseCaching();


// === КІНЕЦЬ ЗМІН У MIDDLEWARE ===


app.MapControllers();

app.Run();
