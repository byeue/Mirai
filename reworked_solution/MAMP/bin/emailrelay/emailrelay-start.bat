start "emailrelay" C:\MAMP\bin\emailrelay\emailrelay.exe --close-stderr --forward-to smtp.gmail.com:587 --port 25 --log --pid-file "C:\MAMP\bin\emailrelay\emailrelay.pid" --poll 0 --remote-clients --spool-dir C:\MAMP\mailspool --syslog --verbose --log-time --log-file=C:\MAMP\logs\emailrelay.log