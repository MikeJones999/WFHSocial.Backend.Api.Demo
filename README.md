# WFHSocial Backend Api Demo

This is a Demo project that I started to work on mid way through last year. This is the backend api portion and has been developed using The Clean Architecture principle. 
This project is in its infancy and thus is not extensive in anyway, but shows the initial setup and early developments of the project design. 

Swagger is installed and will save time when testing. User needs to be created and then login. Once logged in use the Bearer token to make any further requests. Additionally a refresh token end point has been added. #
The User can view, and edit their profile details, they can amend their settings, and can upload a profile picture (which can been seen in the azure storage).

The users can then view and create a single post, and can recall (with use of a pagination and desc filter) all their posts - these are all stored in Azure cosmos db

logs can be seen in seq if running 

Requirements to Run - 

Azure Cosmos Db / emulator. Require a database to be created and the name inserted into the appsettings. Along with the endpoint and primary key details.  Link to Emulator https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-develop-emulator?tabs=windows%2Ccsharp&pivots=api-nosql.
Azurite (should be already connected service within application) - if not - https://learn.microsoft.com/en-gb/azure/storage/common/storage-use-azurite?tabs=visual-studio%2Cblob-storage
Azure storage explorer - Need a blob Container created within Blob containers - named uploads (Lowercase as per requirements) - emulator found here : https://azure.microsoft.com/en-gb/products/storage/storage-explorer/
Mssql server 
.NET 8

additional seq server optional - docker container used : datalust/seq:latest

I have added the appsettings.development.json just so it is easier to see what settings are required. 

