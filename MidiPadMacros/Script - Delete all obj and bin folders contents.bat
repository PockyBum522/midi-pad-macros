REM -------========== MidiPadMacros.Core ==========-------

set folder=".\MidiPadMacros.Core\bin"

cd /d %folder%

for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q 2>NUL || del "%%i" /s/q >NUL )

set folder="..\obj"

cd /d %folder%

for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q 2>NUL || del "%%i" /s/q >NUL )

REM  -------========== MidiPadMacros.Main ==========-------

set folder="..\..\MidiPadMacros.Main\bin"

cd /d %folder%

for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q 2>NUL || del "%%i" /s/q >NUL )

set folder="..\obj"

cd /d %folder%

for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q 2>NUL || del "%%i" /s/q >NUL )

REM  -------========== MidiPadMacros.UI ==========-------

set folder="..\..\MidiPadMacros.UI\bin"

cd /d %folder%

for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q 2>NUL || del "%%i" /s/q >NUL )

set folder="..\obj"

cd /d %folder%

for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q 2>NUL || del "%%i" /s/q >NUL )

pause
