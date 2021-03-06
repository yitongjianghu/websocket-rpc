set currPath="%cd%
set serverPath="%cd%\Server\bin\net47"
set clientPath="%cd%\Client\bin\net47"

if NOT EXIST %serverPath%\Server.exe (
  echo Build 'Server' project first.
  goto :eof
)

if NOT EXIST %clientPath%\Client.exe (
  echo Build 'Client' project first.
  goto :eof
)

cd %serverPath%
start Server.exe
timeout /T 1  > nul

cd %clientPath%
start Client.exe
timeout /T 1  > nul
start Client.exe

cd %currPath%