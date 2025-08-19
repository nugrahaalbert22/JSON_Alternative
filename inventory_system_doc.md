# JSON Alternative System Documentation

This document explains how the **JSON Alternative Inventory System** works for your Gorilla Tag fangame.

---

## Overview
The system allows you to:
- Assign cosmetic or mod items to specific players by their **Photon player name**.
- Store and load those assignments permanently using a JSON file.
- Detect inactive GameObjects in the scene and activate them only for the intended player.
- Manage multiple players and multiple items through the editor window.

---

## Core Components

### 1. **Editor Window**
- A custom Unity Editor Window provides an easy interface to:
  - Select scene items.
  - Assign items to player names.
  - Save and load assignments.
- No need to attach scripts manually—everything is handled via the editor.

### 2. **Player-Item Data**
- Each player is identified by their **Photon Player Name**.
- Each item is referenced by its **inactive GameObject** in the scene.
- Items assigned to a player are stored as entries in the JSON file.

Example JSON entry:
```json
{
  "PlayerName": "BAMBY",
  "Items": ["HatItem", "CoolGlasses"]
}
```

### 3. **Runtime Detection**
- When a player joins:
  - The system checks their Photon name.
  - If their name exists in the saved JSON file, the system finds the corresponding inactive items.
  - Those items are enabled **only for that player**.

### 4. **Permanent Saving**
- Assignments are saved in `inventory.json` inside Unity’s persistent data path.
- Even if the game restarts, the JSON is reloaded and items are reassigned.

---

## Workflow

### Step 1: Assign Items in Editor
1. Open the **Grant Items** Editor Window.
2. Pick inactive GameObjects from the scene.
3. Enter the Photon player name(s).
4. Save to JSON.

### Step 2: Player Joins
1. The system detects the player’s Photon name.
2. Looks up their saved data in `inventory.json`.
3. Finds assigned inactive objects in the scene.
4. Activates them only for that player.

### Step 3: Persistence
- Items remain permanently bound to the player name.
- Rejoining or restarting the game restores their cosmetics.

---

## Key Features
- **Automatic Detection:** No need to grant items at runtime manually.
- **Multiple Players:** Each player can have a different set of items.
- **Multiple Items:** Players can hold multiple cosmetics.
- **Permanent Storage:** Uses JSON for saving and loading.
- **Editor-Friendly:** All setup handled via Unity Editor Window.

---

## Example Use Case
- Player `BAMBY` joins → System detects the name → Activates `GoldenCrown` GameObject → Saves assignment to JSON → Next time `BAMBY` joins, they still have the crown.

---

## JSON Logo
You designed a custom logo for the system (JSON Alternative). You can use it as the icon or splash for the tool.

---

## Future Improvements
- Add support for removing items via editor.
- UI in-game for players to view owned cosmetics.
- Network synchronization for multiplayer confirmation.

---

**In summary:**
This system ensures **permanent, player-specific cosmetic unlocks** tied to Photon names, managed entirely in Unity’s Editor, and stored via JSON.

