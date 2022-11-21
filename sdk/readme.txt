Geisha SDK

Content:
    - lib directory contains nuget packages of Geisha Engine. Use it in your project to build a game.
    - tools directory contains development tools making it easy to develop a game with Geisha Engine.
    - .version file contains SDK version.
    - install-geisha-cli.ps1 allows you easily install Geisha.Cli tool.
    - LICENSE file.

Installation
    1. Create nuget.config file next to solution file of your project.

    Following example nuget.config file will add nuget package source visible in Visual Studio that will allow 
    you to install Geisha nuget packages located in directory "GeishaSDK.0.8.0\lib" next to nuget.config file.

    Example:

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
        <packageSources>
            <add key="Geisha SDK" value=".\GeishaSDK.0.8.0\lib\" />
        </packageSources>
    </configuration>

    2. (optional) Install Geisha.Cli tool with install-geisha-cli.ps1 to have global access to "geisha" command line tool.