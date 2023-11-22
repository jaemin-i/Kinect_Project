create table login
(
    id         varchar(20)  not null
        primary key,
    password   varchar(200) not null,
    last_login date         null,
    constraint login_id_uindex
        unique (id),
    constraint login_id_fk
        foreign key (id) references member (id)
            on update cascade on delete cascade
);

create table member
(
    id        varchar(20) not null
        primary key,
    name      varchar(20) not null,
    phone_num varchar(20) not null,
    email     varchar(30) not null,
    constraint member_id_uindex
        unique (id)
);

insert into project.login (id, password, last_login)
values  ('a44006', '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', '2022-11-27'),
        ('a44018', '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', '2022-11-27'),
        ('a44030', '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', '2022-11-27'),
        ('a44034', '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', '2022-11-27'),
        ('admin', '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', '2022-11-27');

insert into project.member (id, name, phone_num, email)
values  ('a44006', '김민규', '010-5414-7618', '201844006@itc.ac.kr'),
        ('a44018', '안재민', '010-6650-9876', '201844018@itc.ac.kr'),
        ('a44030', '반진성', '010', '201944030@itc.ac.kr'),
        ('a44034', '정성민', '010', '201944034@itc.ac.kr'),
        ('admin', '관리자', '010', 'admin@itc.ac.kr');