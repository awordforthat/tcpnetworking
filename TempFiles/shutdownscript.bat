pause
IF [%1]==[] (goto shutdown) ELSE (goto restart)
:shutdown
shutdown /s /f /t 5 /c "Controlled shutdown commencing"
EXIT 0
:restart
shutdown /r /f /t 5 /c "Controlled restart commencing"
EXIT 0
