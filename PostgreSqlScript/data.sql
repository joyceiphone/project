--Create a database called Part
DROP DATABASE IF EXISTS Part;
CREATE DATABASE part;

-- Create a table named item in the Part database with the specification below
drop table if exists part.item;
create table part.item
(
	id serial not null primary key,
	item_name varchar(50) not null,
	parent_item int references item(id),
	cost int not null,
	req_date date not null
);

-- Insert data into the item table
insert into part.item (item_name, parent_item, cost, req_date)
values
('Item1', null, 500, '02-20-2024'),
('Sub1', 1, 200, '02-10-2024'),
('Sub2', 1, 300, '01-05-2024'),
('Sub3', 2, 300, '01-02-2024'),
('Sub4', 2, 400, '01-02-2024'),
('Item2', null, 600, '03-15-2024'),
('Sub1', 6, 200, '02-25-2024');

-- drop function if exists Get_Total_Cost;
create function Get_Total_Cost(item_name varchar) 
returns bigint as $$
  with recursive linked_cost (id, item_name, parent_item, cost) as
(
	select id, item_name, parent_item, cost 
	from part.item where item_name = $1 and parent_item is null
	union all
	select t.id, t.item_name, t.parent_item, t.cost
	from part.item t
	join linked_cost lc on t.parent_item = lc.id
)
select case when count(1) > 0 THEN sum(cost) else null end
FROM linked_cost;
$$ language sql;

select * from Get_Total_Cost('Item1');