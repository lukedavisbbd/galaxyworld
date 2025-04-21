CREATE TABLE stars (
    star_id serial primary key,
    constellation_id int references constellations(constellation_id) on update cascade on delete set null,
    proper_name varchar(64) check (TRIM(proper_name) = proper_name AND LENGTH(proper_name) > 0),
    right_ascension numeric not null,
    declination numeric not null,
    distance numeric,
    position_cartesian numeric[3],
    magnitude numeric not null,
    velocity_circular numeric[3],
    velocity_cartesian numeric[3],
    colour_index numeric,
    spectral_type varchar(64) check (TRIM(spectral_type) = spectral_type AND LENGTH(spectral_type) > 0)
);

CREATE TABLE catalogue_entries (
    catalogue_id int not null references catalogues(catalogue_id) on update cascade,
    star_id int not null references stars(star_id) on update cascade on delete cascade,
    entry_id varchar(32) check (TRIM(entry_id) = entry_id AND LENGTH(entry_id) > 0),
    entry_designation varchar(32) check (TRIM(entry_designation) = entry_designation AND LENGTH(entry_designation) > 0),
    primary key (catalogue_id, star_id),
    check (entry_id IS NOT NULL OR entry_designation IS NOT NULL),
    unique (catalogue_id, entry_id)
);
