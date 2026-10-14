# FactorioGuide

Вікі-довідник по грі **Factorio 2.1** з адмін-панеллю, REST API та мобільним застосунком. Навчальний проєкт із 12 завдань (ASP.NET Core MVC + REST API + .NET MAUI).

## Предметна область

- **Items** - предмети й споруди гри: назва, категорія, опис, розмір стека, час крафту.
- **Users** - зареєстровані користувачі застосунку. Їх створює адміністратор у панелі (реєстрації через API поки немає).
- **Actions** - запити користувачів змінити опис предмета (Add / Edit / Delete) зі статусами Pending, Approved, Rejected. Адміністратор розглядає будь-який запит будь-коли. Схвалення змінює опис предмета, а `Items.UserId` запам'ятовує автора останньої схваленої правки.
- Адміністратор один, його дані задані в коді.
- Додаткові таблиці: `RecipeIngredients` (рецепти) і `Blueprints` (креслення; функціонал креслень поки не реалізований).

## Структура репозиторію

- `Server/Factorio.Domain` - сутності й enum, без залежностей.
- `Server/Factorio.Application` - сценарії використання (сервіси), порти (інтерфейси), Result.
- `Server/Factorio.Infrastructure` - EF Core + SQLite, репозиторії, JWT, BCrypt.
- `Server/ServerApp` - MVC-адмін-панель, REST API, Swagger.
- `Client/ClientApp` - .NET MAUI застосунок (MVVM).
- `Database/init.sql` - SQL-скрипт схеми й тестових даних.
- `Docs` - `init.sql`, `swagger.json`, `Testing/TESTING.md` (результати тестування).

## Архітектура

Clean Architecture: залежності спрямовані до ядра (ServerApp та Infrastructure залежать від Application, Application - від Domain).

- **Ports & Adapters:** інтерфейси (`IUserRepository`, `IItemRepository`, `IActionRepository`, `IPasswordHasher`, `IJwtTokenService`) оголошені в Application, реалізації - в Infrastructure.
- **Repository + сервіси:** уся бізнес-логіка в `UserService`, `ItemService`, `ActionService`, `AuthService`; контролери тонкі.
- **Result:** сервіси повертають результат (успіх, "не знайдено" або помилка поля) замість винятків.
- **DTO / FormModel:** API й форми не віддають сутності напряму (наприклад, без `PasswordHash`).
- **Options pattern і DI:** `JwtSettings`, реєстрація залежностей в `DependencyInjection`.
- **MVVM** у MAUI (CommunityToolkit.Mvvm), сервіси `ApiService`, `ITokenStore`, `INavigationService`.

## Запуск сервера

Потрібно: .NET 10 SDK, Visual Studio 2026 (навантаження ASP.NET and web development та .NET MAUI), Android-емулятор.

1. Клонувати репозиторій.
2. Створити конфігурацію: скопіювати `Server/ServerApp/appsettings.Example.json` у `Server/ServerApp/appsettings.json` і замінити `Jwt:Key` на довгий випадковий рядок (не менше 32 символів). Файл `appsettings.json` не потрапляє в git.
3. Створити базу (з кореня репозиторію):

```powershell
dotnet tool install --global dotnet-ef
dotnet ef database update --project Server/Factorio.Infrastructure --startup-project Server/ServerApp
```

   З'явиться файл `factorio.db` з тестовими даними. Альтернатива: виконати `Database/init.sql` у SQLite.

4. Запустити сервер:

```powershell
dotnet run --project Server/ServerApp
```

5. Відкрити адмін-панель `http://localhost:5003` і Swagger `http://localhost:5003/swagger`.

Необов'язково: оновити назви й описи предметів з англійської локалізації гри (файл `...\Factorio\data\base\locale\en\base.cfg`):

```powershell
dotnet run --project Server/ServerApp -- import-locale "ШЛЯХ\до\base.cfg"
```

## Запуск MAUI-застосунку

Спочатку запустити сервер. У Visual Studio відкрити `Client/ClientApp/ClientApp.slnx`, обрати Android-емулятор і натиснути Run. Емулятор звертається до сервера за адресою `10.0.2.2:5003` (див. `ApiConfig.cs`).

## Облікові дані (демо)

| Роль | Логін | Пароль |
|------|-------|--------|
| Адміністратор (панель) | `admin` | `admin123` |
| Користувач (API, MAUI) | `user1@test.com` | `user123` |

Це демонстраційні значення для навчального проєкту.

## REST API

| Метод | Адреса | Доступ | Опис |
|-------|--------|--------|------|
| POST | `/api/auth/login` | усі | Вхід за email і паролем, повертає JWT |
| GET | `/api/items?search=` | роль User | Список предметів (пошук за назвою або категорією) |
| GET | `/api/items/{id}` | роль User | Один предмет |
| POST | `/api/actions` | роль User | Створити запит на зміну опису, відповідь 202 Accepted (статус Pending) |

Токен передається в заголовку `Authorization: Bearer <token>`. Токен адміністратора до API не допускається (403). Повний опис: Swagger UI (`/swagger`) та `Docs/swagger.json`.

## Тестування

Результати перевірки адмін-панелі, API, бази даних і MAUI: `Docs/Testing/TESTING.md`.

## Відмінності від тексту завдання

- Visual Studio 2026 і .NET 10 замість .NET 8, solution у форматі `.slnx`.
- П'ять таблиць (Users, Items, Actions, RecipeIngredients, Blueprints) замість трьох.
- `AppDbContext` розміщений у `Factorio.Infrastructure`, а не в `ServerApp/Models`.
- Клас для таблиці Actions названо `UserAction` (щоб не конфліктувати з `System.Action`).
- API-контролери названо `ItemsApiController`, `ActionsApiController`, `AuthApiController`, щоб не збігатися з іменами MVC-контролерів; адреси `/api/items`, `/api/actions`, `/api/auth/login` як у завданні.
- `POST /api/actions` повертає 202 Accepted: запит зберігається й чекає на розгляд адміністратора.
- `Items.UserId` - автор останньої схваленої правки опису (nullable).
