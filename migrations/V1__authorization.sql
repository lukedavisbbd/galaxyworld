create table users (
    user_id serial primary key,
    google_id varchar(64) not null unique,
    created_at timestamp not null default now(),
    last_logged_in timestamp not null default now()
);

create table roles (
    role_id serial primary key,
    role_name varchar(16) not null unique
);

create table user_roles (
    user_id int not null references users(user_id),
    role_id int not null references roles(role_id),
    primary key (user_id, role_id)
);

insert into roles (role_name) values ('admin');
