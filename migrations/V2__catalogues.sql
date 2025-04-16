CREATE TABLE catalogues (
    cat_id serial primary key,
    cat_name varchar(32) unique check (TRIM(cat_name) = cat_name AND LENGTH(cat_name) > 0),
    cat_slug varchar(32) unique check (regexp_like(cat_slug, '^[a-z0-9_]+$'))
);

INSERT INTO catalogues (cat_name, cat_slug) VALUES
    ('ATHYG', 'athyg'),
    ('Tycho-2', 'tycho2'),
    ('Gaia DR3', 'gaia3'),
    ('HYG', 'hyg'),
    ('Hipparcos Main', 'hipparcos'),
    ('Henry Draper', 'hd'),
    ('Yale Bright Star', 'ybs'),
    ('Gliese', 'gliese'),
    ('Bayer', 'bayer'),
    ('Flamsteed', 'flamsteed');
