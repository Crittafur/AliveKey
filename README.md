# AliveKey

**AliveKey** is a simple .NET console app that simulates periodic keypresses to prevent a system from going idle or entering sleep mode.

## Note on Detection

This program uses system calls to check for user input inactivity via `user32.dll`. Some monitoring/spyware software used by companies may flag or detect this type of activity, as it interacts with system APIs that track input events. *Use at your own risk*, especially in environments where monitoring software is in place.