git clean -xdf
dotnet restore
dotnet pack --configuration Release
mkdir packages
find . -name "*.nupkg" -exec cp {} ./packages \;
open packages
