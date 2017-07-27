# Trouble Shooting

There are some things you need to know about how the bot works to understand the requirements and considerations to keep in mind while using the bot.

## How is ClashBuddy working

ClashBuddy makes use of the RainbowMagic technology. RainbowMagic is a powerful remote memory manipulation library that allows to read, write and even execute code in a remote process.
The RainbowMagic components used in ClashBuddy require your Android device to be setup in a special way. Please make sure your setup complies with this requirements.

### Host Requirements

The host is the Computer that is running ClashBuddy. The host is responsible for deployment of the payload to the connected device and to run the whole AI of the automation.
Since ClashBuddy is using .Net Core you're able to run the host on different platforms.
Currently supported platforms are Windows7+ x86 and x64 and OSX 10.10+. Please grab the installation package according to your host system.
During startup RainbowMagic trys to detect if you have ADB installed systemwide. When RainbowMagic isn't able to detect installed ADB it downloads the latest version and extracts it in the current executing directory.
When you want to run multiple instance of ClashBuddy on the same host it might make sense to install ADB system wide.

### Device Requirements

ClashBuddy requires an Android device. This device is called Client in RainbowMagic. The client is deployed to the device during the device setup phase. Communication with the device is done using ADB, this means you must activate USB Debugging on your device.
After the client is deployed and activated on the device it trys to connect to the host on the specified ip address. Due to limitations with Android < 5.0 it is required to have a tcp connection between the host and the client.
When your device is using Android greater or equal to 5.0 you don't need to have your device on the same network as your host. In that case the ADB port forwarding features are used.
Due to the things ClashBuddy is able to do on the Device it requires root privileges. Besides the root privileges make sure to that ptrace is enabled in your kernel.

#### Network Requirements

ClashBuddy tries to detect the internal network address of your host. The Settings Tab allows you to specify the IP to listen on.