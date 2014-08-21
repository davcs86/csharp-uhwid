C# Unique Hardware ID
============

C# class to generate an Unique Hardware ID.

Sometimes is required create unique identifiers for PCs. There are different approaches for doing that, the most basic among these is to use hardware IDs since they're constant (at most of time).

The most popular are the First Drive Volume ID and the processor ID, using the Windows Management Instrumentation (WMI) infrastructure.

This simple class (about 8kb) generates two unique IDs.

1. SimpleUID, a concatenation of the First Drive Volume ID and the processor ID.
2. AdvancedUID, the simpleUID plus a MD5 hash of the Windows build version.

####Notes:

* In Virtualized environments (eg. VirtualBox, VMware, etc) the processor is not available through the WMI. Thus, the class implements an inline Assembly solution (credits to: <http://stackoverflow.com/questions/16460485/inline-assembly-code-to-get-cpu-id>).
* The MD5 hash is generated using the Windows codename and the build number in order to avoid repetition when the build number is the same for 2 or more different operating systems. E.g. Windows 7 & Windows Server 2008 had the same build number. You can check out an approximate list of the Windows Version Numbers on <http://www.gaijin.at/en/lstwinver.php>.
* The project files and the solution files were intentionally made in Visual C# 2010 Express for convenience of users, and targets to .NET Framework 2.0 x86 with compatibility purposes.

####WARNINGS

* **DO NOT** use any of the generated UIDs for production environments, since the code is public, anyone can replicate the UID. It's **STRONGLY RECOMMENDED** to scramble/encrypt the generated UID with a application specific algorithm.
* **NEVER** use external libraries (like this) to generate & validate licenses, due to anybody could patch the dll file to break your licensing system.

## Links

* Homepage: <http://d-castillo.info/projects/csharp-uhwid>
* Source: <https://github.com/davcs86/csharp-uhwid>
* Bugs:   <https://github.com/davcs86/csharp-uhwid/issues>

## Requirements:

1. .NET Framework 2.0+
2. Visual C# 2010 Express (and later).

## Download:

* As zip from this repository: <https://github.com/davcs86/csharp-uhwid/archive/master.zip>

* With git from a terminal:

    ```sh
    git clone https://github.com/davcs86/csharp-uhwid.git
    ```

##How to Use:

1. Add the file **/UHWID/UHWID.cs** to your project.

2. Add the namespace **UHWID** where you'll use the class.

    ```c#
    using UHWID;
    ```

3. Declare a new object 

    ```c#
    UniqueHWID UHWIDEngine = new UniqueHWID();
    ```

4. Get any of the UIDs

    ```c#
    String SimpleUID = UHWIDEngine.SimpleUID;
    String AdvancedUID = UHWIDEngine.AdvancedUID;
    ```

## Support

Drop me line on: <http://d-castillo.info/contactme/> or to: davcs86@gmail.com

## Donations

Did this project help you to save (or earn) some money?<br>
Please, support to the author by making a small donation.

<a href='https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2PK29ZFPUZ5WL' target='_blank'><img width="150" style='border:0px;width:150px' src='http://ko-fi.com/img/button-4.png' border='0' alt='Buy Me A Coffee :) @ PayPal' /></a>
