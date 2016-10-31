pushd Kekiri
dotnet build
popd

pushd Kekiri.Ioc.Autofac
dotnet build
popd

pushd Kekiri.TestRunner.NUnit
dotnet build
popd
