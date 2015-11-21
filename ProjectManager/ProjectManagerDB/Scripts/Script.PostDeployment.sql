/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
:r .\InsertProject.sql
:r .\InsertRoleName.sql
:r .\InsertTaskState.sql
:r .\InsertProjectUser.sql
:r .\InsertRole.sql
:r .\InsertTask.sql
:r .\InsertAssignment.sql
:r .\InsertWorktime.sql