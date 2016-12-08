psexec \\coreimdf01.spb.local taskkill /f /im igniteefcachestore.exe
psexec \\coreimdf02.spb.local taskkill /f /im igniteefcachestore.exe
psexec \\coreimdf03.spb.local taskkill /f /im igniteefcachestore.exe
xcopy  * \\CoreImdf01.spb.local\ignite /y 
xcopy  * \\CoreImdf02.spb.local\ignite /y 
xcopy  * \\CoreImdf03.spb.local\ignite /y 
start psexec \\coreimdf01.spb.local c:\ignite\igniteefcachestore.exe
start psexec \\coreimdf02.spb.local c:\ignite\igniteefcachestore.exe
start psexec \\coreimdf03.spb.local c:\ignite\igniteefcachestore.exe