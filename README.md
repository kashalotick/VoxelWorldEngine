# Course work final

Engine repository: https://github.com/kashalotick/LearningOpenTK

## Setup
1. Install .NET 10.0
1. Clone this repository
2. Clone engine repository https://github.com/kashalotick/LearningOpenTK
3. Add project reference to engine repository
4. Restore denependencies
5. Build and run


## Build
### Go to project folder
```shell
cd GameApp
```


### Windows
#### Release build

```shell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

### Linux
#### Release build
```shell
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

# Controls
- WASD - move
- Space - jump/fly
- Left Shift - sprint/descend
- Mouse - look around
- Left mouse button - break block
- Right mouse button - place block
- TAB - toggle fly mode
- F - toggle free fly lock
- Scroll wheel - change block type
- B - toggle area form
- 1-9 toggle area size
- F3 - toggle debug info
- F1 - toggle ui
- Esc - open menu

# Screenshots
![Снимок экрана 2026-05-09 033318.png](docs/images/%D0%A1%D0%BD%D0%B8%D0%BC%D0%BE%D0%BA%20%D1%8D%D0%BA%D1%80%D0%B0%D0%BD%D0%B0%202026-05-09%20033318.png)
![Снимок экрана 2026-05-09 034952.png](docs/images/%D0%A1%D0%BD%D0%B8%D0%BC%D0%BE%D0%BA%20%D1%8D%D0%BA%D1%80%D0%B0%D0%BD%D0%B0%202026-05-09%20034952.png)
![Снимок экрана 2026-05-09 035058.png](docs/images/%D0%A1%D0%BD%D0%B8%D0%BC%D0%BE%D0%BA%20%D1%8D%D0%BA%D1%80%D0%B0%D0%BD%D0%B0%202026-05-09%20035058.png)
![Снимок экрана 2026-05-09 051118.png](docs/images/%D0%A1%D0%BD%D0%B8%D0%BC%D0%BE%D0%BA%20%D1%8D%D0%BA%D1%80%D0%B0%D0%BD%D0%B0%202026-05-09%20051118.png)