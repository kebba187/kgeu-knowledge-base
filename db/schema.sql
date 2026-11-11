-- Схема базы знаний КГЭУ: согласование и публикация материалов
-- Курс Б1.В.ДЭ.01.02.02 · ЛР9 · Чам Кебба, ТРИС-2-23
-- Соответствует docs/data-model.md. Имена — в snake_case.

-- Учебное пересоздание схемы. Порядок важен: сначала зависимая таблица.
DROP TABLE IF EXISTS publication_requests CASCADE;
DROP TABLE IF EXISTS users CASCADE;
DROP TABLE IF EXISTS departments CASCADE;

-- Справочник подразделений: название хранится здесь один раз.
CREATE TABLE departments (
    id   integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name varchar(150) NOT NULL UNIQUE
);

-- Пользователи: автор, координатор и редактор базы знаний.
CREATE TABLE users (
    id            integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    login         varchar(50) NOT NULL UNIQUE,
    full_name     varchar(150) NOT NULL,
    role          varchar(20) NOT NULL
        CHECK (role IN ('Author', 'Coordinator', 'Editor')),
    password_hash varchar(255) NOT NULL
);

-- Главный объект процесса: заявка на публикацию материала.
CREATE TABLE publication_requests (
    id                 integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    number             varchar(20) NOT NULL UNIQUE,
    title              varchar(80) NOT NULL
        CHECK (char_length(title) BETWEEN 5 AND 80),
    description        varchar(500) NOT NULL
        CHECK (char_length(description) BETWEEN 10 AND 500),
    department_id      integer NOT NULL REFERENCES departments (id),
    status             varchar(20) NOT NULL DEFAULT 'New'
        CHECK (status IN ('New', 'InProgress', 'Closed', 'Cancelled')),
    created_by_user_id integer NOT NULL REFERENCES users (id),
    -- У новой заявки редактора ещё нет, поэтому NULL разрешён.
    assignee_user_id   integer NULL REFERENCES users (id),
    created_at         timestamptz NOT NULL DEFAULT now(),
    updated_at         timestamptz NOT NULL DEFAULT now()
);

-- Ускоряет выборку «заявки в статусе New», которой открывается список.
CREATE INDEX publication_requests_status_idx
    ON publication_requests (status);

-- Ускоряет фильтр по подразделению и JOIN со справочником.
CREATE INDEX publication_requests_department_id_idx
    ON publication_requests (department_id);

-- Ускоряет экран редактора «мои заявки».
CREATE INDEX publication_requests_assignee_user_id_idx
    ON publication_requests (assignee_user_id);

COMMENT ON TABLE publication_requests IS 'Заявки на публикацию материалов базы знаний';
COMMENT ON COLUMN publication_requests.number IS 'Человеческий номер вида БЗ-2026-0001';
COMMENT ON COLUMN publication_requests.status IS 'New, InProgress, Closed, Cancelled';
