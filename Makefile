IGNORE_BIN_OBJ = -type f -not -path *bin/* -not -path *obj/*
OUTPUT_FOLDER = GeneratedExtensionMethods

.PHONY: all clean obj

all: \
	obj \
	obj/code-gen \
	obj/wpf-shared \
	obj/avalonia-shared

obj: 
	mkdir -p obj

obj/code-gen: $(shell find CodeGenerator/CodeGenerator $(IGNORE_BIN_OBJ))
	dotnet build CodeGenerator/CodeGenerator
	touch obj/code-gen

obj/wpf-shared: obj/code-gen $(shell find CodeGenerator/GenerateWPF $(IGNORE_BIN_OBJ))
	dotnet run --project CodeGenerator/GenerateWPF $(OUTPUT_FOLDER)
	touch obj/wpf-shared

obj/avalonia-shared: obj/code-gen $(shell find CodeGenerator/GenerateAvalonia $(IGNORE_BIN_OBJ))
	dotnet run --project CodeGenerator/GenerateAvalonia $(OUTPUT_FOLDER)
	touch obj/avalonia-shared

clean:
	dotnet clean CodeGenerator
	dotnet clean GoodbyeXAML
	rm -rf $(OUTPUT_FOLDER)