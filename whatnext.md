# Что в этом архиве и что делать после распаковки

Исправления по ревью: #13 (навигационные свойства), #7 (проекция в SQL),
#14 (названия в ответе API) и требования ФТ-4, ФТ-5, ФТ-6, ФТ-8,
которые были описаны в ТЗ, но не реализованы.

Архив **только добавляет и заменяет** файлы. Ничего не удаляется —
папка `backend/Migrations` не трогается.

## Файлы

```
backend/Models/PublicationRequest.cs              навигационные свойства
backend/Data/AppDbContext.cs                      связи через навигации, 3 CHECK, DEFAULT статуса
backend/Dtos/PublicationRequestResponse.cs        departmentName, createdByFullName, assigneeFullName
backend/Services/IPublicationRequestStore.cs      метод GetList с отбором
backend/Services/EfPublicationRequestStore.cs     WHERE + JOIN + SELECT одним запросом
backend/Services/InMemoryPublicationRequestStore.cs  тот же отбор в памяти
backend/Services/PublicationRequestService.cs     GetListFor: область видимости по роли
backend/Controllers/PublicationRequestsController.cs  GET с параметрами status и departmentId
frontend/src/api/departments.js                   новый файл: GET /api/departments
frontend/src/dictionaries.js                      остались только подписи статусов
frontend/src/components/TicketTable.jsx           название подразделения и колонка «Редактор»
frontend/src/pages/TicketNew.jsx                  select из справочника вместо списка в коде
```

## После распаковки

```powershell
cd backend
dotnet build
dotnet ef migrations add AddNavigationsAndChecks
dotnet ef database update

cd ..\frontend
npx oxlint
npm run dev
```

Миграция добавляет три ограничения `CHECK` к таблице, в которой уже есть
строки. Если какая-то заявка короче 5 символов в заголовке или 10 в описании,
ограничение не применится — проще пересоздать базу, как в прошлый раз:

```powershell
dropdb -U postgres kb_kgeu
createdb -U postgres kb_kgeu
cd backend
dotnet ef database update
dotnet run
```

## Что проверить руками

1. Войти как `author` — в списке только свои заявки (ФТ-4).
2. Войти как `editor` — только назначенные ему (ФТ-8).
3. Войти как `coordinator` — все заявки; `?status=New` и `?departmentId=2` сужают список (ФТ-5).
4. В таблице видно название подразделения и ФИО редактора, а не голые числа (ФТ-6, D-02).
5. Форма создания: подразделение выбирается из выпадающего списка (ФТ-1).
6. `GET /api/publication-requests?status=Banana` → 400 с полем `status`.

## Ещё не сделано

- Документы: `api-contract.md` (три новых поля и два параметра запроса),
  `role-matrix.md` (список теперь зависит от роли), `requirements-matrix.md`
  (ФТ-4/5/6/8), `known-defects.md` (закрыть D-02).
- Комментарии в issue #7, #13, #14.
- Issue #6 — создание заявки двумя запросами вместо одного.
- Issue #16 — прогон стенда и заполнение «Факт».

## Важно

Код не компилировался: в моей среде нет .NET SDK и закрыт доступ к NuGet.
`dotnet build` — первая настоящая проверка.
