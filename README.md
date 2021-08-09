# UniversalUnityHooks

## Warning: Unstable release
This branch is currently used for the experimental version of UUH, v3.0. If you are looking for a production ready build of UUH, please visit the [v2](https://github.com/UserR00T/UniversalUnityHooks/tree/v2) branch.

## Download
You can download UniversalUnityHooks v3 using the produced artifacts from Github Actions. Do note you need a github account for this. In the future, I'd like to also release the build files to the "releases" section.

## Executing
Running UniversalUnityHooks is very easy. On windows, you can use the `dotnet` tool on the `UniversalUnityHooks.Core.dll`. On unix based systems, you can just use `./UniversalUnityHooks.Core`. Hopefully I can build a windows executable (`.exe`) and bundle it with in the future.  
After running UniversalUnityHooks with no arguments, you should see a help screen with arguments you can use. If you just want to inject plugins, you should use the `execute` command. Type `UniversalUnityHooks.Core execute --help` for more information.

## Building
Run the `dotnet build` tool on the project `UniversalUnityHooks.Core`. This project will be the entry point of the application. An example command would be:  
`dotnet build UniversalUnityHooks.Core/UniversalUnityHooks.Core.csproj --configuration Release`. Make sure your current working directory is set to the git repository.