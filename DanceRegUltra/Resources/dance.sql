/*Таблица событий*/
create table "events"
(
"Id_event" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Title" TEXT DEFAULT '',
"Start_timestamp" LONG DEFAULT -1,
"End_timestamp" LONG DEFAULT -1,
"Json_scheme" TEXT DEFAULT '',
"Id_node_increment" INTERER NOT NULL DEFAULT 1,
"Judge_count" INTEGER DEFAULT 4
);

/*Таблица узлов, связанных с определенным событием*/
create table "event_nodes"
(
"Id_event" INTEGER NOT NULL,
"Id_node" INTEGER NOT NULL,
"Id_member" INTEGER NOT NULL,
"Is_group" BOOLEAN NOT NULL DEFAULT 0,
"Id_platform" INTEGER NOT NULL,
"Id_league" INTEGER NOT NULL,
"Id_block" INTEGER NOT NULL,
"Id_age" INTEGER NOT NULL,
"Id_style" INTEGER NOT NULL,
"Json_scores" TEXT DEFAULT '',
"Position" INTEGER DEFAULT 0
);

create table "schools"
(
"Id_school" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Name" TEXT NOT NULL
);

create table "dancers"
(
"Id_member" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Firstname" TEXT,
"Surname" TEXT,
"Id_school" INTEGER NOT NULL
);

create table "groups"
(
"Id_member" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Json_members" TEXT NOT NULL,
"Id_school" INTEGER NOT NULL
);

/*Шаблоны схем для быстрого создания события*/
create table "template_schemes"
(
"Id_scheme" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Title" TEXT NOT NULL,
"Json_scheme" TEXT NOT NULL
);

create table "leagues"
(
"Id_league" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Name" TEXT DEFAULT '',
"Position" INTEGER DEFAULT 0,
"IsHide" BOOLEAN DEFAULT 0
);

insert into "leagues" ("Name") values ("Дебют");
insert into "leagues" ("Name") values ("Начинающие");
insert into "leagues" ("Name") values ("Любители");
insert into "leagues" ("Name") values ("Открытая лига");
insert into "leagues" ("Name") values ("Высшая лига");
insert into "leagues" ("Name") values ("Профессионалы");
insert into "leagues" ("Name") values ("PRO-AM");
insert into "leagues" ("Name") values ("Продолжающие");
insert into "leagues" ("Name") values ("Супер лига");

create table "ages"
(
"Id_age" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Name" TEXT DEFAULT '',
"Position" INTEGER DEFAULT 0,
"IsHide" BOOLEAN DEFAULT 0
);

insert into "ages" ("Name") values ("Супер-бэби");
insert into "ages" ("Name") values ("Бэби");
insert into "ages" ("Name") values ("Дети");
insert into "ages" ("Name") values ("Бэби 1");
insert into "ages" ("Name") values ("Бэби 2");
insert into "ages" ("Name") values ("Дети+Бэби");
insert into "ages" ("Name") values ("Ювеналы");
insert into "ages" ("Name") values ("Юниоры");
insert into "ages" ("Name") values ("Дочки-Матери");
insert into "ages" ("Name") values ("Ювеналы 1");
insert into "ages" ("Name") values ("Ювеналы 2");
insert into "ages" ("Name") values ("Молодежь+Взрослые");
insert into "ages" ("Name") values ("Юниоры 1");
insert into "ages" ("Name") values ("Юниоры 2");
insert into "ages" ("Name") values ("Ювеналы+Юниоры");
insert into "ages" ("Name") values ("Молодежь");
insert into "ages" ("Name") values ("Взрослые");
insert into "ages" ("Name") values ("Сеньориты");
insert into "ages" ("Name") values ("Гранд-сеньоры");
insert into "ages" ("Name") values ("Смешанная возрастная категория");
insert into "ages" ("Name") values ("Юниоры+Молодежь");
insert into "ages" ("Name") values ("Дети+Ювеналы");

create table "styles"
(
"Id_style" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Name" TEXT DEFAULT '',
"Position" INTEGER DEFAULT 0,
"IsHide" BOOLEAN DEFAULT 0
);

insert into "styles" ("Name") values ("Импровизация Raqs al Sharqi");
insert into "styles" ("Name") values ("Египетский Фольклор");
insert into "styles" ("Name") values ("Не Египетский Фольклор");
insert into "styles" ("Name") values ("Импровизация Табла");
insert into "styles" ("Name") values ("Импровизация Кубок");
insert into "styles" ("Name") values ("Табла");
insert into "styles" ("Name") values ("Эстрадная Песня");
insert into "styles" ("Name") values ("Фьюжн");
insert into "styles" ("Name") values ("СТК");
insert into "styles" ("Name") values ("Индийский танец");
insert into "styles" ("Name") values ("Цыганский танец");
insert into "styles" ("Name") values ("Шоу");
insert into "styles" ("Name") values ("Cценический танец");
insert into "styles" ("Name") values ("Street Shaabi");
insert into "styles" ("Name") values ("Импровизация Балади");