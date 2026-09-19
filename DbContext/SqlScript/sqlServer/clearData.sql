use TravelGuide;
go

create or alter proc clear_db_data
    @OnlySeeded bit = 1,
    @NrCommentsAffected int output,
    @NrAttractionsAffected int output,
    @NrAddressesAffected int output,
    @NrCitiesAffected int output,
    @NrCategoriesAffected int output,
    @NrUsersAffected int output,
    @NrCountriesAffected int output
as
begin
    set nocount on;

    if (@OnlySeeded = 1)
    begin
        delete from Comments where Seeded = 1;    set @NrCommentsAffected = @@ROWCOUNT;
        delete from Attractions where Seeded = 1; set @NrAttractionsAffected = @@ROWCOUNT;
        delete from Addresses where Seeded = 1;   set @NrAddressesAffected = @@ROWCOUNT;
        delete from Cities where Seeded = 1;      set @NrCitiesAffected = @@ROWCOUNT;
        delete from Categories where Seeded = 1;  set @NrCategoriesAffected = @@ROWCOUNT;
        delete from Users where Seeded = 1;       set @NrUsersAffected = @@ROWCOUNT;
        delete from Countries where Seeded = 1;   set @NrCountriesAffected = @@ROWCOUNT;
    end
    else
    begin
        delete from Comments;    set @NrCommentsAffected = @@ROWCOUNT;
        delete from Attractions; set @NrAttractionsAffected = @@ROWCOUNT;
        delete from Addresses;   set @NrAddressesAffected = @@ROWCOUNT;
        delete from Cities;      set @NrCitiesAffected = @@ROWCOUNT;
        delete from Categories;  set @NrCategoriesAffected = @@ROWCOUNT;
        delete from Users;       set @NrUsersAffected = @@ROWCOUNT;
        delete from Countries;   set @NrCountriesAffected = @@ROWCOUNT;
    end
end;