CREATE TABLE stars (
    star_id serial primary key,
    constellation int references constellations(con_id),
    proper_name varchar(64) unique,
    right_ascension numeric not null,
    declination numeric not null,
    pos_src varchar(16) not null,
    distance numeric,
    x0 numeric,
    y0 numeric,
    z0 numeric,
    distance_src varchar(16),
    magnitude numeric not null,
    absolute_magnitude numeric,
    colour_index numeric,
    magnitude_src varchar(16) not null,
    radial_velocity numeric,
    radial_velocity_src varchar(16),
    proper_motion_right_ascension numeric,
    proper_motion_declination numeric,
    proper_motion_src varchar(16),
    velocity_x numeric,
    velocity_y numeric,
    velocity_z numeric,
    spectral_type varchar(16),
    spectral_type_src varchar(16),
    created_at timestamp not null default now(),
    updated_at timestamp not null default now()
);

CREATE TABLE catalogue_star_entries (
    cat_id int not null references catalogues(cat_id) on update cascade,
    star_id int not null references stars(star_id) on update cascade on delete cascade,
    entry_id varchar(32) not null,
    primary key (cat_id, star_id),
    unique (cat_id, entry_id)
);
