IGNORE_BIN_OBJ = -type f -not -path *bin/* -not -path *obj/*

obj/code-gen: $(shell find CodeGenerator/CodeGenerator $(IGNORE_BIN_OBJ))
	dotnet build CodeGenerator/CodeGenerator
	touch obj/code-gen