CREATE TABLE stars (
    star_id serial primary key,
    constellation int references constellations(con_id),
    proper_name varchar(64),
    right_ascension numeric not null,
    declination numeric not null,
    pos_src varchar(64) not null,
    distance numeric,
    x0 numeric,
    y0 numeric,
    z0 numeric,
    distance_src varchar(64),
    magnitude numeric not null,
    absolute_magnitude numeric,
    colour_index numeric,
    magnitude_src varchar(64) not null,
    radial_velocity numeric,
    radial_velocity_src varchar(64),
    proper_motion_right_ascension numeric,
    proper_motion_declination numeric,
    proper_motion_src varchar(64),
    velocity_x numeric,
    velocity_y numeric,
    velocity_z numeric,
    spectral_type varchar(64),
    spectral_type_src varchar(64),
    created_at timestamp not null default now(),
    updated_at timestamp not null default now()
);

CREATE TABLE catalogue_entries (
    cat_id int not null references catalogues(cat_id) on update cascade,
    star_id int not null references stars(star_id) on update cascade on delete cascade,
    entry_id varchar(32),
    entry_designation varchar(32),
    primary key (cat_id, star_id),
    check (entry_id IS NOT NULL OR entry_designation IS NOT NULL),
    unique (cat_id, entry_id)
);
