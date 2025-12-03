# Microsoft Store Publishing Guide
## Publishing MonoGame Desktop Applications to Microsoft Store

**Document Version:** 1.0  
**Date:** November 29, 2025  
**Project:** Paint Trek - MonoGame Desktop Game  
**Author:** Development Team  

---

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Project Configuration](#project-configuration)
4. [Creating Windows Application Packaging Project](#creating-wap-project)
5. [Building and Packaging](#building-and-packaging)
6. [Testing the Package](#testing-the-package)
7. [Publishing to Microsoft Store](#publishing-to-microsoft-store)
8. [Troubleshooting](#troubleshooting)
9. [Important Notes](#important-notes)

---

## Overview

This guide documents the complete process of publishing a MonoGame (.NET 9) desktop application to the Microsoft Store. The process involves creating a Windows Application Packaging (WAP) project, configuring build settings, and submitting to Microsoft Partner Center.

### Key Technologies
- **.NET 9.0** - Target framework
- **MonoGame 3.8** - Game framework
- **Windows Application Packaging Project** - Store packaging
- **Visual Studio 2022** - Development environment

---

## Prerequisites

### Required Software
1. **Visual Studio 2022** (17.8 or later)
   - Workload: ".NET desktop development"
   - Workload: "Universal Windows Platform development"
   - Component: "Windows App Certification Kit" (optional for testing)

2. **Windows 10 SDK** (10.0.19041.0 or later)

3. **Microsoft Partner Center Account**
   - Developer account ($19/year for individuals)
   - Register at: https://partner.microsoft.com/dashboard

### Project Requirements
- Working MonoGame desktop application
- Application icon (1080x1080 PNG)
- Screenshots (1280x800 or higher)
- Privacy policy URL
- Application description

---

## Project Configuration

### Step 1: Configure Main Project (.csproj)

Your main game project needs proper platform configurations:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net9.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>
    <Platforms>AnyCPU;x64;x86</Platforms>
  </PropertyGroup>
  
  <PropertyGroup>
    <ApplicationIcon>Icon.ico</ApplicationIcon>
    <Version>1.0.0.0</Version>
    <AssemblyVersion>1.0.0.0</AssemblyVersion>
  </PropertyGroup>
  
  <!-- Debug Configuration -->
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
    <PlatformTarget>AnyCPU</PlatformTarget>
  </PropertyGroup>
  
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|x64'">
    <PlatformTarget>x64</PlatformTarget>
  </PropertyGroup>
  
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|x86'">
    <PlatformTarget>x86</PlatformTarget>
  </PropertyGroup>
  
  <!-- Release Configuration -->
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Release|x64'">
    <Optimize>true</Optimize>
    <DebugType>none</DebugType>
    <DebugSymbols>false</DebugSymbols>
    <PlatformTarget>x64</PlatformTarget>
  </PropertyGroup>
  
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Release|x86'">
    <Optimize>true</Optimize>
    <DebugType>none</DebugType>
    <DebugSymbols>false</DebugSymbols>
    <PlatformTarget>x86</PlatformTarget>
  </PropertyGroup>
  
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Release|AnyCPU'">
    <Optimize>true</Optimize>
    <DebugType>none</DebugType>
    <DebugSymbols>false</DebugSymbols>
    <PlatformTarget>AnyCPU</PlatformTarget>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="MonoGame.Framework.WindowsDX" Version="3.8.*" />
    <PackageReference Include="MonoGame.Content.Builder.Task" Version="3.8.*" />
  </ItemGroup>
</Project>
```

### Step 2: Restore NuGet Packages

Before packaging, restore packages for all target platforms:

```bash
# Restore for x64
dotnet restore YourProject.csproj -r win-x64

# Restore for x86
dotnet restore YourProject.csproj -r win-x86
```

---

## Creating WAP Project

### Step 1: Add Windows Application Packaging Project

1. In Visual Studio, right-click on Solution → **Add** → **New Project**
2. Search for **"Windows Application Packaging Project"**
3. Name it: `WapProjectForYourApp`
4. Target version: **Windows 10, version 1809 (10.0; Build 17763)** or later
5. Minimum version: **Windows 10, version 1809 (10.0; Build 17763)**

### Step 2: Add Project Reference

1. In WAP project, right-click **Applications** → **Add Reference**
2. Select your main game project
3. Click **OK**

### Step 3: Configure Package.appxmanifest

Edit `Package.appxmanifest` in WAP project:

```xml
<Package>
  <Identity Name="YourPublisherName.PaintTrek"
            Publisher="CN=YourPublisherName"
            Version="1.0.0.0" />
  
  <Properties>
    <DisplayName>Paint Trek</DisplayName>
    <PublisherDisplayName>Your Name</PublisherDisplayName>
    <Logo>Assets\StoreLogo.png</Logo>
  </Properties>
  
  <Dependencies>
    <TargetDeviceFamily Name="Windows.Desktop" 
                        MinVersion="10.0.17763.0" 
                        MaxVersionTested="10.0.22621.0" />
  </Dependencies>
  
  <Applications>
    <Application Id="PaintTrek"
                 Executable="PaintTrek.exe"
                 EntryPoint="Windows.FullTrustApplication">
      <uap:VisualElements
          DisplayName="Paint Trek"
          Description="An exciting space shooter game"
          BackgroundColor="transparent"
          Square150x150Logo="Assets\Square150x150Logo.png"
          Square44x44Logo="Assets\Square44x44Logo.png">
        <uap:DefaultTile Wide310x150Logo="Assets\Wide310x150Logo.png" />
        <uap:SplashScreen Image="Assets\SplashScreen.png" />
      </uap:VisualElements>
    </Application>
  </Applications>
</Package>
```

### Step 4: Add Required Assets

Create these image assets in `Assets` folder:

- **Square44x44Logo.png** - 44x44 pixels (App icon)
- **Square150x150Logo.png** - 150x150 pixels (Medium tile)
- **Square310x310Logo.png** - 310x310 pixels (Large tile)
- **Wide310x150Logo.png** - 310x150 pixels (Wide tile)
- **SplashScreen.png** - 620x300 pixels (Splash screen)
- **StoreLogo.png** - 50x50 pixels (Store logo)

---

## Building and Packaging

### Step 1: Configure Build Settings

1. Open **Configuration Manager** (Build → Configuration Manager)
2. Set configurations:
   ```
   Active solution configuration: Release
   Active solution platform: x64
   
   Main Project:
   ☑ Build
   Platform: x64
   
   WAP Project:
   ☑ Build
   ☑ Deploy
   Platform: x64
   ```

### Step 2: Create App Package

1. Right-click on **WAP Project** → **Publish** → **Create App Packages**

2. **Select Distribution Method:**
   - For testing: **Sideloading**
   - For Microsoft Store: **Microsoft Store under a new app name**

3. **Sign in to Microsoft Partner Center** (if publishing to Store)

4. **Configure Package Settings:**
   ```
   Output location: [Default or custom path]
   Version: 1.0.0.0
   ☑ Automatically increment
   
   Generate app bundle: Always
   
   Architecture Selection:
   ☑ x86  - Release (x86)
   ☑ x64  - Release (x64)
   ☐ ARM  - Release (ARM)
   ☐ ARM64 - Release (ARM64)
   ☐ Neutral - Release (Any CPU)
   
   ☑ Include public symbol files (for crash analysis)
   ☑ Generate artifacts to validate app with Windows App Certification Kit
   ```

5. Click **Create**

### Step 3: Build Process

The build process will:
1. Compile your game for each selected architecture
2. Package all files into `.appx` files
3. Create an `.appxbundle` containing all architectures
4. Generate installation scripts
5. Create a test certificate (for sideloading)

**Output Files:**
- `YourApp_1.0.0.0_x86_x64.appxbundle` - Main package file
- `YourApp_1.0.0.0_x86_x64.cer` - Test certificate
- `Add-AppDevPackage.ps1` - Installation script
- `Install.ps1` - Alternative installation script

---

## Testing the Package

### Method 1: Local Installation (Sideloading)

1. Navigate to output folder:
   ```
   [Solution]\WapProject\AppPackages\[PackageName]_Test\
   ```

2. Open **PowerShell as Administrator**

3. Run installation script:
   ```powershell
   .\Add-AppDevPackage.ps1
   ```

4. Follow prompts to install certificate and app

5. Launch app from Start Menu

### Method 2: Windows App Certification Kit (Optional)

**Note:** WACK is optional. If you encounter the error "AppCert.exe wasn't found", you can skip this test or install WACK through Visual Studio Installer.

To install WACK:
1. Open **Visual Studio Installer**
2. Click **Modify** on your Visual Studio installation
3. Go to **Individual components** tab
4. Search for **"Windows App Certification Kit"**
5. Check the box and click **Modify**

To run tests:
1. After package creation, click **"Launch Windows App Certification Kit"**
2. Select your `.appxbundle` file
3. Run all tests (takes 5-15 minutes)
4. Review results and fix any issues

**Common WACK Tests:**
- Application launch test
- Suspend/resume test
- Performance test
- Security test
- Package compliance test

**Alternative Testing Tools:**
- Manual testing on different devices
- Beta testing with users
- Third-party testing services

---

## Publishing to Microsoft Store

### Step 1: Create Microsoft Partner Center Account

1. Go to https://partner.microsoft.com/dashboard
2. Sign up for developer account ($19/year for individuals)
3. Complete identity verification
4. Accept agreements

### Step 2: Reserve App Name

1. In Partner Center, go to **Apps and games**
2. Click **New product** → **MSIX or PWA app**
3. Enter app name: **Paint Trek**
4. Check availability and reserve

### Step 3: Create Store Listing

Fill in required information:

**Product Identity:**
- App name: Paint Trek
- Category: Games → Action & adventure
- Subcategory: Arcade

**Properties:**
- Privacy policy URL: [Your URL]
- Age rating: 7+ (or appropriate rating)
- Price: Free or set price

**Store Listings (English):**

**Description:**
```
Paint Trek is an exciting space shooter game where you navigate through 
challenging levels, defeat enemies, and collect power-ups. Features:

• 10 unique levels with increasing difficulty
• Multiple weapon types and power-ups
• Boss battles
• Smooth 60 FPS gameplay
• Retro-inspired graphics
• Original soundtrack

Pilot your ship through dangerous space sectors, upgrade your weapons, 
and become the ultimate space pilot!
```

**Screenshots:**
- Upload 1-10 screenshots (1280x800 or higher)
- Show gameplay, menus, and key features
- Add captions describing each screenshot

**App Icon:**
- 1080x1080 PNG
- Transparent background recommended
- Clear, recognizable design

**Additional Assets:**
- Promotional images (optional)
- Trailers (optional)

### Step 4: Upload Package

1. Go to **Packages** section
2. Click **Upload package**
3. Select your `.appxbundle` file
4. Wait for validation (automatic)
5. Review any warnings or errors

### Step 5: Submit for Certification

1. Review all sections (green checkmarks)
2. Click **Submit to the Store**
3. Certification process begins (1-3 days typically)
4. Monitor status in Partner Center

### Step 6: Certification Process

Microsoft will review:
- Package integrity
- Content policy compliance
- Technical requirements
- Age rating accuracy
- Privacy policy

**Possible Outcomes:**
- ✅ **Approved** - App goes live
- ⚠️ **Needs attention** - Fix issues and resubmit
- ❌ **Rejected** - Review feedback and make changes

---

## Troubleshooting

### Issue 1: "Assets file not found" Error

**Error Message:**
```
Assets file 'obj\wappublish\win-x64\project.assets.json' not found.
Run a NuGet package restore to generate this file.
```

**Solution:**
```bash
dotnet restore YourProject.csproj -r win-x64
dotnet restore YourProject.csproj -r win-x86
```

### Issue 2: Platform Mismatch

**Error Message:**
```
There was a mismatch between the processor architecture of the project 
being built "MSIL" and the processor architecture of the reference "x86".
```

**Solution:**
Ensure `.csproj` has proper platform configurations (see Project Configuration section).

### Issue 3: "Microsoft.DesktopBridge.props not found"

**Error Message:**
```
The imported project "Microsoft.DesktopBridge.props" was not found.
```

**Solution:**
This is a known issue with .NET 9 and WAP projects. Workarounds:
1. Use .NET 8 for packaging project
2. Use alternative packaging methods (Inno Setup, WiX)
3. Update Visual Studio to latest version

### Issue 4: MonoGame Content Not Included

**Problem:** Game runs but content (textures, sounds) is missing.

**Solution:**
Ensure `Content` folder is set to copy to output:
```xml
<ItemGroup>
  <Content Include="Content\**\*.*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

### Issue 5: App Crashes on Launch

**Common Causes:**
1. Missing dependencies (.NET runtime)
2. Content files not found
3. File path issues (absolute vs relative)

**Solution:**
- Test with self-contained deployment
- Check Event Viewer for crash details
- Add error logging to your app

### Issue 6: "Product doesn't install on selected device families"

**Error Message:**
```
The product doesn't install on the currently selected device families. 
Please check that the device family selection is correct and re-submit the product.
Tested devices: HP 17-bs011dx
```

**Problem:** Package.appxmanifest only targets `Windows.Desktop` device family, but Microsoft Store requires `Windows.Universal` support for broader compatibility.

**Solution:**

1. **Update Package.appxmanifest** to include both Universal and Desktop device families:

```xml
<Dependencies>
  <TargetDeviceFamily Name="Windows.Universal" MinVersion="10.0.17134.0" MaxVersionTested="10.0.22621.0" />
  <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.17134.0" MaxVersionTested="10.0.22621.0" />
</Dependencies>
```

2. **Add RuntimeIdentifiers to .csproj** for multi-architecture support:

```xml
<PropertyGroup>
  <!-- Microsoft Store Compatibility -->
  <RuntimeIdentifiers>win-x86;win-x64;win-arm64</RuntimeIdentifiers>
  <SelfContained>false</SelfContained>
  <PublishSingleFile>false</PublishSingleFile>
  <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
</PropertyGroup>
```

3. **Lower MinVersion** for better compatibility:
   - Old: `10.0.17763.0` (Windows 10 1809)
   - New: `10.0.17134.0` (Windows 10 1803)
   - This increases device compatibility by ~6 months of Windows 10 versions

4. **Rebuild package** with updated configuration

**Why This Happens:**
- Microsoft Store certification tests on various device types
- Desktop-only apps may fail on hybrid devices (Surface, tablets)
- Universal device family ensures broader compatibility

**Verification:**
After making changes, rebuild and test on:
- Desktop PC (x64)
- Laptop (x64)
- Tablet mode (if available)
- Different Windows 10/11 versions

---

## Important Notes

### Architecture Selection

**Recommended for Desktop Games:**
- ✅ **x64** - Modern 64-bit systems (required)
- ✅ **x86** - Older 32-bit systems (optional, increases compatibility)
- ❌ **ARM/ARM64** - Not needed for desktop games (Surface devices)
- ❌ **Neutral** - Only for architecture-independent apps

**Why x64 + x86:**
- x64: Covers 95%+ of modern PCs
- x86: Adds compatibility for older systems
- Bundle size increases but reaches more users

### Sideloading vs Microsoft Store

**Sideloading Package:**
- For testing and distribution outside Store
- Requires developer mode or certificate installation
- Can be shared with testers
- Not suitable for Store submission

**Microsoft Store Package:**
- Signed by Microsoft
- Automatic updates
- Trusted installation
- Required for Store publication

**Important:** Always create a **new package** specifically for Store submission, even if you already have a sideloading package.

### Version Numbering

Follow semantic versioning:
```
Major.Minor.Build.Revision
1.0.0.0 - Initial release
1.0.1.0 - Bug fixes
1.1.0.0 - New features
2.0.0.0 - Major update
```

Enable **"Automatically increment"** to avoid version conflicts.

### Content Policy

Ensure your game complies with Microsoft Store policies:
- No inappropriate content
- Accurate age rating
- No misleading information
- Proper licensing for assets
- Privacy policy if collecting data

### Performance Considerations

**Optimize for Store:**
- Fast startup time (< 5 seconds)
- Smooth 60 FPS gameplay
- Low memory usage
- Proper suspend/resume handling
- No memory leaks

### Update Strategy

**Planning Updates:**
1. Fix critical bugs quickly (hotfix)
2. Bundle minor fixes (monthly)
3. Major features (quarterly)
4. Keep users informed via Store description

**Update Process:**
1. Increment version number
2. Create new package
3. Upload to Partner Center
4. Submit for certification
5. Users get automatic updates

---

## Alternative Packaging Methods

If WAP project doesn't work for your scenario:

### Option 1: Inno Setup (Recommended)

**Pros:**
- Free and open source
- Easy to use
- Creates professional installers
- Works with any .NET version

**Cons:**
- Not for Microsoft Store
- Manual distribution

**Basic Script:**
```iss
[Setup]
AppName=Paint Trek
AppVersion=1.0.0
DefaultDirName={pf}\PaintTrek
DefaultGroupName=Paint Trek
OutputDir=installer
OutputBaseFilename=PaintTrek_Setup
Compression=lzma2
SolidCompression=yes
SetupIconFile=Icon.ico

[Files]
Source: "bin\Release\net9.0-windows\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\Paint Trek"; Filename: "{app}\PaintTrek.exe"
Name: "{commondesktop}\Paint Trek"; Filename: "{app}\PaintTrek.exe"

[Run]
Filename: "{app}\PaintTrek.exe"; Description: "Launch Paint Trek"; Flags: postinstall nowait
```

### Option 2: WiX Toolset

**Pros:**
- Industry standard
- MSI packages
- Advanced customization

**Cons:**
- Steeper learning curve
- XML-based configuration

### Option 3: MSIX Packaging Tool

**Pros:**
- GUI-based
- Converts existing installers
- Microsoft official tool

**Cons:**
- Requires existing installer
- Limited automation

---

## Checklist

### Before Packaging
- [ ] Game runs without errors
- [ ] All content files included
- [ ] Icon and assets prepared
- [ ] Version number updated
- [ ] Privacy policy created
- [ ] Screenshots captured

### During Packaging
- [ ] Correct distribution method selected
- [ ] Both x86 and x64 architectures selected
- [ ] Package builds successfully
- [ ] No build warnings or errors

### Before Store Submission
- [ ] Local testing completed
- [ ] WACK tests passed (optional)
- [ ] Store listing prepared
- [ ] Age rating determined
- [ ] Price decided
- [ ] Partner Center account ready

### After Submission
- [ ] Monitor certification status
- [ ] Respond to any feedback
- [ ] Plan marketing strategy
- [ ] Prepare for user feedback
- [ ] Plan update schedule

---

## Resources

### Official Documentation
- Microsoft Partner Center: https://partner.microsoft.com/dashboard
- MSIX Packaging: https://docs.microsoft.com/windows/msix/
- Store Policies: https://docs.microsoft.com/windows/uwp/publish/store-policies
- MonoGame Documentation: https://docs.monogame.net/

### Tools
- Visual Studio: https://visualstudio.microsoft.com/
- Inno Setup: https://jrsoftware.org/isinfo.php
- WiX Toolset: https://wixtoolset.org/
- MSIX Packaging Tool: https://www.microsoft.com/store/productId/9N5LW3JBCXKF

### Community
- MonoGame Community: https://community.monogame.net/
- Microsoft Q&A: https://docs.microsoft.com/answers/
- Stack Overflow: https://stackoverflow.com/questions/tagged/monogame

---

## Conclusion

Publishing a MonoGame application to Microsoft Store requires careful configuration and attention to detail. Key takeaways:

1. **Proper project configuration** is essential for multi-architecture support
2. **WAP projects** bridge desktop apps to Store packaging
3. **Testing** can be done locally; WACK is optional
4. **Store submission** requires complete metadata and assets
5. **Alternative methods** exist if WAP doesn't work

Remember: The first submission is always the hardest. Once configured, updates are straightforward.

**Good luck with your game launch!** 🚀

---

**Document History:**
- v1.0 (2025-11-29): Initial version based on Paint Trek project experience

**Contributors:**
- Development Team
- AI Assistant (Kiro)

---

*This document is provided as-is for educational purposes. Always refer to official Microsoft documentation for the most current information.*
