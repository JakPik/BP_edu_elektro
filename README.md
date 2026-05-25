# Edukativní 3D puzzle hra – elektrické obvody (Unity projekt)

## Abstrakt

Tento projekt představuje prototyp edukativní 3D puzzle hry, ve které hráč řeší prostorově navržené úrovně (místnosti) prostřednictvím zapojování a aktivace elektrických obvodů.

Hlavní herní smyčka je založena na postupném řešení uzavřených prostorových hádanek, kde elektrické obvody slouží jako interaktivní mechanismus pro zpřístupnění další části úrovně.

Aplikace je implementována v herním enginu Unity a využívá zjednodušený model elektrických obvodů založený na uzlové (node-based) reprezentaci. Tento model není zamýšlen jako plnohodnotný fyzikální simulátor, ale jako didakticko-herní abstrakce umožňující čitelné a konzistentní řešení herních úloh.

---

## Herní koncept (core design)

Základní princip hry:

- Hráč se pohybuje v 3D prostředí rozděleném na místnosti
- Každá místnost obsahuje samostatný logický problém
- Pro postup dále je nutné vyřešit elektrický obvod v dané místnosti
- Po splnění podmínek dojde k odemknutí další části úrovně

Elektrické obvody zde slouží jako:

- herní mechanismus pro progres
- logický problémový systém
- nástroj pro řízení interakce v prostředí

---

## Požadavky na spuštění

### Minimální systémové požadavky

- OS: Windows 10 / 11 (64-bit)
- Disk: 4 GB volného místa

---

## Instalace vývojového prostředí

### 1. Unity Hub

Stáhnout zde:
https://unity.com/download

Postup:

1. Instalace Unity Hub
2. Přihlášení do Unity účtu
3. Instalace Unity Editoru

---

### 2. Unity Editor

Doporučená verze:

- Unity 6

Doporučené moduly:

- Visual Studio (C# tooling)

---

## Stažení projektu

### Varianta A – Git

```bash
git clone https://github.com/JakPik/BP_edu_elektro.git
```

### Varianta B – ZIP

- stáhnout a rozbalit projekt
- umístit do adresáře bez diakritiky

## Otevření projektu

1. Otevřít Unity Hub
2. Kliknout na Open Project
3. Vybrat root složku projektu (obsahuje Assets/, ProjectSettings/)
4. Počkat na import a inicializaci

## Spuštění projektu

1. Otevřít:

```
Assets/Scenes/
```

2. Vybrat hlavní scénu:
    - např. DemoScene.unity
3. Spustit tlačítkem:

```
Play
```

## Vytvoření buildu (EXE)

### 1. Build Settings

```
File → Build Settings
```

### 2. Přidání scény

- Add Open Scenes

### 3. Platforma

- PC, Mac & Linux Standalone
- Windows

Kliknout:
Switch Platform

### 4. Build

Kliknout:
Build

Zvolit výstupní složku (např. Build/)

## Výstup

Po buildu vzniká:

- spustitelný .exe
- datová složka s assety

## Použité Unity balíčky

- Input System
- Unity Splines
- Core Unity Modules

Instalace:

```
Window → Package Manager
```

## Ověření funkčnosti

Po spuštění musí být splněno:

- načtení hlavní scény
- funkční pohyb hráče
- funkční interakce s objekty
- správné chování puzzle systému v místnosti
- možnost postupu mezi místnostmi

## Ovládání

- WASD / šipky – pohyb
- Myš – kamera
- E – interakce
- F - uchopení objetů
- R - restart místnosti
- ESC – menu (pokud je implementováno)

## Charakter projektu

Projekt je implementován jako proof of concept (POC) edukativní puzzle hry.

Hlavní zaměření:

- návrh 3D puzzle struktury založené na místnostech
- využití elektrických obvodů jako logického mechanismu
- testování interaktivního edukačního gameplaye
- ověření čitelnosti a srozumitelnosti uzlové reprezentace

Nejedná se o plnohodnotný fyzikální simulátor.

## Autor

Jakub Pikal
2026

Bakalářská práce – Edukativní 3D puzzle hra (elektrické obvody)
