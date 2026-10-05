# *CSharp Database Extension*

A repository for a framweork extensions package. Framweork extensions are CSM shaped utilities and tools for frameworks and languages used during development.

> For more details about each version content, please consult [Changelog](./csharp_database_extension/CHANGELOG.md)

## **Installation & Usage**

> dotnet nuget add source --name "github" --username {*GITHUB.USR*} --password {*GITHUB.PAT*} "<https://nuget.pkg.github.com/Cosmos-CSM/index.json>"

* GITHUB.USR: It's your github user account.

* GITHUB.PAT: It's a generated personal access token, go to *Settings* > *Developer Settings* > *Personal access tokens*, create a **Classic** type access token and provide at minimum **Read:Packages** permission.

> dotnet add package **CSharp.Database.Extension** --source github

## **Testing**

A testing purposes package is included, provides tools to handle easier tests setup, configurations, and managament of classes.

> dotnet add package **CSharp.Database.Extension.Testing** --source github

Highly recommended only use on testing projects.

> For more details about package content please consult [CHANGELOG]()