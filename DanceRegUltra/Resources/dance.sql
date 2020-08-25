/*������� �������*/
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

/*������� �����, ��������� � ������������ ��������*/
create table "event_nodes"
(
"Id_event" INTEGER NOT NULL,
"Id_node" INTEGER NOT NULL,
"Id_member" INTEGER NOT NULL,
"Is_group" BOOLEAN NOT NULL DEFAULT 0,
"Index_platfrorm" INTEGER NOT NULL,
"Id_league" INTEGER NOT NULL,
"Index_block" INTEGER NOT NULL,
"Id_age" INTEGER NOT NULL,
"Id_style" INTEGER NOT NULL,
"Json_scores" TEXT DEFAULT ''
);

/*������� ����������� �����*/
create table "event_nodes_replace"
(
"Id_replace" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Id_event" INTEGER NOT NULL,
"Index_from" INTEGER NOT NULL,
"Index_where"  INTEGER NOT NULL
);

/*������� ���� ��� �������� �������� �������*/
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

insert into "leagues" ("Name") values ("���� 1");
insert into "leagues" ("Name") values ("���� 2");
insert into "leagues" ("Name") values ("���� 3");

create table "ages"
(
"Id_age" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Name" TEXT DEFAULT '',
"Position" INTEGER DEFAULT 0,
"IsHide" BOOLEAN DEFAULT 0
);

insert into "ages" ("Name") values ("������� 1");
insert into "ages" ("Name") values ("������� 2");
insert into "ages" ("Name") values ("������� 3");

create table "styles"
(
"Id_style" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Name" TEXT DEFAULT '',
"Position" INTEGER DEFAULT 0,
"IsHide" BOOLEAN DEFAULT 0
);

insert into "styles" ("Name") values ("����� 1");
insert into "styles" ("Name") values ("����� 2");
insert into "styles" ("Name") values ("����� 3");