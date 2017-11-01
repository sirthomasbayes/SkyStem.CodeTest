# (Skystem) Code Challenge

Directory Structure:

 * Skystem.Challenge.Core : Library of database + data-access agnostic object models and interface definitions.
 * Skystem.Challenge.Service : Contains concrete implementations of interfaces defined in Skystem.Challenge.Core. Uses EntityFramework for data-access.
 * Skystem.Challenge.App.Tests : Integration tests for concrete service implementations in Skystem.Challenge.Service.
 * Skystem.Challenge.App : Application providing API for adding/updating/removing Items, ItemAttributes (dynamic), and ItemGroups.

Overview of object models:

 * Item : Represents an item with Name, Description, and Id (PK).
 * AttributeType : Represents an attribute an Item can possess. Id (PK) and Name is Unique.
 * ItemAttribute : Represents an attribute Item with ItemId possesses. ItemId (FK into Items), AttributeId (FK into AttributeTypes), Value (value of Attribute)
 * ItemGroupAttribute : Represents an (Attribute, Value) pair we are grouping by. GroupId (FK into Groups), AttributeId (FK into AttributeTypes), Value (value of Attribute)
 * ItemGroup : Represents an ItemGroup with Name, Description, and Id (PK) . An Item i belongs to ItemGroup g if i.Attributes represents a superset of g.Attributes

Notes on SQL Server:
 * Skystem.Challenge.App.Tests : DatabaseName=SkystemTest, ConfigName=SkystemDb
 * Skystem.Challenge.App : DatabaseName=Skystem, ConfigName=SkystemDb
 * Both configurations use SQL authentication and connects as sa with password cZjJVXnu

After setting up IIS for Skystem.Challenge.App, documentation for API can be found @ /swagger

### Libraries used:

 * SimpleInjector : Tried it out for DI; I rather like it :) 
 * EntityFramework : For data-access
 * .NET MVC + WebAPI : For implementing API
 * Visual Studio's built-in Testing Framework is used for integration tests. To run tests: Menu - Test > Run > All Tests

Additional setup documentation can be found in the Skystem Setup Documentation pdf.
