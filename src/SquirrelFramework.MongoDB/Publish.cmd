

rem <!> Change the version and re-build the code under Release mode

rem generate .nuspec file.
rem nuget spec

rem Run run the following commands
rem nuget pack SquirrelFramework.csproj -Properties Configuration=Release -Properties NuspecFile=SquirrelFramework.nuspec
rem dotnet pack --configuration release -p:NuspecFile=SquirrelFramework.nuspec

rem ##########
rem The nuget package will generated when building the project.
rem ##########


rem Set the Token on local nuget setapikey *****
rem To change the version
nuget push SquirrelFramework.MongoDB.0.9.2.nupkg -Source https://www.nuget.org/api/v2/package