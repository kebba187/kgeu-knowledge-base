# База знаний КГЭУ: согласование и публикация материалов

Курс **РИС и Web** (Б1.В.ДЭ.01.02.02) · КГЭУ · семестр 1 · 2026/2027

| | |
|---|---|
| Студент | Чам Кебба |
| Группа | ТРИС-2-23 |
| Тема | «База знаний КГЭУ»: модуль согласования и публикации материалов (семестровая часть темы ВКР №6) |
| Стек | React + Vite · ASP.NET Core 8 · PostgreSQL (клиент на React — по согласованию с преподавателем 06.10.2026) |

Сотрудник подразделения подаёт заявку на публикацию материала → координатор назначает редактора → редактор проверяет и публикует материал или отклоняет заявку.

Статусы: `New` (Поступила) → `InProgress` (На проверке) → `Closed` (Опубликовано) / `Cancelled` (Отклонено).

---

## Сервер (backend)

Нужен .NET SDK 8.0 или новее (`dotnet --version`).

```bash
cd backend
dotnet restore
dotnet run
```

| Адрес | Что это |
|---|---|
| `http://localhost:5000` | базовый адрес API |
| `http://localhost:5000/swagger` | Swagger, доступен в режиме Development |
| `http://localhost:5000/health` | проверка живости процесса |
| `http://localhost:5000/info` | имя приложения, среда, режим хранения |

Важно:

- заявки хранятся в PostgreSQL: после перезапуска сервера записи на месте;
- `GET /health` отвечает `200`, когда процесс запущен и маршрут отвечает. Это **не** доказывает, что создание заявки работает: `/health` не принимает `title`, `departmentId` и `description`;
- маршруты заявок описаны в Swagger и в `docs/api-contract.md`.

## База данных

```bash
createdb kb_kgeu
```

Строка подключения хранится локально, в репозитории её нет:

```bash
cd backend
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=kb_kgeu;Username=postgres;Password=ВАШ_ПАРОЛЬ"
```

Таблицы создаёт миграция EF Core:

```bash
cd backend
dotnet ef database update      # dotnet tool install --global dotnet-ef, если команды нет
```

Учебные данные:

```bash
psql -d kb_kgeu -f db/seed.sql
```

Файлы `db/schema.sql`, `db/seed.sql`, `db/queries.sql` — результат ЛР9: та же схема на чистом SQL.

## Клиент (frontend)

Нужен Node.js 22.18+ (LTS) или 24.12+.

```bash
cp frontend/.env.example frontend/.env   # Windows: copy frontend\.env.example frontend\.env
cd frontend
npm install
npm run dev
```

Открыть `http://localhost:5173`. Адреса: `/` — список, `/new` — создание, `/login` — вход. Пока данные берутся из учебного mock (`VITE_USE_MOCK=true`), сервер подключается позже.

## Учебные учётные записи

Только для локального стенда курса:

| Логин | Пароль | Роль | Что может |
|---|---|---|---|
| `author` | `Author-2026` | автор | создаёт заявки |
| `coordinator` | `Coordinator-2026` | координатор | назначает редактора |
| `editor` | `Editor-2026` | редактор | меняет статус своих заявок |

Пользователей с хэшами паролей создаёт сервер при первом запуске в режиме Development. В базе лежит хэш `PasswordHasher<User>`, а не пароль. Права по ролям — [`docs/role-matrix.md`](docs/role-matrix.md).

## Документы

**ЛР1–2. Анализ, требования, архитектура**

- [Заявка на тему](docs/project-proposal.md) · [Требования](docs/lab-01-requirements.md)
- [Архитектура](docs/architecture.md) · [Модель данных](docs/data-model.md) · [Контракт API](docs/api-contract.md) · [Матрица требований](docs/requirements-matrix.md)

**ЛР3–7. Клиент и каркас сервера**

- [Отчёты по лабораторным](docs/reports/)
