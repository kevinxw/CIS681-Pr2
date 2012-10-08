:run.bat


: for ouput on the console ------------------------



: for a non recursive search of all the files use the following command


"CodeAnal.exe" "../CodeAnal/CodeAnal/*.*" "/nr"



: for a recursive search in all the sub-directories uncomment and use the following command

:"CodeAnal.exe" "../CodeAnal/CodeAnal/*.*" "/r"



: for ouput redirection to a file uncomment and use the following------------------


:"CodeAnal.exe" "../CodeAnal/CodeAnal/*.*" "/nr" > output.rtf
