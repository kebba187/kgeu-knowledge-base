# База знаний КГЭУ: согласование и публикация материалов

Курс **РИС и Web** (Б1.В.ДЭ.01.02.02) · КГЭУ · семестр 1 · 2026/2027

| | |
|---|---|
| Студент | Чам Кебба |
| Группа | ТРИС-2-23 |
| Тема | «База знаний КГЭУ»: модуль согласования и публикации материалов (семестровая часть темы ВКР №6) |
| Стек | React + Vite · ASP.NET Core 8 · PostgreSQL 16 (клиент на React — по согласованию с преподавателем 06.10.2026) |

Сотрудник подразделения подаёт заявку на публикацию материала → координатор назначает редактора → редактор проверяет и публикует материал или отклоняет заявку.

Статусы: `New` (Поступила) → `InProgress` (На проверке) → `Closed` (Опубликовано) / `Cancelled` (Отклонено).

---

## Что нужно на рабочем месте

| Инструмент | Версия |
|---|---|
| .NET SDK | 8.0 или новее |
| Node.js | 22.18+ (LTS) или 24.12+ |
| PostgreSQL | 14 или новее |

Проверка: `dotnet --version`, `node -v`, `psql --version`.

## 1. База данных

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
dotnet ef database update
```

Учебные данные (подразделения и примеры заявок):

```bash
psql -d kb_kgeu -f db/seed.sql
```

Файлы `db/schema.sql`, `db/seed.sql`, `db/queries.sql` — результат ЛР9: та же схема, но на чистом SQL.

## 2. Сервер API

```bash
cd backend
dotnet restore
dotnet run
```

| Адрес | Что это |
|---|---|
| `http://localhost:5000` | базовый адрес API |
| `http://localhost:5000/swagger` | Swagger в режиме Development |
| `http://localhost:5000/health` | только проверка живости процесса |
| `http://localhost:5000/info` | имя приложения, среда, режим хранения |

`GET /health` отвечает `200`, даже если заявки не создаются: это проверка процесса, а не функции создания.

## 3. Клиент

```bash
cp frontend/.env.example frontend/.env   # Windows: copy frontend\.env.example frontend\.env
cd frontend
npm install
npm run dev
```

Открыть `http://localhost:5173`. Адреса: `/` — список, `/new` — создание, `/login` — вход.

`frontend/.env` в Git не хранится. Значения по умолчанию:

```dotenv
VITE_API_BASE_URL=http://localhost:5000
VITE_USE_MOCK=false
```

`VITE_USE_MOCK=true` переключает список и создание на учебный mock — удобно, если сервер ещё не поднят.

## 4. Учебные учётные записи

Только для локального стенда курса:

| Логин | Пароль | Роль | Что может |
|---|---|---|---|
| `author` | `Author-2026` | автор | создаёт заявки |
| `coordinator` | `Coordinator-2026` | координатор | назначает редактора |
| `editor` | `Editor-2026` | редактор | меняет статус своих заявок |
| `editor2` | редактор | `Editor2-2026` | Второй редактор базы знаний |


Пользователей с хэшами паролей создаёт сервер при первом запуске в режиме Development. В базе лежит хэш `PasswordHasher<User>`, а не пароль.

## 5. Проверочный сценарий

1. Войти как `author`, создать заявку на `/new` — в списке появляется номер вида `БЗ-2026-0001`, статус «Поступила».
2. Войти как `coordinator`, выбрать редактора в строке заявки — статус не меняется, редактор записан.
3. Войти как `editor`, нажать «Взять на проверку» — статус «На проверке».
4. Остановить API (`Ctrl+C`), запустить снова, обновить список — заявка на месте.

Подробности и ожидаемые коды: [`docs/acceptance-check.md`](docs/acceptance-check.md).

## 6. Известные ограничения

Список ведётся в [`docs/known-defects.md`](docs/known-defects.md).

## Документы

**ЛР1–2. Анализ, требования, архитектура**

- [Заявка на тему](docs/project-proposal.md) · [Требования](docs/lab-01-requirements.md)
- [Архитектура](docs/architecture.md) · [Модель данных](docs/data-model.md) · [Контракт API](docs/api-contract.md) · [Матрица требований](docs/requirements-matrix.md)

**ЛР3–6. Клиент**

- [Отчёты](docs/reports/) — каркас, маршруты, форма, API-модуль и состояния UI

**ЛР7–12. Сервер, база и сборка**

- [Коллекция запросов](docs/api-collection/) · [Матрица ролей](docs/role-matrix.md)
- [Проверка критериев](docs/acceptance-check.md) · [Известные дефекты](docs/known-defects.md)

**ЛР13. Предзащита**

- [Чеклист предзащиты](docs/predefense-check.md) · [Пояснительная записка](docs/kursovaya-zapiska.md)
- [Отчёты по лабораторным](docs/reports/)
