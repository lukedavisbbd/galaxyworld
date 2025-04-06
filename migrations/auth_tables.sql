create table users (
    user_id serial primary key,
    google_id varchar(64) unique,
    created_at timestamp default now(),
    last_logged_in timestamp default now()
);

create table roles (
    role_id serial primary key,
    role_name varchar(16) unique
);

create table user_roles (
    user_id int references users(user_id),
    role_id int references roles(role_id),
    primary key (user_id, role_id)
);

insert into roles (role_name) values ('admin');
