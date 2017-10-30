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




Props to Mr. Doob and his [code editor](http://mrdoob.com/projects/code-editor/), from which
the inspiration to this, and some handy implementation hints, came.

### Libraries used:

 * SimpleInjector : Tried it out for DI; I rather like it :) 
 * EntityFramework : For data-access
 * .NET MVC + WebAPI : For implementing API 


