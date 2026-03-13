# Imperium Events

<p align="center">
<img src="https://raw.githubusercontent.com/Caen007/ImperiumEvents/main/img/Start_and_Finish_Gate.png" width="900">
</p>

<p align="center">
<b>A Valheim event mod created for Imperium Hosting.</b>
</p>

<p align="center">
Host races, competitions, and community events directly inside your Valheim server.
</p>

---

# ⚔ Imperium Events

**Imperium Events** is a race and event framework designed for **Imperium Hosting community events**.

It allows servers to create structured competitions such as the **Imperium Triathlon**, where players can register, race, and automatically record their results.

Perfect for:

- Community servers
- Imperium Hosting events
- PvE competitions
- Streamer hosted races
- Server tournaments

---

# 🪓 What This Mod Adds

• Start & Finish race gates  
• Event decoration banners  
• Directional race banners  
• Safe zone and danger markers  
• Player registration rune  
• Race finish rune  
• Imperium horn (admin race start)  
• Live scoreboard  
• Automatic race result tracking  

Just install and host your own Valheim race events.

---

# 📸 Screenshots

---

## Start & Finish Gates

Used to mark the **start and finish of the race course**.

<p align="center">
<img src="https://raw.githubusercontent.com/Caen007/ImperiumEvents/main/img/Start_and_Finish_Gate.png" width="900">
</p>

---

## Event Banners

Decorate event areas or mark the race hub.

<p align="center">
<img src="https://raw.githubusercontent.com/Caen007/ImperiumEvents/main/img/Imperium_and_Welcome_Banner.png" width="850">
</p>

---

## Directional Banners

Guide players through the race route.

<p align="center">
<img src="https://raw.githubusercontent.com/Caen007/ImperiumEvents/main/img/DirectionalBanners.png" width="900">
</p>

---

## Danger & Safe Zone Banners

Used to mark dangerous areas or restricted zones during events.

<p align="center">
<img src="https://raw.githubusercontent.com/Caen007/ImperiumEvents/main/img/Danger_and_safe_Banners.png" width="900">
</p>

---

## Event Control Objects

These objects control the race system.

Includes:

- Start Rune (player registration)
- Finish Rune (race completion)
- Imperium Horn (race start)
- Scoreboard (race status)

<p align="center">
<img src="https://raw.githubusercontent.com/Caen007/ImperiumEvents/main/img/Start_and_fnish_rune_horn_scoreboard.png" width="900">
</p>

---

## Hammer Build Tab

All event pieces appear under the **ImperiumEvents hammer tab**.

<p align="center">
<img src="https://raw.githubusercontent.com/Caen007/ImperiumEvents/main/img/HammerTab.png">
</p>

---

# ⚔ How The Event Works

### Player Registration

Players interact with the **Start Rune** to register for the race.

---

### Race Start

An **admin uses the Imperium Horn** to start the race.

A countdown begins before the race starts.

---

### Race Progress

The **scoreboard updates live**, showing:

- Registered racers
- Players currently racing
- Players who finished
- Race times

---

### Finish

Players interact with the **Finish Rune** when they reach the finish line.

Their completion time is automatically recorded.

---

### Results

When all racers finish:

• Final rankings appear on the scoreboard  
• Results popup appears  
• Results are saved automatically  

---

# 📊 Race Results File

Race results are automatically saved to:

```
BepInEx/config/ImperiumTriathlonResults.json
```

The file stores:

- Player names
- Finish times
- Final rankings

---

# ⚙ Configuration

Config file location:

```
BepInEx/config/Imperium.Events.cfg
```

### AdminPlayerNames

Players allowed to start or reset races.

Example:

```
AdminPlayerNames = Caenos,James
```

---

### CountdownSeconds

Race countdown before start.

Default:

```
5
```

---

# 🔨 Installation

### 1 Install dependencies

BepInEx Pack  
https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/

Jotunn  
https://valheim.thunderstore.io/package/ValheimModding/Jotunn/

---

### 2 Install the mod

Place the mod `.dll` inside:

```
BepInEx/plugins/
```

---

### 3 Launch Valheim

The event pieces will appear in the **Hammer build menu**.

---

# 🧾 Credits

Created by **Caenos**

Developed for **Imperium Hosting community events**.

---

## Special thanks :

- TheUndertaker – for testing and feedback.
- Everyone who helped test the Imperium events.
---

<p align="center">
<b>Bring organized competition to your Valheim server.</b>
</p>