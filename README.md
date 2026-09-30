# FactorioGuide

Система для управління Users, Items, Actions з MVC-адмін-панеллю та MAUI-застосунком.

## Структура репозиторію

- `Server/ServerApp` — ASP.NET Core (MVC-адмін-панель + REST API)
- `Client/ClientApp` — .NET MAUI застосунок (MVVM)
- `Database` — SQL-скрипти (з'явиться в Завданні 2)
- `Docs` — документація (з'явиться пізніше)

## Вимоги

- Visual Studio з робочими навантаженнями ASP.NET and web development та .NET MAUI
- .NET SDK (версія, як у проєктах)
- Android-емулятор (наприклад, API 33)

## Запуск

1. Клонувати репозиторій.
2. Відкрити `Server/ServerApp` у Visual Studio і запустити (F5).
3. Відкрити `Client/ClientApp` і запустити на Android-емуляторі.

## Примітка

`appsettings.json` не зберігається в репозиторії; його потрібно створити локально.