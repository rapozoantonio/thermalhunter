# Contributing to Thermal Hunt

## Development Setup

### Prerequisites
- Unity 2022.3.20f1 LTS or later
- Visual Studio 2022 or JetBrains Rider
- Git 2.30+
- For mobile builds:
  - Xcode 14+ (iOS)
  - Android Studio 2022+ (Android)
- For PC builds:
  - Steam SDK (optional, for Steam features)

### Initial Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourstudio/thermalhunter.git
   cd thermalhunter
   ```

2. **Open in Unity**
   - Launch Unity Hub
   - Click "Add" and select the project folder
   - Ensure Unity 2022.3.20f1 LTS is installed
   - Open the project

3. **Install required packages**
   - Unity will automatically install packages from `Packages/manifest.json`
   - Wait for package resolution to complete

4. **Configure build settings**
   - See platform-specific instructions below

---

## Build Instructions

### iOS Build

#### Prerequisites
- macOS with Xcode 14+
- Apple Developer Account
- iOS device or simulator

#### Steps
1. **Switch to iOS platform**
   ```
   File > Build Settings > iOS > Switch Platform
   ```

2. **Configure iOS settings**
   ```
   Edit > Project Settings > Player > iOS
   - Bundle Identifier: com.yourstudio.thermalhunter
   - Target SDK: iOS 14.0+
   - Architecture: ARM64
   - Scripting Backend: IL2CPP
   ```

3. **Build to Xcode project**
   ```
   File > Build Settings > Build
   ```

4. **Open in Xcode**
   - Open generated `.xcodeproj` file
   - Select development team
   - Configure signing

5. **Build and run**
   - Select target device
   - Click Run (⌘R)

#### TestFlight Deployment
1. Archive the build in Xcode
2. Upload to App Store Connect
3. Submit for TestFlight review
4. Invite beta testers

---

### Android Build

#### Prerequisites
- Android Studio 2022+
- Android SDK (API Level 21+)
- Java JDK 11

#### Steps
1. **Switch to Android platform**
   ```
   File > Build Settings > Android > Switch Platform
   ```

2. **Configure Android settings**
   ```
   Edit > Project Settings > Player > Android
   - Package Name: com.yourstudio.thermalhunter
   - Minimum API Level: 21 (Android 5.0)
   - Target API Level: 33 (Android 13)
   - Scripting Backend: IL2CPP
   - Target Architectures: ARM64
   ```

3. **Build settings**
   ```
   File > Build Settings
   - Build System: Gradle
   - Build App Bundle (Google Play): ON
   ```

4. **Build**
   ```
   File > Build Settings > Build
   ```
   - This generates an `.aab` file for Google Play

#### Google Play Deployment
1. Create release in Google Play Console
2. Upload `.aab` file
3. Fill in store listing
4. Submit for review

---

### PC (Steam) Build

#### Prerequisites
- Steamworks SDK (optional)
- Steam Partner account

#### Steps
1. **Switch to Windows platform**
   ```
   File > Build Settings > Windows, Mac, Linux > Switch Platform
   ```

2. **Configure PC settings**
   ```
   Edit > Project Settings > Player > PC
   - Architecture: x86_64
   - Scripting Backend: IL2CPP
   ```

3. **Build**
   ```
   File > Build Settings > Build
   ```

4. **Steam integration** (optional)
   - Install Steamworks.NET plugin
   - Configure Steam App ID
   - Add achievement definitions
   - Test with Steam client

#### Steam Deployment
1. Upload build to Steam via SteamPipe
2. Configure store page
3. Set release date
4. Submit for review

---

## Code Style Guidelines

### C# Conventions
- Use PascalCase for public members
- Use camelCase for private members
- Prefix private fields with underscore: `_myField` (optional)
- Use `var` when type is obvious
- Always use braces `{}` for control flow
- Maximum line length: 120 characters

### Example
```csharp
public class MyComponent : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            rb.velocity = direction * speed;
        }
    }
}
```

### File Organization
- One class per file
- File name matches class name
- Group related files in folders
- Use namespaces for large systems

---

## Testing

### Manual Testing Checklist
- [ ] Thermal vision activates/deactivates
- [ ] Rats spawn correctly
- [ ] Shooting mechanics work
- [ ] Hit detection accurate
- [ ] Battery drains over time
- [ ] Mission objectives complete
- [ ] Save/load works
- [ ] Ads display (mobile only)
- [ ] IAP functions correctly
- [ ] No memory leaks
- [ ] Stable 60 FPS on target devices

### Performance Profiling
1. **Unity Profiler**
   ```
   Window > Analysis > Profiler
   ```
   - Monitor CPU, GPU, memory
   - Check GC allocations
   - Profile on device

2. **Mobile Profiling**
   - iOS: Xcode Instruments
   - Android: Android Profiler

3. **Target Metrics**
   - Mobile: 60 FPS @ 1080p
   - PC: 120 FPS @ 1080p
   - Memory: < 1GB
   - Build size: < 150MB

---

## Git Workflow

### Branch Strategy
- `main` - Production-ready code
- `develop` - Integration branch
- `feature/feature-name` - New features
- `bugfix/bug-description` - Bug fixes
- `hotfix/critical-fix` - Emergency fixes

### Commit Messages
Use conventional commits:
```
feat: add thermal battery recharge mechanic
fix: resolve rat AI pathfinding bug
docs: update build instructions
perf: optimize heat signature rendering
refactor: extract ballistics calculation to utility
```

### Pull Request Process
1. Create feature branch from `develop`
2. Make changes and commit
3. Push to remote
4. Create PR to `develop`
5. Wait for code review
6. Merge when approved

---

## Debugging

### Common Issues

#### Build Fails
- Check Unity version matches 2022.3.20f1
- Verify all packages are installed
- Clear Library folder and reimport

#### Performance Issues
- Enable IL2CPP code stripping
- Use object pooling for frequently spawned objects
- Reduce texture sizes
- Optimize shader complexity

#### Mobile-Specific
- Test on actual devices, not just simulators
- Check permissions in manifest
- Verify API levels are correct

---

## Asset Pipeline

### 3D Models
- Format: FBX
- Max poly count: 2000 tris (characters), 5000 tris (environments)
- Include LOD levels for distant objects

### Textures
- Format: PNG or TGA
- Max size: 2048x2048
- Use texture atlases where possible
- Compress using ASTC (mobile) or DXT (PC)

### Audio
- Format: WAV (source), Vorbis (runtime)
- Sample rate: 44.1kHz
- Bit depth: 16-bit
- Compress to reduce build size

---

## Release Checklist

### Pre-Release
- [ ] All tests pass
- [ ] No critical bugs
- [ ] Performance meets targets
- [ ] Store assets prepared (screenshots, videos)
- [ ] Privacy policy updated
- [ ] Terms of service updated

### Store Submission
- [ ] App icons (all sizes)
- [ ] Screenshots (all device sizes)
- [ ] Feature graphic
- [ ] Store description
- [ ] Keywords/tags
- [ ] Age rating
- [ ] Privacy policy URL

### Post-Release
- [ ] Monitor crash reports
- [ ] Track analytics
- [ ] Respond to reviews
- [ ] Plan hotfix if needed

---

## Contact

For questions or issues:
- Create a GitHub issue
- Email: dev@yourstudio.com
- Discord: [Your Discord Server]

---

## License

Proprietary - All rights reserved
See LICENSE file for details
