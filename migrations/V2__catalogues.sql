CREATE TABLE catalogues (
    catalogue_id serial primary key,
    catalogue_name varchar(32) unique check (TRIM(catalogue_name) = catalogue_name AND LENGTH(catalogue_name) > 0),
    catalogue_slug varchar(32) unique check (regexp_like(catalogue_slug, '^[a-z0-9_]+$'))
);

INSERT INTO catalogues (catalogue_name, catalogue_slug) VALUES
    ('ATHYG', 'athyg'),
    ('Tycho-2', 'tycho2'),
    ('Gaia DR3', 'gaia3'),
    ('HYG', 'hyg'),
    ('Hipparcos Main', 'hip'),
    ('Henry Draper', 'hd'),
    ('Yale Bright Star', 'ybs'),
    ('Gliese', 'gliese'),
    ('Bayer', 'bayer'),
    ('Flamsteed', 'flam');
