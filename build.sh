rm -rf Overseer/bin
dotnet restore
dotnet build
cp Overseer/overseerbundle Overseer/bin/Debug/netstandard2.0/
rm -rf ~/.config/r2modmanPlus-local/RiskOfRain2/profiles/Overseer/BepInEx/plugins/Overseer
cp -r Overseer/bin/Debug/netstandard2.0/  ~/.config/r2modmanPlus-local/RiskOfRain2/profiles/Overseer/BepInEx/plugins/Overseer
cp -r Overseer/libs/YAU.dll  ~/.config/r2modmanPlus-local/RiskOfRain2/profiles/Overseer/BepInEx/plugins/Overseer/YAU.dll
cp Overseer/bin/Debug/netstandard2.0/Overseer.dll ~/Projects/Overseer/Assets/
cp Overseer/libs/YAU.dll ~/Projects/Overseer/Assets/

rm -rf OverseerBuild
mkdir OverseerBuild
cp manifest.json OverseerBuild
cp README.md OverseerBuild
cp icon.png OverseerBuild
cp Overseer/bin/Debug/netstandard2.0/Overseer.dll OverseerBuild
cp Overseer/overseerbundle OverseerBuild
cd OverseerBuild
zip ../Overseer.zip *
cd ..