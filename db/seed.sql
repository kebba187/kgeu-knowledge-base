-- Учебные данные базы знаний КГЭУ · ЛР9
-- Пароли здесь не хранятся: в password_hash лежит заглушка,
-- настоящие хэши создаёт ASP.NET Core Identity в ЛР11.

TRUNCATE publication_requests, users, departments RESTART IDENTITY CASCADE;

INSERT INTO departments (name) VALUES
    ('Учебный отдел'),
    ('Деканат'),
    ('Студенческий городок'),
    ('ИТ-служба');

INSERT INTO users (login, full_name, role, password_hash) VALUES
    ('author',      'Сотрудник учебного отдела', 'Author',      'заглушка-до-ЛР11'),
    ('coordinator', 'Координатор базы знаний',   'Coordinator', 'заглушка-до-ЛР11'),
    ('editor',      'Редактор базы знаний',      'Editor',      'заглушка-до-ЛР11');

-- Четыре статуса представлены, чтобы GROUP BY показал все группы.
INSERT INTO publication_requests
    (number, title, description, department_id, status, created_by_user_id, assignee_user_id, created_at)
VALUES
    ('БЗ-2026-0001', 'Порядок оформления академического отпуска',
     'Какие документы нужны, куда подавать и в какой срок рассматривают.',
     1, 'New', 1, NULL, now() - interval '5 days'),

    ('БЗ-2026-0002', 'Как получить доступ к ЭИОС',
     'Где взять логин, что делать при утере пароля, к кому обращаться.',
     4, 'InProgress', 1, 3, now() - interval '4 days'),

    ('БЗ-2026-0003', 'Заселение в общежитие',
     'Список документов и порядок заселения первокурсников.',
     3, 'Closed', 1, 3, now() - interval '3 days'),

    ('БЗ-2026-0004', 'Устаревший порядок пересдачи',
     'Материал отклонён: положение изменилось, нужна новая редакция.',
     2, 'Cancelled', 1, 3, now() - interval '2 days'),

    ('БЗ-2026-0005', 'Как получить справку об обучении',
     'Куда подавать заявление и сколько дней готовится справка.',
     2, 'New', 1, NULL, now() - interval '1 day');
