# Building iOS

After generating Xcode project, you need to:

1. Resolve code signing
2. Disable bitcode for both targets (**Unity-iPhone** and
   **UnityFramework.framework**)
3. For files  `ICadeReaderView.m` and `ICadeManager.m` in
   `Libraries/InControl/Plugins/iOS` uncheck **Unity-iPhone** Target Membership
   (https://forum.unity.com/threads/incontrol-xcode-build-error-icademanager.823305/
   the file is changed already)
4. For target **Unity-iPhone** in the Build Phases settings add
   **UnityFramework.framework** to Link Binary With Libraries.
   (https://issuetracker.unity3d.com/issues/m1-ios-ios-build-fails-in-xcode-when-launching-from-m1-silicon-mac-file-system-sandbox-blocked-error-loading-dlopen)
