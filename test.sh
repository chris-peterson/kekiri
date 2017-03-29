#!/bin/bash -e

pushd src/Kekiri.Examples.NUnit
dotnet run
popd

pushd src/Kekiri.Examples.Xunit
dotnet test
popd
