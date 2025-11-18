# Thermal Hunt - Mobile Deployment Readiness Checklist

Complete checklist for deploying Thermal Hunt to Apple App Store and Google Play Store.

---

## Table of Contents

1. [Pre-Deployment Requirements](#pre-deployment-requirements)
2. [iOS App Store Requirements](#ios-app-store-requirements)
3. [Android Play Store Requirements](#android-play-store-requirements)
4. [Build Configuration](#build-configuration)
5. [Testing Requirements](#testing-requirements)
6. [Store Assets Requirements](#store-assets-requirements)
7. [Legal & Compliance](#legal--compliance)
8. [Pre-Submission Final Checks](#pre-submission-final-checks)

---

## 1. Pre-Deployment Requirements

### Technical Prerequisites

- [ ] Unity 2022.3.20f1 LTS installed
- [ ] Latest Xcode (14.0+) installed (iOS)
- [ ] Android Studio 2022+ installed (Android)
- [ ] Valid Apple Developer Account ($99/year)
- [ ] Valid Google Play Developer Account ($25 one-time)
- [ ] Code signing certificates configured
- [ ] App IDs registered on both platforms

### Code Completeness

- [ ] All core gameplay systems functional
- [ ] UI fully implemented and polished
- [ ] Audio assets imported and integrated
- [ ] Particle effects implemented
- [ ] Tutorial/onboarding complete
- [ ] Analytics integration tested
- [ ] Ad integration tested (Unity Ads)
- [ ] IAP integration tested (Unity IAP)
- [ ] Save/load system tested
- [ ] No debug code in production builds
- [ ] Logging reduced (no excessive console spam)

### Performance Targets Met

- [ ] iOS: 60 FPS on iPhone 12+, 30 FPS on iPhone 8
- [ ] Android: 60 FPS on equivalent devices (SD 865+)
- [ ] Memory usage < 1GB RAM
- [ ] Build size < 150MB (ideally < 100MB)
- [ ] Load time < 5 seconds
- [ ] No memory leaks detected
- [ ] Battery drain acceptable (< 10% per 15 min)
- [ ] No overheating issues on tested devices

---

## 2. iOS App Store Requirements

### App Store Connect Setup

- [ ] App created in App Store Connect
- [ ] Bundle ID matches: `com.yourstudio.thermalhunter`
- [ ] App Name: "Thermal Hunt" (or approved variant)
- [ ] Primary language set
- [ ] Categories selected:
  - Primary: Games > Action
  - Secondary: Games > Simulation
- [ ] Age rating completed (12+)
- [ ] Privacy policy URL provided
- [ ] Support URL provided
- [ ] Marketing URL (optional)

### Technical Requirements

- [ ] **Minimum iOS version**: 14.0
- [ ] **Target SDK**: Latest iOS version
- [ ] **Architecture**: ARM64 only (no 32-bit)
- [ ] **Scripting backend**: IL2CPP
- [ ] **Build output**: .ipa file
- [ ] **Bitcode**: Disabled (not required anymore)
- [ ] **Code signing**: Distribution certificate
- [ ] **Provisioning profile**: App Store distribution profile

### Unity iOS Build Settings

```
File > Build Settings > iOS

Platform Settings:
- Target SDK: Device SDK
- Target minimum iOS Version: 14.0
- Architecture: ARM64
- Symlink Unity libraries: Disabled

Player Settings (Edit > Project Settings > Player > iOS):
- Company Name: [Your Studio Name]
- Product Name: Thermal Hunt
- Bundle Identifier: com.yourstudio.thermalhunter
- Version: 1.0.0
- Build: 1 (increment for each submission)

Other Settings:
- Scripting Backend: IL2CPP
- Target Architectures: ARM64
- Strip Engine Code: Enabled
- Managed Stripping Level: Medium
- Script Compilation: Release

Optimization:
- Graphics API: Metal
- Color Space: Linear (or Gamma)
- Lightmap Encoding: Normal Quality
- HDR Cubemap Encoding: Normal Quality

Icon Settings:
- Override for iOS: Yes
- App Icons: All sizes provided (20×20 to 1024×1024)
```

### iOS-Specific Features

- [ ] Camera permission NOT requested (not needed for game)
- [ ] Microphone permission NOT requested
- [ ] Photo library permission NOT requested
- [ ] Location permission NOT requested
- [ ] Push notifications NOT implemented (unless planned)
- [ ] Game Center integration (optional):
  - [ ] Leaderboards configured
  - [ ] Achievements configured
- [ ] iCloud saves (optional but recommended)
- [ ] Universal links configured (if needed)

### App Store Content Requirements

- [ ] **Screenshots**:
  - [ ] iPhone 6.7" (iPhone 14 Pro Max): 3-10 screenshots
  - [ ] iPhone 6.5" (iPhone 11 Pro Max): 3-10 screenshots
  - [ ] iPhone 5.5" (iPhone 8 Plus): 3-10 screenshots (optional)
  - [ ] iPad Pro 12.9" (6th gen): 3-10 screenshots (if iPad support)

- [ ] **App Preview Videos** (Optional but recommended):
  - [ ] iPhone 6.7": 15-30 second video
  - [ ] iPhone 6.5": 15-30 second video

- [ ] **App Icon**: 1024×1024 PNG (no transparency, no alpha)

- [ ] **App Description**:
  - [ ] Compelling 4000 character max description
  - [ ] Keywords optimized (100 character max)
  - [ ] What's New text for this version

- [ ] **Promotional Text**: 170 characters (editable anytime)

### App Review Information

- [ ] Demo account credentials (if applicable)
- [ ] Contact information for reviewer
- [ ] Notes for reviewer explaining:
  - [ ] How to test ads (rewarded/interstitial)
  - [ ] How to test IAP
  - [ ] Any unique gameplay mechanics
  - [ ] Beta test info (TestFlight)

### App Privacy Details

- [ ] Privacy Policy URL (active and accessible)
- [ ] Data collection disclosure:
  - [ ] Analytics data (Unity Analytics)
  - [ ] Advertising ID (for ads)
  - [ ] Purchase history (for IAP)
  - [ ] Crash logs
  - [ ] Device ID (anonymous)
  - [ ] Gameplay data

- [ ] Data usage purposes:
  - [ ] Analytics
  - [ ] App functionality
  - [ ] Advertising
  - [ ] Product personalization

### App Store Guidelines Compliance

- [ ] No private APIs used
- [ ] No undocumented features accessed
- [ ] App icon matches actual game content
- [ ] Screenshots are from actual gameplay
- [ ] No misleading metadata
- [ ] Age rating accurate (12+ for mild violence)
- [ ] No malicious code or spyware
- [ ] No copyright violations
- [ ] Ads clearly marked as ads
- [ ] IAP properly implemented (restore purchases)
- [ ] No gambling mechanics (unless licensed)

### TestFlight Beta Testing (Recommended)

- [ ] Internal testing group created
- [ ] Beta build uploaded
- [ ] Test notes provided
- [ ] 5-10 testers invited
- [ ] Testing period: 1-2 weeks
- [ ] Major bugs fixed before submission
- [ ] User feedback incorporated

---

## 3. Android Play Store Requirements

### Google Play Console Setup

- [ ] App created in Google Play Console
- [ ] Package name: `com.yourstudio.thermalhunter`
- [ ] App name: "Thermal Hunt"
- [ ] Default language set
- [ ] Categories selected:
  - Category: Games > Action
  - Tags: Hunting, Shooter, Tactical
- [ ] Content rating questionnaire completed
- [ ] Target audience: Age 13+ (Teen)
- [ ] Privacy policy URL provided
- [ ] Developer contact info provided

### Technical Requirements

- [ ] **Minimum SDK**: API Level 21 (Android 5.0 Lollipop)
- [ ] **Target SDK**: API Level 33 (Android 13) or latest
- [ ] **Architecture**: ARM64 (required), ARMv7 (optional)
- [ ] **Scripting backend**: IL2CPP
- [ ] **Build output**: .aab (Android App Bundle) required
- [ ] **Signing**: Keystore configured and secured
- [ ] **64-bit compliance**: Yes (mandatory since Aug 2019)

### Unity Android Build Settings

```
File > Build Settings > Android

Platform Settings:
- Texture Compression: ASTC
- Build System: Gradle
- Build App Bundle (Google Play): Enabled

Player Settings (Edit > Project Settings > Player > Android):
- Company Name: [Your Studio Name]
- Product Name: Thermal Hunt
- Package Name: com.yourstudio.thermalhunter
- Version: 1.0.0
- Bundle Version Code: 1 (increment for each upload)

Other Settings:
- Scripting Backend: IL2CPP
- Target Architectures: ARM64 (required), ARMv7 (optional)
- Install Location: Automatic
- Internet Access: Require (for ads/analytics)
- Write Permission: Internal Only

Identification:
- Override Default Keystore: Yes
- Keystore: [Your keystore path]
- Keystore Password: [Secure password]
- Alias: thermalhunter
- Alias Password: [Secure password]

Publishing Settings:
- Build: Release
- Minify: Release
- Proguard: Enabled (if using)

Optimization:
- Graphics API: OpenGL ES 3, Vulkan (auto-select)
- Color Space: Linear
- Multithreaded Rendering: Enabled
- Static Batching: Enabled
- Dynamic Batching: Enabled

Icon Settings:
- Override for Android: Yes
- Adaptive Icon: Foreground + Background layers
- All density icons provided (48×48 to 192×192)
```

### Android-Specific Permissions

```xml
<!-- Required permissions (add to AndroidManifest.xml if needed) -->
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="android.permission.WAKE_LOCK" />

<!-- NOT required (ensure these are NOT present) -->
<!-- <uses-permission android:name="android.permission.CAMERA" /> -->
<!-- <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" /> -->
<!-- <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" /> -->
```

- [ ] Only required permissions declared
- [ ] No dangerous permissions requested unnecessarily
- [ ] Runtime permission requests implemented properly

### Play Store Content Requirements

- [ ] **Screenshots**:
  - [ ] Phone: Minimum 2, maximum 8 (16:9 or 9:16 aspect)
  - [ ] 7-inch tablet: Minimum 1 (optional)
  - [ ] 10-inch tablet: Minimum 1 (optional)
  - [ ] Recommended: 1080×1920 or 1920×1080

- [ ] **Feature Graphic**: 1024×500 (required)
  - High quality banner for Play Store listing
  - No text that's hard to read

- [ ] **App Icon**: 512×512 PNG (32-bit, max 1MB)
  - High resolution version for Play Store

- [ ] **Promo Video** (Optional but recommended):
  - YouTube video link
  - 30 seconds to 2 minutes

- [ ] **Short Description**: 80 characters max
  - Catchy one-liner about the game

- [ ] **Full Description**: 4000 characters max
  - Compelling description with features
  - Keywords for ASO optimization

### Store Listing Content

- [ ] **App Category**: Games > Action
- [ ] **Content Rating**:
  - [ ] Complete IARC questionnaire
  - [ ] Expected rating: Everyone 10+, Teen, or Mature 17+
  - [ ] Declare violence level (fantasy violence, mild violence)

- [ ] **Target Audience**:
  - [ ] Select age groups (13+)
  - [ ] Declare if app designed for children (No)

- [ ] **Data Safety**:
  - [ ] What data is collected
  - [ ] How data is used
  - [ ] Security practices
  - [ ] Data retention policy

### Google Play Policies Compliance

- [ ] No deceptive behavior
- [ ] No malware or mobile unwanted software
- [ ] Ads policy compliance:
  - [ ] Ads clearly distinguishable from content
  - [ ] No ads that interfere with gameplay
  - [ ] No full-screen interstitial ads during gameplay
  - [ ] Ad frequency reasonable
- [ ] No gambling (unless licensed)
- [ ] No restricted content (hate speech, violence, etc.)
- [ ] Proper attribution for third-party content
- [ ] COPPA compliance (if targeting children)
- [ ] GDPR compliance (EU users)

### Internal Testing Track (Recommended)

- [ ] Internal testing track created
- [ ] .aab file uploaded
- [ ] Release notes added
- [ ] Testers added (up to 100)
- [ ] Testing period: 1-2 weeks
- [ ] Critical bugs fixed
- [ ] Device compatibility verified

### Pre-Launch Report

- [ ] Reviewed automated pre-launch report
- [ ] No critical crashes detected
- [ ] Security vulnerabilities addressed
- [ ] Performance issues resolved

---

## 4. Build Configuration

### Optimization Settings (Both Platforms)

- [ ] **IL2CPP Code Stripping**:
  - Enabled, set to Medium or High
  - Link.xml file created for any stripped classes

- [ ] **Texture Compression**:
  - iOS: ASTC, PVRTC
  - Android: ETC2, ASTC

- [ ] **Audio Compression**:
  - Background music: Vorbis, Quality 70%
  - SFX: ADPCM or Vorbis, Quality 100%

- [ ] **Shader Stripping**:
  - Unused shaders stripped
  - Shader variants reduced

- [ ] **Asset Bundle Configuration** (if used):
  - Compressed bundles
  - LZ4 compression for faster loading

### Build Variants

- [ ] **Development Build**: OFF (for production)
- [ ] **Script Debugging**: OFF
- [ ] **Deep Profiling**: OFF
- [ ] **Autoconnect Profiler**: OFF
- [ ] **Compression Method**: LZ4HC (best balance)

### Platform-Specific Compilation

- [ ] Platform-dependent code uses proper defines:
  ```csharp
  #if UNITY_IOS
      // iOS-specific code
  #elif UNITY_ANDROID
      // Android-specific code
  #elif UNITY_STANDALONE
      // PC-specific code
  #endif
  ```

- [ ] No platform-specific APIs called without guards

---

## 5. Testing Requirements

### Device Testing Matrix

#### iOS Devices (Minimum)

- [ ] iPhone 14 Pro Max (latest, high-end)
- [ ] iPhone 12 (mid-range, common)
- [ ] iPhone 8 (minimum supported, older)
- [ ] iPad (if supporting tablets)

#### Android Devices (Minimum)

- [ ] Samsung Galaxy S23 (high-end, Snapdragon)
- [ ] Google Pixel 6 (mid-range, stock Android)
- [ ] OnePlus 9 (mid-range, OxygenOS)
- [ ] Budget device with SD 720 (low-end, minimum spec)

### Test Cases Checklist

#### Functional Testing

- [ ] App launches successfully
- [ ] Tutorial completes without errors
- [ ] Main menu navigation works
- [ ] Contract selection works
- [ ] Loadout selection works
- [ ] Mission starts correctly
- [ ] Gameplay mechanics functional:
  - [ ] Shooting and hit detection
  - [ ] Thermal vision toggle
  - [ ] Battery management
  - [ ] AI behavior correct
  - [ ] Score calculation accurate
  - [ ] Mission objectives track properly
- [ ] Mission completion/failure works
- [ ] Save/load system works
- [ ] Settings apply correctly
- [ ] Pause/resume works
- [ ] Back button behavior (Android)

#### Monetization Testing

- [ ] Rewarded ads show and grant rewards
- [ ] Interstitial ads show at correct times
- [ ] Ad frequency capping works
- [ ] IAP flow works for all products
- [ ] Purchase restoration works
- [ ] "Remove Ads" IAP disables ads
- [ ] Currency purchases add currency
- [ ] Receipt validation works

#### Performance Testing

- [ ] Consistent 30-60 FPS during gameplay
- [ ] No frame drops or stuttering
- [ ] Memory usage stable (no leaks)
- [ ] Battery drain acceptable
- [ ] Device doesn't overheat
- [ ] Load times acceptable (< 5 seconds)
- [ ] No excessive GC allocations

#### Stress Testing

- [ ] Long play sessions (30+ minutes)
- [ ] Rapid mission completion (10+ missions)
- [ ] App backgrounding/foregrounding
- [ ] Low memory conditions
- [ ] Poor network conditions
- [ ] Airplane mode handling

#### Edge Cases

- [ ] No internet connection:
  - [ ] App launches offline
  - [ ] Gameplay works offline
  - [ ] Ads gracefully fail
  - [ ] Save/load works offline
- [ ] First launch experience
- [ ] Data wipe and restore
- [ ] Different screen orientations (if supported)
- [ ] Different aspect ratios
- [ ] Notched displays (iPhone X+)
- [ ] Different languages (if localized)

#### Regression Testing

- [ ] All previously fixed bugs remain fixed
- [ ] No new bugs introduced
- [ ] All features from previous version work

---

## 6. Store Assets Requirements

### Required Asset Sizes

#### iOS App Store

| Asset | Size | Format | Notes |
|-------|------|--------|-------|
| App Icon | 1024×1024 | PNG | No transparency |
| iPhone 14 Pro Max | 1290×2796 | PNG/JPG | 3-10 screenshots |
| iPhone 11 Pro Max | 1242×2688 | PNG/JPG | 3-10 screenshots |
| iPhone 8 Plus | 1242×2208 | PNG/JPG | Optional |
| iPad Pro 12.9" | 2048×2732 | PNG/JPG | If supporting iPad |

#### Android Play Store

| Asset | Size | Format | Notes |
|-------|------|--------|-------|
| App Icon | 512×512 | PNG | 32-bit with alpha |
| Feature Graphic | 1024×500 | PNG/JPG | Required |
| Phone Screenshots | 1080×1920 | PNG/JPG | 2-8 required |
| 7" Tablet | 1200×1920 | PNG/JPG | Optional |
| 10" Tablet | 1920×1200 | PNG/JPG | Optional |

### Screenshot Content Guidelines

- [ ] Show actual gameplay (no mockups)
- [ ] Highlight thermal vision feature
- [ ] Show different environments
- [ ] Display UI elements clearly
- [ ] Include action shots (rats, shooting)
- [ ] Show progression/rewards screen
- [ ] No misleading content
- [ ] High quality, no pixelation
- [ ] Correct aspect ratios

### Marketing Copy

- [ ] **App Name**: "Thermal Hunt" or approved variant
- [ ] **Subtitle/Short Description**: Compelling tagline (30 chars iOS, 80 chars Android)
  - Example: "Hunt rats with thermal vision!"
- [ ] **Keywords** (iOS): 100 characters, comma-separated
  - thermal, hunt, hunting, shooter, tactical, scope, rats, sniper, night vision, action
- [ ] **Full Description**: Engaging 4000 character description
  - Hook in first 2 lines
  - Feature bullets
  - Call to action

---

## 7. Legal & Compliance

### Required Legal Documents

- [ ] **Privacy Policy**
  - [ ] URL: `https://yourdomain.com/privacy`
  - [ ] Accessible 24/7
  - [ ] Covers:
    - What data is collected
    - How data is used
    - How data is stored
    - Third-party services (Unity Ads, Analytics)
    - User rights (access, deletion)
    - Contact information
  - [ ] GDPR compliant (EU)
  - [ ] COPPA compliant (if applicable)
  - [ ] CalOPPA compliant (California)

- [ ] **Terms of Service** (Optional but recommended)
  - [ ] URL: `https://yourdomain.com/terms`
  - [ ] Covers:
    - License to use app
    - User conduct rules
    - IAP terms (non-refundable, etc.)
    - Limitation of liability
    - Governing law

- [ ] **EULA** (End User License Agreement)
  - Can use Apple's standard EULA
  - Or create custom EULA if needed

### Age Rating

#### iOS Age Rating
- [ ] 4+ (unlikely due to violence)
- [ ] 9+ (possible if cartoonish)
- [x] **12+ (recommended)** - Infrequent/Mild:
  - Realistic Violence
  - Horror/Fear Themes
- [ ] 17+ (if mature content)

#### Android Content Rating (IARC)
- [ ] Everyone
- [ ] Everyone 10+
- [x] **Teen (recommended)** - Fantasy Violence
- [ ] Mature 17+

### Compliance Checklists

#### GDPR (EU Users)
- [ ] Privacy policy includes GDPR disclosures
- [ ] User can request data deletion
- [ ] User can export their data
- [ ] Cookie consent (if web-based features)
- [ ] Data breach notification plan

#### COPPA (US, under 13)
- [ ] App not directed at children under 13
- [ ] If under 13: Parental consent mechanism
- [ ] No personal data collection from minors
- [ ] Age gate at app start (if needed)

#### Advertising Compliance
- [ ] Ads clearly marked
- [ ] No misleading ads
- [ ] Ad content appropriate for age rating
- [ ] Frequency capping implemented
- [ ] Rewarded ads are opt-in
- [ ] No ads during critical gameplay

#### IAP Compliance
- [ ] Restore purchases button present
- [ ] Purchase confirmation before charging
- [ ] No accidental purchases
- [ ] Clear pricing displayed
- [ ] Parental gate for purchases (if kids game)

---

## 8. Pre-Submission Final Checks

### Code Quality

- [ ] No compiler warnings
- [ ] No runtime errors in logs
- [ ] All debug logging disabled or reduced
- [ ] No hardcoded API keys or secrets
- [ ] Code obfuscation enabled (optional)
- [ ] License keys for third-party assets valid

### Build Quality

- [ ] Build size optimized (< 150MB)
- [ ] App launches in < 5 seconds
- [ ] No splash screen crashes
- [ ] Proper version numbering:
  - iOS: Version 1.0.0, Build 1
  - Android: Version 1.0.0, Version Code 1
- [ ] Build signed with production certificate
- [ ] Build uploaded to respective consoles

### Pre-Submission Testing

- [ ] Install from TestFlight/Internal Testing
- [ ] Fresh install test (no previous data)
- [ ] Update install test (if update)
- [ ] Run through full gameplay session
- [ ] Check all IAPs work
- [ ] Check all ads work
- [ ] Verify analytics tracking
- [ ] No crashes during 30-minute session

### Metadata Review

- [ ] All text proofread (no typos)
- [ ] All images high quality
- [ ] All links work (privacy policy, support, etc.)
- [ ] Keywords optimized for ASO
- [ ] App description compelling
- [ ] Ratings and reviews prepared for launch

### Support Infrastructure

- [ ] Support email monitored: support@yourstudio.com
- [ ] Support website live (if applicable)
- [ ] FAQ page created
- [ ] Social media accounts created:
  - [ ] Twitter/X
  - [ ] Instagram
  - [ ] Facebook
  - [ ] Discord (optional)
- [ ] Press kit prepared (if marketing)

### Post-Launch Monitoring

- [ ] Analytics dashboard set up
- [ ] Crash reporting enabled (Unity Cloud Diagnostics)
- [ ] App Store Connect notifications enabled
- [ ] Google Play Console notifications enabled
- [ ] Review monitoring tool set up
- [ ] Hotfix plan prepared (critical bugs)

---

## Submission Process

### iOS Submission Steps

1. [ ] Build .ipa file in Unity
2. [ ] Open Xcode project
3. [ ] Archive build in Xcode
4. [ ] Upload to App Store Connect
5. [ ] Wait for processing (10-60 minutes)
6. [ ] Fill out all metadata in App Store Connect
7. [ ] Submit for review
8. [ ] Monitor review status
9. [ ] Respond to any reviewer feedback
10. [ ] Once approved, release manually or automatically

**Typical Review Time**: 24-48 hours (can be longer)

### Android Submission Steps

1. [ ] Build .aab file in Unity
2. [ ] Upload to Google Play Console
3. [ ] Create release (Production, Beta, or Internal)
4. [ ] Add release notes
5. [ ] Review pre-launch report
6. [ ] Fill out all store listing content
7. [ ] Submit for review
8. [ ] Monitor review status
9. [ ] Once approved, roll out to production

**Typical Review Time**: Few hours to 7 days

---

## Common Rejection Reasons & How to Avoid

### iOS Rejections

| Reason | Prevention |
|--------|------------|
| Crashes on launch | Test on multiple devices, fix all crashes |
| Incomplete information | Fill out all required metadata |
| Privacy policy missing | Provide active URL |
| Misleading content | Ensure screenshots match gameplay |
| IAP doesn't work | Test restore purchases thoroughly |
| Guideline 4.3 (spam) | Ensure app is unique and valuable |

### Android Rejections

| Reason | Prevention |
|--------|------------|
| Content policy violation | Review content guidelines carefully |
| Data safety form incomplete | Complete all data disclosure questions |
| Crashes | Fix crashes shown in pre-launch report |
| Misleading metadata | Accurate descriptions and screenshots |
| Broken IAP | Test all purchase flows |
| Missing permissions justification | Explain why each permission is needed |

---

## Final Checklist Summary

### Critical Must-Haves

- [ ] App builds successfully for both platforms
- [ ] No crashes during gameplay
- [ ] All features functional
- [ ] Ads and IAP work correctly
- [ ] Performance meets targets
- [ ] Privacy policy live
- [ ] All store assets uploaded
- [ ] Age ratings correct
- [ ] Tested on real devices

### Launch Day Preparation

- [ ] Press release ready (if applicable)
- [ ] Social media posts scheduled
- [ ] Support team briefed
- [ ] Monitoring dashboard open
- [ ] Hotfix build prepared (just in case)

---

**Status**: Use this checklist throughout your deployment process. Check off items as you complete them.

**Last Updated**: 2025-11-18
