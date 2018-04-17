create database SpendingTracker;
use SpendingTracker;

drop table categories;
drop table spendings;
drop table users;

create table users
(
	user_id int primary key,
	user_name varchar(40)
);

create table spendings
(
	spending_id int primary key identity(1, 1) not null,
	user_id int not null,
	amount float,
	dt datetime,
	foreign key (user_id) references users(user_id)
);

create table categories
(
	category_name varchar(40),
	spending_id int identity(1, 1) not null, 
	foreign key (spending_id) references spendings(id)
);