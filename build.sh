rm -rf Overseer/bin
dotnet restore
dotnet build
cp Overseer/overseerbundle Overseer/bin/Debug/netstandard2.0/
rm -rf ~/.config/r2modmanPlus-local/RiskOfRain2/profiles/Overseer/BepInEx/plugins/Overseer
cp -r Overseer/bin/Debug/netstandard2.0/  ~/.config/r2modmanPlus-local/RiskOfRain2/profiles/Overseer/BepInEx/plugins/Overseer
cp -r Overseer/libs/YAU.dll  ~/.config/r2modmanPlus-local/RiskOfRain2/profiles/Overseer/BepInEx/plugins/Overseer/YAU.dll
cp Overseer/bin/Debug/netstandard2.0/Overseer.dll ~/Projects/Overseer/Assets/
cp Overseer/libs/YAU.dll ~/Projects/Overseer/Assets/
