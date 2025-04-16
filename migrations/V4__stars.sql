CREATE TABLE stars (
    star_id serial primary key,
    constellation int references constellations(con_id),
    proper_name varchar(64) check (TRIM(proper_name) = proper_name AND LENGTH(proper_name) > 0),
    right_ascension numeric not null,
    declination numeric not null,
    pos_src varchar(64) not null check (TRIM(pos_src) = pos_src AND LENGTH(pos_src) > 0),
    distance numeric,
    x0 numeric,
    y0 numeric,
    z0 numeric,
    distance_src varchar(64) check (TRIM(distance_src) = distance_src AND LENGTH(distance_src) > 0),
    magnitude numeric not null,
    absolute_magnitude numeric,
    colour_index numeric,
    magnitude_src varchar(64) not null check (TRIM(magnitude_src) = magnitude_src AND LENGTH(magnitude_src) > 0),
    radial_velocity numeric,
    radial_velocity_src varchar(64) check (TRIM(radial_velocity_src) = radial_velocity_src AND LENGTH(radial_velocity_src) > 0),
    proper_motion_right_ascension numeric,
    proper_motion_declination numeric,
    proper_motion_src varchar(64) check (TRIM(proper_motion_src) = proper_motion_src AND LENGTH(proper_motion_src) > 0),
    velocity_x numeric,
    velocity_y numeric,
    velocity_z numeric,
    spectral_type varchar(64) check (TRIM(spectral_type) = spectral_type AND LENGTH(spectral_type) > 0),
    spectral_type_src varchar(64) check (TRIM(spectral_type_src) = spectral_type_src AND LENGTH(spectral_type_src) > 0),
    created_at timestamp not null default now(),
    updated_at timestamp not null default now()
);

CREATE TABLE catalogue_entries (
    cat_id int not null references catalogues(cat_id) on update cascade,
    star_id int not null references stars(star_id) on update cascade on delete cascade,
    entry_id varchar(32) check (TRIM(entry_id) = entry_id AND LENGTH(entry_id) > 0),
    entry_designation varchar(32) check (TRIM(entry_designation) = entry_designation AND LENGTH(entry_designation) > 0),
    primary key (cat_id, star_id),
    check (entry_id IS NOT NULL OR entry_designation IS NOT NULL),
    unique (cat_id, entry_id)
);
