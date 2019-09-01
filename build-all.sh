#!/bin/bash

# Generate all of the auto-generated projects
OUTPUT_DIR="./GeneratedExtensionMethods"
OUTPUT_DIR=$(realpath $OUTPUT_DIR)

pushd CodeGenerator/GenerateWPF
dotnet run $OUTPUT_DIR
popd

pushd CodeGenerator/GenerateAvalonia
dotnet run $OUTPUT_DIR
popd


# Build the nuget packages
pushd GoodbyeXAML/GoodbyeXAML.Avalonia
dotnet build
popd

pushd GoodbyeXAML/GoodbyeXAML.Wpf.Core
dotnet build
popd

pushd GoodbyeXAML/GoodbyeXAML.Wpf.Framework
dotnet build
nuget pack -OutputDirectory bin/Debug
popd