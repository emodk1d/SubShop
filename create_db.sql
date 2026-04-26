CREATE TABLE  table_products
(
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    name            TEXT NOT NULL,
    unit_of_measure TEXT NOT NULL,
    price           REAL NOT NULL CHECK (price >= 0),
    quantity        INT NOT NULL DEFAULT 0 CHECK (quantity >= 0),
    created_at      TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    updated_at      TEXT NOT NULL DEFAULT (datetime('now', 'localtime'))
);

CREATE TABLE  table_sales
(
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    product_id   INTEGER NOT NULL,
    quantity     INT    NOT NULL,
    price        REAL    NOT NULL,
    total_amount INT   NOT NULL,
    sale_date    TEXT    NOT NULL DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (product_id) REFERENCES table_products (id)
);

CREATE TRIGGER  trigger_update_products
    AFTER UPDATE
    ON table_products
BEGIN
    UPDATE table_products
    SET updated_at = datetime('now', 'localtime')
    WHERE id = NEW.id;
END;

INSERT INTO table_products (name, unit_of_measure, price, quantity)
VALUES ('Ноутбук ASUS', 'шт', 75000.00, 10),
       ('Мышь Logitech', 'шт', 1500.00, 50),
       ('Клавиатура механическая', 'шт', 3500.00, 25),
       ('Монитор 27"', 'шт', 25000.00, 15),
       ('Кабель USB Type-C', 'шт', 500.00, 100),
       ('Наушники Bluetooth', 'шт', 4000.00, 30),
       ('Веб-камера HD', 'шт', 3000.00, 20),
       ('Коврик для мыши', 'шт', 800.00, 40);
