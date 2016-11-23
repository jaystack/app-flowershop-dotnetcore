# app-flowershop-dotnetcore

## Installation

https://www.microsoft.com/net/download/core

## How to build

The default runtimes are:
```json
  "runtimes": {
    "win10-x64": {},
    "osx.10.10-x64": {},
    "ubuntu.14.04-x64": {}
  }
```

## Connected services

https://github.com/jaystack/service-flowershop-data-dotnetcore.git <br />
https://github.com/jaystack/app-flowershop-cart-dotnetcore.git <br />
https://github.com/jaystack/app-flowershop-items-dotnetcore.git

## How to build

```bash
$ git clone https://github.com/jaystack/app-flowershop-dotnetcore.git

$ cd app-flowershop-dotnetcore
$ dotnet restore
$ dotnet run
```

