C# Unique Hardware ID
============

C# class to generate an Unique Hardware ID.

Sometimes is required create unique identifiers for PCs. There are different approaches for doing that, the most basic among these is to use hardware IDs since they're constant (at most of time).

The most popular are the First Drive Volume ID and the processor ID, using the Windows Management Instrumentation (WMI) infrastructure.

This simple class (about 8kb) generates two unique IDs.

1. SimpleUID, a concatenation of the First Drive Volume ID and the processor ID.
2. AdvancedUID, the simpleUID plus a MD5 hash of the Windows build version.

###Notes:

* In Virtualized environments (eg. VirtualBox, VMware, etc) the processor is not available through the WMI. Thus, the class implements an inline Assembly solution (credits to: <http://stackoverflow.com/questions/16460485/inline-assembly-code-to-get-cpu-id>).
* The MD5 hash is generated using the Windows codename and the build number in order to avoid repetition when the build number is the same for 2 or more different operating systems. E.g. Windows 7 & Windows Server 2008 had the same build number. You can check out an approximate list of the Windows Version Numbers on <http://www.gaijin.at/en/lstwinver.php>.
* The project files and the solution files were intentionally made in Visual C# 2010 Express for convenience of users, and targets to .NET Framework 2.0 x86 with compatibility purposes.

###WARNINGS

* DO NOT use any of the generated UIDs for production environments, since the code is public, anyone can replicate the UID. I STRONGLY RECOMMEND you to scramble/encrypt the generated UID with a application specific algorithm.
* NEVER use external libraries (like this) to generate & validate licenses, due to anybody could patch the dll file with reverse engineering to break your licensing system.