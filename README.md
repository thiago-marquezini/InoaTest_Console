# InoaTest_Console

C# Test Provided by Inoa

Features:
- Watch stock exchange asset and notify (via email) when to sell or buy based on pre-defined values (API: hgbrasil free limited plan)
- Accepts multiple symbols at once (check file bin\Release\netcoreapp3.1\0 - InoaTest_Console_Multiple.bat)

Usage:
- InoaTest_Console.exe PETR4 27.40 22.50
- InoaTest_Console.exe PETR4 27.40 22.50 PETR3 26.40 22.50 BIDI3 16.40 12.50 BIDI4 16.40 12.50

External requeriments:
- RestSharp (for simple REST API calls)

> Email configuration file: bin\Release\netcoreapp3.1\appsettings.json
