# *CSharp Database Extension*

A repository for a framweork extensions package. Framweork extensions are CSM shaped utilities and tools for frameworks and languages used during development.

[Changelog](./csharp_database_extension/CHANGELOG.md)

## **Installation & Usage**

// --> Example for NuGet based packages.

> dotnet nuget add source --name "github" --username {*GITHUB.USR*} --password {*GITHUB.PAT*} "<https://nuget.pkg.github.com/Cosmos-CSM/index.json>"

* GITHUB.USR: It's your github user account.

* GITHUB.PAT: It's a generated personal access token, go to *Settings* > *Developer Settings* > *Personal access tokens*, create a **Classic** type access token and provide at minimum **Read:Packages** permission.

> dotnet add package **CSharp.Database.Extension** --source github
