# Thermal Hunt - App Store Assets Guide

Complete guide for creating and preparing all assets needed for Apple App Store and Google Play Store submissions.

---

## Table of Contents

1. [Asset Overview](#asset-overview)
2. [App Icons](#app-icons)
3. [Screenshots](#screenshots)
4. [Feature Graphics](#feature-graphics)
5. [Promotional Videos](#promotional-videos)
6. [Marketing Copy](#marketing-copy)
7. [Asset Creation Tools](#asset-creation-tools)
8. [Quality Guidelines](#quality-guidelines)

---

## 1. Asset Overview

### Complete Asset Checklist

#### iOS App Store
- [ ] App Icon: 1024×1024 PNG
- [ ] iPhone 14 Pro Max screenshots (1290×2796): 3-10 images
- [ ] iPhone 11 Pro Max screenshots (1242×2688): 3-10 images
- [ ] iPhone 8 Plus screenshots (1242×2208): 2-10 images (optional)
- [ ] iPad Pro 12.9" screenshots (2048×2732): 2-5 images (if iPad support)
- [ ] App Preview videos: 15-30 seconds (optional)

#### Google Play Store
- [ ] App Icon: 512×512 PNG
- [ ] Feature Graphic: 1024×500 PNG/JPG
- [ ] Phone screenshots (1080×1920): 2-8 images
- [ ] 7" Tablet screenshots (1200×1920): 1-8 images (optional)
- [ ] 10" Tablet screenshots (1920×1200): 1-8 images (optional)
- [ ] Promo video: YouTube link, 30 sec - 2 min (optional)

---

## 2. App Icons

### iOS App Icon Requirements

**Main Icon**: 1024×1024 pixels
- Format: PNG (no transparency, no alpha channel)
- Color space: RGB
- No rounded corners (iOS adds them automatically)
- Design should be simple and recognizable at small sizes

**All Required Sizes** (Unity handles these automatically):
- 20×20, 29×29, 40×40, 58×58, 60×60, 76×76, 80×80, 87×87, 120×120, 152×152, 167×167, 180×180, 1024×1024

### Android App Icon Requirements

**Play Store Icon**: 512×512 pixels
- Format: 32-bit PNG with alpha channel
- Color space: RGB
- Can have transparency

**Adaptive Icon** (Recommended):
- Foreground layer: 108×108 DP (432×432 pixels @4x)
- Background layer: 108×108 DP (432×432 pixels @4x)
- Safe zone: Center 72×72 DP (288×288 pixels @4x)
- Can be different shapes on different devices

### Icon Design Concept for Thermal Hunt

**Concept**: Thermal scope view with rat silhouette

```
Design Elements:
- Background: Dark black/navy (night theme)
- Center: Circular thermal scope reticle
- Inside scope: Bright white/yellow rat silhouette (heat signature)
- Color gradient: Blue (cold) to yellow/white (hot)
- Accent: Red crosshair or targeting reticle
- Style: Clean, modern, recognizable

Colors:
- Primary: #000000 (black)
- Secondary: #FFFFFF (white/hot)
- Accent: #FF4444 (red crosshair)
- Thermal gradient: #1a1a3e → #ffdd00

Typography (if text):
- Font: Bold, sans-serif
- Text: "TH" or minimal
```

### Icon Mockup Template

```
┌─────────────────────────┐
│   ╔═══════════════╗     │
│   ║   Thermal     ║     │
│   ║   ┌───────┐   ║     │
│   ║   │  RAT  │   ║     │ ← White/yellow rat (hot)
│   ║   │ (hot) │   ║     │
│   ║   └───────┘   ║     │
│   ║   ╬─────╬     ║     │ ← Red crosshair
│   ║       ║       ║     │
│   ╚═══════════════╝     │
│   Dark Background       │
└─────────────────────────┘
```

---

## 3. Screenshots

### Screenshot Strategy

**Goal**: Show key features and gameplay in 5-8 images

**Recommended Screenshot Sequence**:

1. **Hero Shot** - Thermal scope view with rat in crosshair
   - Shows main gameplay mechanic
   - Thermal effect prominent
   - Engaging action moment

2. **Environment Shot** - Farm setting with multiple rats visible
   - Shows game setting
   - Multiple targets visible through thermal
   - Demonstrates scale

3. **HUD/UI Shot** - Gameplay with UI elements visible
   - Ammo counter
   - Mission timer
   - Score display
   - Battery indicator

4. **Mission Select** - Contract selection screen
   - Shows progression system
   - Multiple missions available
   - Star ratings visible

5. **Loadout Screen** - Weapon and scope selection
   - Shows customization options
   - Multiple weapons displayed
   - Upgrade system visible

6. **Results Screen** - Mission complete with 3 stars
   - Shows rewards
   - Star rating
   - Score breakdown
   - Progression satisfying

7. **Action Shot** - Mid-mission intense moment
   - Thermal vision active
   - Multiple rats fleeing
   - Dynamic gameplay

8. **Scope Detail** - Close-up of thermal effect
   - Beautiful thermal rendering
   - Heat signatures clear
   - Technical showcase

### Screenshot Technical Specs

#### iOS Specifications

| Device | Resolution | Aspect Ratio | Priority |
|--------|------------|--------------|----------|
| iPhone 14 Pro Max | 1290×2796 | 19.5:9 | Required |
| iPhone 11 Pro Max | 1242×2688 | 19.5:9 | Required |
| iPhone 8 Plus | 1242×2208 | 16:9 | Optional |
| iPad Pro 12.9" | 2048×2732 | 4:3 | If iPad |

#### Android Specifications

| Type | Resolution | Aspect Ratio | Priority |
|------|------------|--------------|----------|
| Phone | 1080×1920 | 16:9 or 9:16 | Required |
| Phone | 1080×2340 | 19.5:9 | Recommended |
| 7" Tablet | 1200×1920 | 10:16 | Optional |
| 10" Tablet | 1920×1200 | 16:10 | Optional |

### Screenshot Capture Process

```
Unity Screenshot Capture:

1. Set Game view to target resolution
2. Disable editor UI
3. Play game to desired moment
4. Use Unity's Screenshot API:

   // Add to a debug script
   void Update()
   {
       if (Input.GetKeyDown(KeyCode.F12))
       {
           string filename = "Screenshot_" +
               System.DateTime.Now.ToString("yyyyMMdd_HHmmss") +
               ".png";
           ScreenCapture.CaptureScreenshot(filename, 2); // 2x supersize
       }
   }

5. Screenshots saved to project root folder
6. Post-process in Photoshop/Figma
```

### Screenshot Composition Tips

**Do:**
- ✅ Show actual gameplay (not mockups)
- ✅ Use high resolution (2x or 4x)
- ✅ Clean UI (no debug text)
- ✅ Interesting moments (action, not idle)
- ✅ Variety (different environments, situations)
- ✅ Consistent branding
- ✅ Text overlays minimal (if any)

**Don't:**
- ❌ Fake/staged screenshots
- ❌ Low quality or pixelated
- ❌ Development UI visible
- ❌ Boring moments
- ❌ Too similar screenshots
- ❌ Misleading content

---

## 4. Feature Graphics

### Google Play Feature Graphic

**Required for Android Play Store**

**Specs**:
- Size: 1024×500 pixels
- Format: PNG or JPG
- Max file size: 1MB
- No transparency

**Design Concept**:

```
┌───────────────────────────────────────────────────────────┐
│                                                           │
│  THERMAL HUNT           [Thermal Scope View]    [Logo]   │
│  Hunt Rats at Night     [Rat Silhouette Hot]    [Icon]   │
│                         [Crosshair + Effect]             │
│  ⭐⭐⭐ Epic Hunting      [Dark Background]               │
│                                                           │
└───────────────────────────────────────────────────────────┘
     <------- 1024px ------->
```

**Design Elements**:
- Left 1/3: App name + tagline
- Center 1/3: Key visual (thermal scope/rat)
- Right 1/3: App icon + rating/badges
- Background: Dark (night theme)
- Colors: Thermal gradient (blue→yellow)

**Text Content Ideas**:
- "THERMAL HUNT"
- "Hunt Rats with Night Vision"
- "Tactical Hunting Simulator"
- "Authentic FLIR Experience"

---

## 5. Promotional Videos

### Video Requirements

#### iOS App Preview

**Specs**:
- Length: 15-30 seconds
- Format: H.264 or ProRes
- Resolution: Match screenshot sizes
- No audio narration (music okay)
- First 3 seconds are preview in browse
- Video loops automatically

**Recommended Content** (30 seconds):
```
00:00 - 00:03: Hook - Thermal scope view with rat
00:03 - 00:08: Gameplay - Shooting rats
00:08 - 00:13: Feature - Show different scopes/weapons
00:13 - 00:18: Mission - Contract completion screen
00:18 - 00:23: Action - Multiple rats fleeing
00:23 - 00:28: Reward - Star rating and rewards
00:28 - 00:30: CTA - App icon + "Download Now"
```

#### Android Promo Video

**Specs**:
- YouTube video link
- Length: 30 seconds to 2 minutes
- Format: Any YouTube-supported format
- Can include voiceover/narration
- Must comply with Google Play policies

**Recommended Content** (60 seconds):
```
00:00 - 00:05: Hook - "Ever hunted rats with thermal vision?"
00:05 - 00:15: Gameplay overview
00:15 - 00:25: Feature showcase (scopes, weapons, contracts)
00:25 - 00:35: Progression (unlocks, upgrades)
00:35 - 00:45: Testimonials/reviews (if available)
00:45 - 00:55: Unique selling points
00:55 - 01:00: CTA - "Download now!"
```

### Video Creation Process

```
1. Script and storyboard
2. Record gameplay footage:
   - Unity Recorder package
   - OBS Studio
   - Built-in screen recording
3. Edit video:
   - DaVinci Resolve (free)
   - Adobe Premiere Pro
   - Final Cut Pro
4. Add music (royalty-free):
   - YouTube Audio Library
   - Epidemic Sound
   - Artlist
5. Add text overlays (minimal)
6. Export at required specs
7. Test on mobile devices
8. Upload to stores/YouTube
```

---

## 6. Marketing Copy

### App Name & Subtitle

**iOS**:
- **App Name**: "Thermal Hunt" (30 char max)
- **Subtitle**: "Night Vision Rat Hunting" (30 char max)

**Android**:
- **App Name**: "Thermal Hunt: Night Vision Hunting" (50 char max)
- **Short Description**: "Hunt rats with authentic thermal scope vision at night!" (80 char max)

### Keywords (iOS Only)

**100 characters max, comma-separated**

```
thermal,hunt,hunting,shooter,tactical,scope,rats,sniper,night vision,action,flir,simulation,pest,farm,precision
```

**Keyword Strategy**:
- Primary: thermal, hunt, hunting
- Secondary: shooter, tactical, scope
- Long-tail: night vision, thermal scope, rat hunting
- Competitors: (research competing apps)

### Full Description Template

**4000 characters max**

```markdown
🎯 THERMAL HUNT - The Ultimate Night Hunting Simulator

Experience the thrill of tactical rat hunting with authentic thermal vision technology. Use real FLIR thermal scopes to eliminate pests in the darkness of night. Are you ready for the hunt?

🔥 AUTHENTIC THERMAL VISION
• Real FLIR camera aesthetics
• Heat signature detection
• Battery management mechanics
• Multiple thermal scope upgrades
• Living targets glow bright against the cold night

🎮 TACTICAL GAMEPLAY
• Precision shooting mechanics
• Realistic ballistics simulation
• Hold breath for steady aim
• Strategic positioning matters
• Sound propagation - don't spook your targets!

🐀 INTELLIGENT RAT AI
• Realistic rodent behavior
• Rats flee when threatened
• Alert calls warn nearby rats
• Hide in unreachable spots
• Different rat types and sizes

🏆 PROGRESSION & UNLOCKS
• 15+ challenging contracts
• Multiple weapons and scopes
• Star rating system
• Daily challenges
• Experience and currency rewards
• Unlock new equipment and missions

📱 OPTIMIZED FOR MOBILE
• Smooth 60 FPS gameplay
• Intuitive touch controls
• Short 10-15 minute sessions
• Perfect for quick gaming
• Low battery consumption

🎨 UNIQUE MINECRAFT-STYLE GRAPHICS
• Procedural blocky aesthetics
• No large download required
• Distinctive visual style
• Lightweight build size

💎 FREE TO PLAY
• Complete game free
• Optional ads for rewards
• Premium unlocks available
• No pay-to-win mechanics

🌟 FEATURES
✓ Authentic thermal rendering
✓ 7-state rat AI behavior
✓ Physics-based ballistics
✓ Multiple environments
✓ Campaign mode
✓ Battery upgrade system
✓ Rewarding progression
✓ Offline play supported

📊 PERFECT FOR
• Hunting enthusiasts
• Tactical shooter fans
• Precision gaming lovers
• Quick session players
• Strategy gamers

🎯 HUNT. SCOPE. ELIMINATE.

Download Thermal Hunt now and experience the most authentic thermal hunting simulation on mobile!

---

⭐ Join thousands of hunters worldwide
🎮 New content updated regularly
📧 Support: support@yourstudio.com
🌐 Privacy: https://yourstudio.com/privacy
```

### What's New (Update Notes)

**Template for version 1.0.0**:

```
🎉 THERMAL HUNT LAUNCH

Welcome to Thermal Hunt! The ultimate thermal vision hunting simulator.

🆕 Launch Features:
• 15 challenging contracts
• 3 authentic thermal scopes
• 3 precision rifles
• Campaign mode for 10-15 min sessions
• Daily challenges system
• Complete progression system

🔥 What Makes Us Special:
• Authentic FLIR thermal rendering
• Realistic rat AI behavior
• Physics-based bullet ballistics
• Unique Minecraft-style aesthetics

Thank you for playing! We'd love to hear your feedback.

Happy hunting! 🎯
```

---

## 7. Asset Creation Tools

### Recommended Software

#### Free Tools
- **GIMP** - Image editing (Photoshop alternative)
- **Inkscape** - Vector graphics
- **Figma** - UI/UX design (free tier)
- **Canva** - Quick graphics (free tier)
- **DaVinci Resolve** - Video editing
- **Blender** - 3D modeling (if needed)
- **OBS Studio** - Screen recording

#### Paid Tools (Professional)
- **Adobe Photoshop** - Industry standard image editing
- **Adobe Illustrator** - Vector graphics
- **Adobe Premiere Pro** - Video editing
- **Sketch** - UI/UX design (Mac only)
- **Affinity Designer** - Affordable Illustrator alternative

### Asset Templates

#### Photoshop Icon Template (1024×1024)

```
Layers:
1. Background (dark gradient)
2. Thermal effect (gradients)
3. Scope reticle (vector shapes)
4. Rat silhouette (white/yellow)
5. Crosshair (red, blend mode: Screen)
6. Vignette (dark edges)
7. Text (optional, app name)

Export:
- Format: PNG-24
- No transparency
- sRGB color profile
- Maximum quality
```

#### Feature Graphic Template (1024×500)

```
Figma/Sketch Layers:
1. Background (dark blue to black gradient)
2. Thermal scope visual (center)
3. App name (left, large text)
4. Tagline (left, smaller text)
5. App icon (right)
6. Rating stars (if available)
7. Decorative elements (minimal)

Export:
- Format: PNG or JPG
- 1024×500 pixels
- Compressed < 1MB
- sRGB color profile
```

---

## 8. Quality Guidelines

### Image Quality Checklist

- [ ] **Resolution**: Exact pixel dimensions required
- [ ] **Format**: PNG or JPG as specified
- [ ] **File size**: Under limits (1MB for most)
- [ ] **Color space**: sRGB (not CMYK)
- [ ] **Compression**: Optimized but not over-compressed
- [ ] **Sharpness**: Clear, not blurry
- [ ] **Text**: Readable at small sizes
- [ ] **Branding**: Consistent across all assets
- [ ] **No artifacts**: No compression artifacts or noise

### Content Guidelines

- [ ] **Accurate**: Shows actual gameplay
- [ ] **High quality**: Professional appearance
- [ ] **Appropriate**: Matches age rating (12+)
- [ ] **Consistent**: Unified visual style
- [ ] **Compelling**: Draws attention
- [ ] **Clear**: Easy to understand at a glance
- [ ] **No clutter**: Clean composition
- [ ] **Platform compliance**: Follows store guidelines

### Common Mistakes to Avoid

❌ **Don't**:
- Use low resolution images
- Include watermarks or logos from other services
- Show outdated UI or features
- Use placeholder text ("Lorem ipsum")
- Include competitor branding
- Make false claims
- Use copyrighted content
- Over-promise features not in game

✅ **Do**:
- Use high-res original screenshots
- Show current version of game
- Match actual gameplay experience
- Use final UI and graphics
- Highlight unique features
- Be honest about content
- Use original or licensed content
- Set realistic expectations

---

## Quick Reference: All Required Assets

### iOS Minimum Requirements
```
☑ App Icon: 1024×1024 PNG (no alpha)
☑ iPhone 14 Pro Max: 3 screenshots (1290×2796)
☑ iPhone 11 Pro Max: 3 screenshots (1242×2688)
☑ App Description: Up to 4000 chars
☑ Keywords: Up to 100 chars
☑ Subtitle: Up to 30 chars
```

### Android Minimum Requirements
```
☑ App Icon: 512×512 PNG (with alpha)
☑ Feature Graphic: 1024×500 PNG/JPG
☑ Phone Screenshots: 2 minimum (1080×1920)
☑ Short Description: Up to 80 chars
☑ Full Description: Up to 4000 chars
```

### Optional But Recommended
```
⭐ Promo video (iOS: 15-30s, Android: YouTube)
⭐ Tablet screenshots
⭐ Multiple phone orientations
⭐ Localized assets for other languages
```

---

## Asset Production Timeline

**Week 1**: Planning & Design
- [ ] Design app icon concepts
- [ ] Create feature graphic mockup
- [ ] Write marketing copy drafts

**Week 2**: Asset Creation
- [ ] Finalize icon design
- [ ] Capture all screenshots
- [ ] Create feature graphic
- [ ] Record promo video

**Week 3**: Polish & Review
- [ ] Edit and polish all assets
- [ ] Get feedback from team
- [ ] Make revisions
- [ ] Prepare for upload

**Week 4**: Upload & Test
- [ ] Upload to test environment
- [ ] Review in store listings
- [ ] Final adjustments
- [ ] Submit for review

---

## Resources

### Asset Dimension Quick Reference

```
iOS App Icon: 1024×1024 (PNG, no alpha)
Android Icon: 512×512 (PNG, with alpha)
Android Feature: 1024×500 (PNG/JPG)

iPhone 14 Pro Max: 1290×2796 (portrait)
iPhone 11 Pro Max: 1242×2688 (portrait)
iPhone 8 Plus: 1242×2208 (portrait)

Android Phone: 1080×1920 (portrait)
Android Phone: 1920×1080 (landscape)
```

### Useful Links

- [Apple App Store Screenshot Specs](https://developer.apple.com/app-store/product-page/)
- [Google Play Asset Guidelines](https://support.google.com/googleplay/android-developer/answer/9866151)
- [App Store Review Guidelines](https://developer.apple.com/app-store/review/guidelines/)
- [Google Play Policy Center](https://play.google.com/about/developer-content-policy/)

---

**Last Updated**: 2025-11-18
**Version**: 1.0
