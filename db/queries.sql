-- Обязательные запросы ЛР9 · база знаний КГЭУ
-- Запускать после schema.sql и seed.sql.

-- 1. Фильтр и сортировка: новые заявки сверху.
-- Использует индекс publication_requests_status_idx.
SELECT number, title, status, created_at
FROM publication_requests
WHERE status = 'New'
ORDER BY created_at DESC;

-- 2. JOIN: вместо department_id показываем название подразделения.
SELECT r.number,
       r.title,
       d.name AS department_name,
       r.status
FROM publication_requests AS r
JOIN departments AS d ON d.id = r.department_id
ORDER BY r.created_at DESC;

-- 3. GROUP BY: сколько заявок в каждом статусе.
SELECT status, COUNT(*) AS count
FROM publication_requests
GROUP BY status
ORDER BY status;

-- 4. JOIN с пользователями: кто автор и кто назначен редактором.
-- LEFT JOIN, потому что у новой заявки редактора ещё нет.
SELECT r.number,
       author.full_name AS author_name,
       editor.full_name AS editor_name,
       r.status
FROM publication_requests AS r
JOIN users AS author ON author.id = r.created_by_user_id
LEFT JOIN users AS editor ON editor.id = r.assignee_user_id
ORDER BY r.number;

-- 5. GROUP BY по подразделениям: нагрузка на каждое.
SELECT d.name AS department_name,
       COUNT(r.id) AS requests,
       COUNT(*) FILTER (WHERE r.status = 'New') AS waiting
FROM departments AS d
LEFT JOIN publication_requests AS r ON r.department_id = d.id
GROUP BY d.name
ORDER BY requests DESC, d.name;

-- 6. Проверка ограничения CHECK: вставка со статусом Done должна быть отклонена.
-- Ожидаемый результат: ERROR ... violates check constraint.
-- Запрос намеренно оставлен в конце файла и не входит в seed.
-- INSERT INTO publication_requests
--     (number, title, description, department_id, status, created_by_user_id)
-- VALUES ('БЗ-2026-9999', 'Проверка ограничения', 'Статус Done не разрешён схемой курса.', 1, 'Done', 1);
