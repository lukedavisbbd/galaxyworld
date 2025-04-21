create table users (
    user_id serial primary key,
    google_id varchar(64) not null unique,
    check (regexp_like(google_id, '^[0-9]+$'))
);

create table roles (
    role_id serial primary key,
    role_name varchar(16) not null unique check (TRIM(role_name) = role_name AND LENGTH(role_name) > 0)
);

create table user_roles (
    user_id int not null references users(user_id) on delete cascade on update cascade,
    role_id int not null references roles(role_id) on delete cascade on update cascade,
    primary key (user_id, role_id)
);

insert into roles (role_name) values ('admin');
