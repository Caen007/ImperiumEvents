# Imperium Events

<p align="center">
<img src="https://raw.githubusercontent.com/Caen007/ImperiumEvents/main/img/Start_and_Finish_Gate.png" width="900">
</p>

<p align="center">
<b>A Valheim event mod for hosting organized server competitions.</b>
</p>

<p align="center">
Create race events, competitions, and community challenges directly inside Valheim.
</p>

---

# ⚔ Imperium Events

**Imperium Events** adds a full **race event system** to Valheim.

Players can **register, race, and finish events**, while the server automatically tracks results and rankings.

Perfect for:

- Community servers
- PvE competitions
- Streamer hosted events
- Server tournaments

---

# 🪓 Features

✔ Race registration system  
✔ Admin-controlled race start  
✔ Automatic race timer  
✔ Live scoreboard updates  
✔ Automatic race results  
✔ JSON results export  
✔ Event gates and banners  
✔ Directional race markers  

---

# 🏁 Start & Finish Gates

Used to mark the **start and finish of the race course**.

<p align="center">
<img src="https://raw.githubusercontent.com/Caen007/ImperiumEvents/main/img/Start_and_Finish_Gate.png" width="900">
</p>

---

# 🏳 Event Banners

Decorate event areas and mark race hubs.

<p align="center">
<img src="https://raw.githubusercontent.com/Caen007/ImperiumEvents/main/img/Imperium_and_Welcome_Banner.png" width="800">
</p>

---

# 🧭 Directional Banners

Guide players through the race route.

<p align="center">
<img src="https://raw.githubusercontent.com/Caen007/ImperiumEvents/main/img/DirectionalBanners.png" width="900">
</p>

---

# ⚠ Danger & Safe Zone Markers

Used to mark dangerous areas or safe zones during events.

<p align="center">
<img src="https://raw.githubusercontent.com/Caen007/ImperiumEvents/main/img/Danger_and_safe_Banners.png" width="900">
</p>

---

# 📢 Event Control Objects

These objects control the race system.

Includes:

- Start Rune (player registration)
- Finish Rune (race completion)
- Imperium Horn (admin race start)
- Scoreboard (race status)

<p align="center">
<img src="https://raw.githubusercontent.com/Caen007/ImperiumEvents/main/img/Start_and_fnish_rune_horn_scoreboard.png" width="900">
</p>

---

# 🔨 Hammer Build Tab

All pieces appear in the **custom hammer tab**.

<p align="center">
<img src="https://raw.githubusercontent.com/Caen007/ImperiumEvents/main/img/HammerTab.png">
</p>

---

# ⚔ How The Event Works

### 1️⃣ Player Registration

Players interact with the **Start Rune** to register for the race.

---

### 2️⃣ Race Start

An **admin uses the Imperium Horn** to start the race.

A countdown begins before the race starts.

---

### 3️⃣ Race Progress

The **scoreboard updates live**, showing:

- Registered racers
- Players currently racing
- Players who finished
- Race times

---

### 4️⃣ Finish

Players interact with the **Finish Rune** when they reach the finish line.

Their completion time is automatically recorded.

---

### 5️⃣ Results

When all racers finish:

- Final rankings appear on the scoreboard
- A results popup appears
- Results are saved automatically

---

# 📊 Race Results File

Race results are automatically saved to:

```
BepInEx/config/ImperiumTriathlonResults.json
```

The file contains:

- Player names
- Finish times
- Final rankings

---

# ⚙ Configuration

Config file:

```
BepInEx/config/Imperium.Events.cfg
```

### AdminPlayerNames

Players allowed to start or reset races.

Example:

```
AdminPlayerNames = Caenos,James
```

### CountdownSeconds

Race countdown before start.

Default:

```
5
```

---

# 🔧 Installation

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

### 3 Start the game

The event pieces will appear in the **Hammer build menu**.

---

# 🧾 Credits

Created by **Caenos**

---

## Special Thanks

- Community testers
- Imperium event participants

---

<p align="center">
<b>Host epic races and bring competition to your Valheim server.</b>
</p>