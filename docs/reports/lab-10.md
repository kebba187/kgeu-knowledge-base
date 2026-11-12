# Отчёт · ЛР10. EF Core: сервер пишет в базу

| Поле | Значение |
|------|----------|
| Дисциплина | Б1.В.ДЭ.01.02.02 · РИС и Web |
| Студент | Чам Кебба |
| Группа | ТРИС-2-23 |
| № ЛР | 10 |
| Тема работы | EF Core и PostgreSQL вместо списка в памяти |
| Тема проекта | «База знаний КГЭУ»: согласование и публикация материалов |

---

## 1. Цель

Сервер перестаёт хранить заявки в памяти и пишет их в PostgreSQL через EF Core. Создали запись → перезапустили API → запись на месте. Строка подключения в репозиторий не попадает.

## 2. Что сделали

1. Подключены пакеты `Npgsql.EntityFrameworkCore.PostgreSQL` и `Microsoft.EntityFrameworkCore.Design`.
2. `backend/Data/AppDbContext.cs` — наборы `Departments`, `Users`, `PublicationRequests`; имена таблиц и колонок заданы явно в `snake_case`, чтобы совпадать с `db/schema.sql`.
3. `backend/Models/Department.cs` и `backend/Models/User.cs` — сущности справочника и пользователей.
4. `backend/Services/EfPublicationRequestStore.cs` — та же реализация интерфейса `IPublicationRequestStore`, что и список в памяти, но через EF Core. Контроллер и сервис не менялись.
5. `backend/Data/PostgresStorage.cs` — регистрация контекста и хранилищ одной строкой в `Program.cs`.
6. Строка подключения читается по ключу `ConnectionStrings:Default`; в `appsettings.json` значение пустое.

## 3. Соответствие модели и схемы ЛР9

| C# | PostgreSQL |
|---|---|
| `Id` | `id` |
| `Number` | `number` |
| `DepartmentId` | `department_id` |
| `CreatedByUserId` | `created_by_user_id` |
| `AssigneeUserId` | `assignee_user_id` (NULL разрешён) |
| `CreatedAt` / `UpdatedAt` | `created_at` / `updated_at` |

Ограничение статусов задано и в схеме, и в модели: `status IN ('New','InProgress','Closed','Cancelled')`.

## 4. Запуск

```bash
cd backend
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=kb_kgeu;Username=postgres;Password=ВАШ_ПАРОЛЬ"

dotnet tool install --global dotnet-ef   # если ещё не установлен
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Миграция создаётся на рабочем месте и коммитится вместе с `backend/Migrations/`. Если база уже содержит таблицы из `db/schema.sql`, перед `database update` их нужно удалить (или создать отдельную базу), иначе миграция попытается создать существующие таблицы.

## 5. Доказательство успеха — рестарт-тест

| Шаг | Действие | Факт |
|---|---|---|
| 1 | `POST /api/publication-requests` | `201`, id = ____, number = ____ |
| 2 | запись в базе | `SELECT id, number, title, status FROM publication_requests WHERE id = __;` → строка есть |
| 3 | `Ctrl+C`, затем `dotnet run` | сервер снова слушает `:5000` |
| 4 | `GET /api/publication-requests/{id}` | `200`, те же `number`, `title`, `status` |

Заполнить одним и тем же id до и после рестарта; вставить скрин SQL-результата.

## 6. Доказательство отказа

| Что сделали | Ожидание | Факт |
|---|---|---|
| `dotnet run` без заданной строки подключения | понятная ошибка, а не падение в середине запроса | `InvalidOperationException: Не задана строка подключения ConnectionStrings:Default…` |
| `POST` с `departmentId`, которого нет в справочнике | `400`, а не `500` от внешнего ключа | `400 {"fields":{"departmentId":"Такого подразделения нет в справочнике"}}` |

## 7. Секреты

| Проверка | Факт |
|---|---|
| `appsettings.json` | `"Default": ""` — пароля нет |
| `git ls-files` | файлов user-secrets в репозитории нет |
| README | описан порядок задания секрета, сам пароль не указан |

## 8. Что осталось на следующую работу

1. Вход и роли: гость — 401, чужая роль — 403 (ЛР11).
2. Пароли пользователей хэшируются библиотекой, заглушки из `db/seed.sql` заменяются.
3. Сквозная сборка клиента и сервера (ЛР12).
