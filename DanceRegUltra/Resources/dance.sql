/*������� �������*/
create table "events"
(
"Id_event" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Title" TEXT DEFAULT '',
"Start_timestamp" LONG DEFAULT -1,
"End_timestamp" LONG DEFAULT -1,
"Json_scheme" TEXT DEFAULT '',
"Id_node_increment" INTERER NOT NULL DEFAULT 1,
"Num_increment" INTERER NOT NULL DEFAULT 100,
"Enable_rand_nums" BOOLEAN DEFAULT 1,
"All_member_count" INTEGER DEFAULT 0
);

/*������� �����, ��������� � ������������ ��������*/
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
"Prize_place" INTEGER DEFAULT 0,
"Is_print_prize" BOOLEAN DEFAULT 0,
"Position" INTEGER DEFAULT 0
);

create table "nominations"
(
"Id_event" INTEGER NOT NULL,
"Id_block" INTEGER NOT NULL,
"Id_league" INTEGER NOT NULL,
"Id_age" INTEGER NOT NULL,
"Id_style" INTEGER NOT NULL,
"Is_show_in_search" BOOLEAN DEFAULT 1,
"Json_judge_ignore" TEXT DEFAULT '',
"Separate_dancer_group" BOOLEAN NOT NULL DEFAULT 1
);

/*��������� ����*/
create table "schools"
(
"Id_school" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Name" TEXT NOT NULL
);

/*��������� ��������*/
create table "dancers"
(
"Id_member" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Firstname" TEXT,
"Surname" TEXT,
"Id_school" INTEGER NOT NULL
);

/*��������� ����� ����������*/
create table "groups"
(
"Id_member" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Json_members" TEXT NOT NULL,
"Id_school" INTEGER NOT NULL
);

/*������ ���������� �������*/
create table "nums_for_members"
(
"Id_event" INTEGER NOT NULL,
"Id_member" INTEGER NOT NULL,
"Is_group" BOOLEAN NOT NULL,
"Number" INTEGER DEFAULT 0
);

/*������� ���� ��� �������� �������� �������*/
create table "template_schemes"
(
"Id_scheme" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Title" TEXT NOT NULL,
"Json_scheme" TEXT NOT NULL
);

insert into template_schemes (Title, Json_scheme) values ('�� ���������', '{"PlatformIncrement":1,"BlockIncrement":4,"Platforms":[{"IdArray":0,"Title":"��������� 1","Values":[{"Id":1,"IsChecked":true},{"Id":2,"IsChecked":true},{"Id":3,"IsChecked":true},{"Id":4,"IsChecked":true},{"Id":5,"IsChecked":true},{"Id":6,"IsChecked":true},{"Id":7,"IsChecked":true},{"Id":8,"IsChecked":true},{"Id":9,"IsChecked":true}],"ScoreType":0,"JudgeCount":0}],"Blocks":[{"IdArray":0,"Title":"���� 1","Values":[{"Id":1,"IsChecked":true},{"Id":2,"IsChecked":true},{"Id":3,"IsChecked":true},{"Id":4,"IsChecked":true},{"Id":5,"IsChecked":true},{"Id":6,"IsChecked":true},{"Id":7,"IsChecked":true},{"Id":8,"IsChecked":true},{"Id":9,"IsChecked":true},{"Id":10,"IsChecked":true},{"Id":11,"IsChecked":true},{"Id":12,"IsChecked":true},{"Id":13,"IsChecked":true},{"Id":14,"IsChecked":true},{"Id":15,"IsChecked":true},{"Id":16,"IsChecked":true},{"Id":17,"IsChecked":true},{"Id":18,"IsChecked":true},{"Id":19,"IsChecked":true},{"Id":20,"IsChecked":true},{"Id":21,"IsChecked":true},{"Id":22,"IsChecked":true}],"ScoreType":0,"JudgeCount":0},{"IdArray":1,"Title":"���� 2","Values":[{"Id":1,"IsChecked":true},{"Id":2,"IsChecked":true},{"Id":3,"IsChecked":true},{"Id":4,"IsChecked":true},{"Id":5,"IsChecked":true},{"Id":6,"IsChecked":true},{"Id":7,"IsChecked":true},{"Id":8,"IsChecked":true},{"Id":9,"IsChecked":true},{"Id":10,"IsChecked":true},{"Id":11,"IsChecked":true},{"Id":12,"IsChecked":true},{"Id":13,"IsChecked":true},{"Id":14,"IsChecked":true},{"Id":15,"IsChecked":true},{"Id":16,"IsChecked":true},{"Id":17,"IsChecked":true},{"Id":18,"IsChecked":true},{"Id":19,"IsChecked":true},{"Id":20,"IsChecked":true},{"Id":21,"IsChecked":true},{"Id":22,"IsChecked":true}],"ScoreType":0,"JudgeCount":0},{"IdArray":2,"Title":"���� 3","Values":[{"Id":1,"IsChecked":true},{"Id":2,"IsChecked":true},{"Id":3,"IsChecked":true},{"Id":4,"IsChecked":true},{"Id":5,"IsChecked":true},{"Id":6,"IsChecked":true},{"Id":7,"IsChecked":true},{"Id":8,"IsChecked":true},{"Id":9,"IsChecked":true},{"Id":10,"IsChecked":true},{"Id":11,"IsChecked":true},{"Id":12,"IsChecked":true},{"Id":13,"IsChecked":true},{"Id":14,"IsChecked":true},{"Id":15,"IsChecked":true},{"Id":16,"IsChecked":true},{"Id":17,"IsChecked":true},{"Id":18,"IsChecked":true},{"Id":19,"IsChecked":true},{"Id":20,"IsChecked":true},{"Id":21,"IsChecked":true},{"Id":22,"IsChecked":true}],"ScoreType":0,"JudgeCount":0},{"IdArray":3,"Title":"���� 4","Values":[{"Id":1,"IsChecked":true},{"Id":2,"IsChecked":true},{"Id":3,"IsChecked":true},{"Id":4,"IsChecked":true},{"Id":5,"IsChecked":true},{"Id":6,"IsChecked":true},{"Id":7,"IsChecked":true},{"Id":8,"IsChecked":true},{"Id":9,"IsChecked":true},{"Id":10,"IsChecked":true},{"Id":11,"IsChecked":true},{"Id":12,"IsChecked":true},{"Id":13,"IsChecked":true},{"Id":14,"IsChecked":true},{"Id":15,"IsChecked":true},{"Id":16,"IsChecked":true},{"Id":17,"IsChecked":true},{"Id":18,"IsChecked":true},{"Id":19,"IsChecked":true},{"Id":20,"IsChecked":true},{"Id":21,"IsChecked":true},{"Id":22,"IsChecked":true}],"ScoreType":0,"JudgeCount":0}],"Styles":[{"Id":1,"IsChecked":true},{"Id":2,"IsChecked":true},{"Id":3,"IsChecked":true},{"Id":4,"IsChecked":true},{"Id":5,"IsChecked":true},{"Id":6,"IsChecked":true},{"Id":7,"IsChecked":true},{"Id":8,"IsChecked":true},{"Id":9,"IsChecked":true},{"Id":10,"IsChecked":true},{"Id":11,"IsChecked":true},{"Id":12,"IsChecked":true},{"Id":13,"IsChecked":true},{"Id":14,"IsChecked":true},{"Id":15,"IsChecked":true}]}');

/*��������� ���*/
create table "leagues"
(
"Id_league" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Name" TEXT DEFAULT '',
"Position" INTEGER DEFAULT 0,
"IsHide" BOOLEAN DEFAULT 0
);

insert into "leagues" ("Name") values ("�����");
insert into "leagues" ("Name") values ("����������");
insert into "leagues" ("Name") values ("��������");
insert into "leagues" ("Name") values ("�������� ����");
insert into "leagues" ("Name") values ("������ ����");
insert into "leagues" ("Name") values ("�������������");
insert into "leagues" ("Name") values ("PRO-AM");
insert into "leagues" ("Name") values ("������������");
insert into "leagues" ("Name") values ("����� ����");

/*��������� ���������� ���������*/
create table "ages"
(
"Id_age" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Name" TEXT DEFAULT '',
"Position" INTEGER DEFAULT 0,
"IsHide" BOOLEAN DEFAULT 0
);

insert into "ages" ("Name") values ("�����-����");
insert into "ages" ("Name") values ("����");
insert into "ages" ("Name") values ("����");
insert into "ages" ("Name") values ("���� 1");
insert into "ages" ("Name") values ("���� 2");
insert into "ages" ("Name") values ("����+����");
insert into "ages" ("Name") values ("�������");
insert into "ages" ("Name") values ("������");
insert into "ages" ("Name") values ("�����-������");
insert into "ages" ("Name") values ("������� 1");
insert into "ages" ("Name") values ("������� 2");
insert into "ages" ("Name") values ("��������+��������");
insert into "ages" ("Name") values ("������ 1");
insert into "ages" ("Name") values ("������ 2");
insert into "ages" ("Name") values ("�������+������");
insert into "ages" ("Name") values ("��������");
insert into "ages" ("Name") values ("��������");
insert into "ages" ("Name") values ("���������");
insert into "ages" ("Name") values ("�����-�������");
insert into "ages" ("Name") values ("��������� ���������� ���������");
insert into "ages" ("Name") values ("������+��������");
insert into "ages" ("Name") values ("����+�������");

/*��������� ������ �����*/
create table "styles"
(
"Id_style" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
"Name" TEXT DEFAULT '',
"Position" INTEGER DEFAULT 0,
"IsHide" BOOLEAN DEFAULT 0
);

insert into "styles" ("Name") values ("������������ Raqs al Sharqi");
insert into "styles" ("Name") values ("���������� ��������");
insert into "styles" ("Name") values ("�� ���������� ��������");
insert into "styles" ("Name") values ("������������ �����");
insert into "styles" ("Name") values ("������������ �����");
insert into "styles" ("Name") values ("�����");
insert into "styles" ("Name") values ("��������� �����");
insert into "styles" ("Name") values ("�����");
insert into "styles" ("Name") values ("���");
insert into "styles" ("Name") values ("��������� �����");
insert into "styles" ("Name") values ("��������� �����");
insert into "styles" ("Name") values ("���");
insert into "styles" ("Name") values ("C���������� �����");
insert into "styles" ("Name") values ("Street Shaabi");
insert into "styles" ("Name") values ("������������ ������");