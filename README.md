[![Build status](https://ci.appveyor.com/api/projects/status/c040s6utdqj5fkj7?svg=true)](https://ci.appveyor.com/project/neyrox/rarog)
# rarog
SQL database  
server mode (framed tcp protocol with msgpack serialization)  
durability (database saved after each change)

## supported statements
create table, drop table, alter table add/drop column  
insert, update, select, delete

## supported operations on update
\+ (all types)  
\-, \*, / (only numeric)

## supported conditions
=, >, <, >=, <=, <>, and, or

## supported column types
int, bigint, float(same as double), double, varchar
