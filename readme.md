# How to
To get everything working, you need to have the following packages installed:
 - mesa-libgl

First, build the application using `dotnet build` this way, we get a bin/Debug folder.
Then, create a symlink for `libsoloud.so` & `libglfw.so` to `${PWD}/bin/Debug/net5.0/lib`

Then, replace the contents of `GXPEngine.runtimeconfig.dev.json` & `GXPEngine.runtimeconfig.json` (located in `bin/Debug/net5.0`)
to the content of `runtimes.md`.

Lastly, `dotnet run` to run the application.