use TravelGuide;
go

--create a schemas
if not exists (select * from sys.schemas where name = 'gstusr')
    exec('create schema gstusr');
go
if not exists (select * from sys.schemas where name = 'usr')
    exec('create schema usr');
go

create or alter view vw_db_info as
select
    (select count(*) from Users)                as NrUsers,
    (select count(*) from Users where Seeded=1) as NrSeededUsers,
    (select count(*) from Users where Seeded=0) as NrUnseededUsers,
 
    (select count(*) from Cities)                as NrCities,
    (select count(*) from Cities where Seeded=1) as NrSeededCities,
    (select count(*) from Cities where Seeded=0) as NrUnseededCities,
 
    (select count(*) from Attractions)                as NrAttractions,
    (select count(*) from Attractions where Seeded=1) as NrSeededAttractions,
    (select count(*) from Attractions where Seeded=0) as NrUnseededAttractions,
 
    (select count(*) from Attractions a
        where exists (select 1 from Comments c where c.AttractionId = a.AttractionId)) as NrAttractionsWithComments,

    (select count(*) from Comments)                as NrComments,
    (select count(*) from Comments where Seeded=1) as NrSeededComments,
    (select count(*) from Comments where Seeded=0) as NrUnseededComments,

    (select count(*) from Addresses)                as NrAddresses,
    (select count(*) from Addresses where Seeded=1) as NrSeededAddresses,
    (select count(*) from Addresses where Seeded=0) as NrUnseededAddresses;
go